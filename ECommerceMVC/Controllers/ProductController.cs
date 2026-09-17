using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Product")]
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Ürünleri listele
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("CustomerApi");

            var products = await client.GetFromJsonAsync<List<Product>>("Product");

           return View("~/Views/Admin/Product/Index.cshtml", products);
        }

        // Ürün ekleme sayfasını aç
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Product/Create.cshtml");
        }

        // Yeni ürünü API'ye gönder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("CustomerApi");

                await client.PostAsJsonAsync("Product", product);

                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Admin/Product/Create.cshtml", product);
        }

        // Ürün düzenleme sayfasını aç
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("CustomerApi");

            var product = await client.GetFromJsonAsync<Product>($"Product/{id}");

            if (product == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Product/Edit.cshtml", product);
        }

        // Düzenlenen ürünü API'ye gönder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("CustomerApi");

                await client.PutAsJsonAsync($"Product/{product.Id}", product);

                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Admin/Product/Edit.cshtml", product);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("CustomerApi");

            await client.DeleteAsync($"Product/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}