using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(new { success = true, data = categories });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { success = false, message = "Category not found." });
            }
            return Ok(new { success = true, data = category });
        }

        [HttpGet("{id}/books")]
        public async Task<IActionResult> GetCategoryBooks(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { success = false, message = "Category not found." });
            }

            var books = await _context.Books
                .Where(b => b.CategoryId == id)
                .ToListAsync();

            return Ok(new { success = true, data = books });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            category.CreatedDate = System.DateTime.UtcNow;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId },
                new { success = true, data = category });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest(new { success = false, message = "ID mismatch." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(c => c.CategoryId == id))
                {
                    return NotFound(new { success = false, message = "Category not found." });
                }
                throw;
            }

            return Ok(new { success = true, data = category });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { success = false, message = "Category not found." });
            }

            // Check if category has books
            var hasBooks = await _context.Books.AnyAsync(b => b.CategoryId == id);
            if (hasBooks)
            {
                return BadRequest(new { success = false, message = "Cannot delete category with existing books." });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Category deleted successfully." });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Ok(new { success = true, data = new List<object>() });
            }

            var categories = await _context.Categories
                .Where(c => c.CategoryName.Contains(term) || c.Description.Contains(term))
                .Select(c => new { c.CategoryId, c.CategoryName })
                .Take(5)
                .ToListAsync();

            return Ok(new { success = true, data = categories });
        }
    }
}