using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Cart")]
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("CustomerApi");

            var carts = await client.GetFromJsonAsync<List<Cart>>("Cart");

            return View("~/Views/Admin/Cart/Index.cshtml", carts);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Cart/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cart cart)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("CustomerApi");

                await client.PostAsJsonAsync("Cart", cart);

                return RedirectToAction(nameof(Index));
            }

            return View(cart);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("CustomerApi");

            await client.DeleteAsync($"Cart/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}