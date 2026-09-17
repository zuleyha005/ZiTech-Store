using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ECommerceMVC.Models;
using ECommerceMVC.Data;

namespace ECommerceMVC.Controllers
{
    [Authorize]
    [Route("Cart")]
    public class UserCartController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _context;

        public UserCartController(
    IHttpClientFactory httpClientFactory,
    ApplicationDbContext context)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        // Sepeti görüntüle
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;

            if (!int.TryParse(customerIdClaim, out int customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("CustomerApi");

                var carts = await client.GetFromJsonAsync<List<Cart>>("Cart");

                var userCart = carts?
                    .Where(c => c.CustomerId == customerId)
                    .ToList() ?? new List<Cart>();

                var products = await client.GetFromJsonAsync<List<Product>>("Product")
                               ?? new List<Product>();

                var cartItems = userCart.Select(cart =>
                {
                    var product = products.FirstOrDefault(p => p.Id == cart.ProductId);

                    return new CartItemViewModel
                    {
                        CartId = cart.Id,
                        ProductId = cart.ProductId,
                        Quantity = cart.Quantity,
                        ProductName = product?.Name ?? "Ürün bulunamadı",
                        Price = product?.Price ?? 0
                    };
                }).ToList();

                return View("~/Views/Cart/Index.cshtml", cartItems);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Sepet sunucusuna şu anda ulaşılamıyor. Lütfen daha sonra tekrar deneyin.";

                return View(
                    "~/Views/Cart/Index.cshtml",
                    new List<CartItemViewModel>()
                );
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage =
                    "Sepet yüklenirken beklenmeyen bir hata oluştu.";

                return View(
                    "~/Views/Cart/Index.cshtml",
                    new List<CartItemViewModel>()
                );
            }
        }

        [HttpGet("Count")]
        public async Task<IActionResult> Count()
        {
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;

            if (!int.TryParse(customerIdClaim, out int customerId))
            {
                return Unauthorized();
            }

            var client = _httpClientFactory.CreateClient("CustomerApi");

            var carts = await client.GetFromJsonAsync<List<Cart>>("Cart");

            var totalQuantity = carts?
                .Where(c => c.CustomerId == customerId)
                .Sum(c => c.Quantity) ?? 0;

            return Ok(totalQuantity);
        }

        // Ürünü sepete ekle
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartModel model)
        {
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;

            if (!int.TryParse(customerIdClaim, out int customerId))
            {
                return Unauthorized();
            }

            var client = _httpClientFactory.CreateClient("CustomerApi");

            // Mevcut sepetleri getir
            var carts = await client.GetFromJsonAsync<List<Cart>>("Cart");

            // Aynı müşterinin aynı ürünü sepette var mı?
            var existingCart = carts?
                .FirstOrDefault(c =>
                    c.CustomerId == customerId &&
                    c.ProductId == model.ProductId);

            if (existingCart != null)
            {
                // Varsa adetini artır
                existingCart.Quantity++;

                var updateResponse = await client.PutAsJsonAsync(
                    $"Cart/{existingCart.Id}",
                    existingCart
                );

                if (!updateResponse.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }
            else
            {
                // Yoksa yeni ürün ekle
                var cart = new Cart
                {
                    CustomerId = customerId,
                    ProductId = model.ProductId,
                    Quantity = 1
                };

                var response = await client.PostAsJsonAsync("Cart", cart);

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }

            return Ok();
        }
        [HttpPost("UpdateQuantity/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int id, int change)
        {
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;

            if (!int.TryParse(customerIdClaim, out int customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("CustomerApi");

            var carts = await client.GetFromJsonAsync<List<Cart>>("Cart");

            var cart = carts?
                .FirstOrDefault(c => c.Id == id && c.CustomerId == customerId);

            if (cart != null)
            {
                cart.Quantity += change;

                if (cart.Quantity <= 0)
                {
                    await client.DeleteAsync($"Cart/{id}");
                }
                else
                {
                    await client.PutAsJsonAsync($"Cart/{id}", cart);
                }
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost("RemoveQuantity/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveQuantity(int id, int quantityToRemove)
        {
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;

            if (!int.TryParse(customerIdClaim, out int customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("CustomerApi");

            var carts = await client.GetFromJsonAsync<List<Cart>>("Cart");

            var cart = carts?
                .FirstOrDefault(c => c.Id == id && c.CustomerId == customerId);

            if (cart != null)
            {
                cart.Quantity -= quantityToRemove;

                if (cart.Quantity <= 0)
                {
                    await client.DeleteAsync($"Cart/{id}");
                }
                else
                {
                    await client.PutAsJsonAsync($"Cart/{id}", cart);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;

            if (!int.TryParse(customerIdClaim, out int customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("CustomerApi");

            var carts = await client.GetFromJsonAsync<List<Cart>>("Cart");
            var products = await client.GetFromJsonAsync<List<Product>>("Product");

            var userCart = carts?
                .Where(c => c.CustomerId == customerId)
                .ToList() ?? new List<Cart>();

            var checkoutItems = userCart.Select(cart =>
            {
                var product = products?
                    .FirstOrDefault(p => p.Id == cart.ProductId);

                return new CartItemViewModel
                {
                    CartId = cart.Id,
                    ProductId = cart.ProductId,
                    Quantity = cart.Quantity,
                    ProductName = product?.Name ?? "Ürün bulunamadı",
                    Price = product?.Price ?? 0
                };
            }).ToList();

            return View("~/Views/Cart/Checkout.cshtml", checkoutItems);
        }

        [HttpPost("CompletePayment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletePayment()
        {
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;

            if (!int.TryParse(customerIdClaim, out int customerId))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("CustomerApi");

            var carts = await client.GetFromJsonAsync<List<Cart>>("Cart");
            var products = await client.GetFromJsonAsync<List<Product>>("Product");

            var userCart = carts?
                .Where(c => c.CustomerId == customerId)
                .ToList() ?? new List<Cart>();

            if (userCart.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 0
            };

            foreach (var cart in userCart)
            {
                var product = products?
                    .FirstOrDefault(p => p.Id == cart.ProductId);

                if (product != null)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name ?? "Ürün",
                        Price = product.Price,
                        Quantity = cart.Quantity
                    };

                    order.OrderItems.Add(orderItem);

                    order.TotalAmount += product.Price * cart.Quantity;
                }
            }

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            // Sipariş oluşturulduktan sonra sepeti temizle
            foreach (var cart in userCart)
            {
                await client.DeleteAsync($"Cart/{cart.Id}");
            }

            return RedirectToAction(nameof(PaymentSuccess));
        }

        [HttpGet("PaymentSuccess")]
        public IActionResult PaymentSuccess()
        {
            return View("~/Views/Cart/PaymentSuccess.cshtml");
        }
        public class AddToCartModel
        {
            public int ProductId { get; set; }
        }
    }
}