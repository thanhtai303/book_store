using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _context.Books.ToListAsync();
            return Ok(new { success = true, data = books });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(new { success = false, message = "Book not found." });
            }
            return Ok(new { success = true, data = book });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Ok(new { success = true, data = new List<object>() });
            }

            var books = await _context.Books
                .Where(b => b.Title.Contains(term) || b.Author.Contains(term))
                .Select(b => new { b.BookId, b.Title })
                .Take(5)
                .ToListAsync();

            return Ok(new { success = true, data = books });
        }
    }
}