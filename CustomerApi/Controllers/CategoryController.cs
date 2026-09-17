using Microsoft.AspNetCore.Mvc;
using CustomerApi.Data;
using Microsoft.EntityFrameworkCore;
using CustomerApi.Models;

namespace CustomerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CustomerDbContext _context;

        public CategoryController(CustomerDbContext context)
        {
            _context = context;
        }

        // Kategorileri getir
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _context.Categories.ToList();

            return Ok(categories);
        }

        // Kategori ekle
        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();

            return Ok(category);
        }

        // Kategori güncelle
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, Category updatedCategory)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            category.Name = updatedCategory.Name;

            _context.SaveChanges();

            return Ok(category);
        }

        // Kategori sil
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok(category);
        }
    }
}