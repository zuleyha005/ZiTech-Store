using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Category")]
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Kategorileri listele
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("CustomerApi");

            var categories = await client.GetFromJsonAsync<List<Category>>("Category");

            return View("~/Views/Admin/Category/Index.cshtml", categories);
        }

        // Kategori ekleme sayfasını aç

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Category/Create.cshtml");
        }

        // Yeni kategoriyi API'ye gönder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("CustomerApi");

                await client.PostAsJsonAsync("Category", category);

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // Kategori düzenleme sayfasını aç
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("CustomerApi");

            var category = await client.GetFromJsonAsync<Category>($"Category/{id}");

            if (category == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Category/Edit.cshtml", category);
        }

        // Düzenlenen kategoriyi API'ye gönder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("CustomerApi");

                await client.PutAsJsonAsync($"Category/{category.Id}", category);

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // Kategori sil
        [HttpPost("Delete{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("CustomerApi");

            await client.DeleteAsync($"Category/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}