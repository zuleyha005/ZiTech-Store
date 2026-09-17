using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using ECommerceMVC.Services;

namespace ECommerceMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Customer")]
    public class CustomerController : Controller
    {
        private readonly JwtHttpClient _jwtHttpClient;

        public CustomerController(JwtHttpClient jwtHttpClient)
        {
            _jwtHttpClient = jwtHttpClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var client = _jwtHttpClient.CreateClient();
            var customers = await client.GetFromJsonAsync<List<Customer>>("Customer");

            return View("~/Views/Admin/Customer/Index.cshtml", customers);
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Customer/Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var client = _jwtHttpClient.CreateClient();
                await client.PostAsJsonAsync("Customer", customer);

                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _jwtHttpClient.CreateClient();

            await client.DeleteAsync($"Customer/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}