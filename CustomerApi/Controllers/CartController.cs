using Microsoft.AspNetCore.Mvc;
using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CustomerDbContext _context;
        public CartController(CustomerDbContext context)
        {
            _context = context;
        }

        // Sepetleri getir
        [HttpGet]
        public IActionResult GetCarts()
        {
            var carts = _context.Carts.ToList();

            return Ok(carts);
        }

        // Sepete ürün ekle
        [HttpPost]
        public IActionResult AddCart(Cart cart)
        {
            _context.Carts.Add(cart);
            _context.SaveChanges();

            return Ok(cart);
        }

        // Sepeti güncelle
        [HttpPut("{id}")]
        public IActionResult UpdateCart(int id, Cart updatedCart)
        {
            var cart = _context.Carts.FirstOrDefault(x => x.Id == id);

            if (cart == null)
            {
                return NotFound();
            }

            cart.CustomerId = updatedCart.CustomerId;
            cart.ProductId = updatedCart.ProductId;
            cart.Quantity = updatedCart.Quantity;

            _context.SaveChanges();

            return Ok(cart);
        }
        // Sepetten ürün sil
        [HttpDelete("{id}")]
        public IActionResult DeleteCart(int id)
        {
            var cart = _context.Carts.FirstOrDefault(x => x.Id == id);

            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            _context.SaveChanges();

            return Ok(cart);
        }
    }
}