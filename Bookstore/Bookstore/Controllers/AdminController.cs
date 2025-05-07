using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    public class AdminViewController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("books")]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                var user = await _context.Users.FindAsync(userId);
                if (user.Username != "admin")
                {
                    return Unauthorized(new { success = false, message = "Admin access required." });
                }

                var books = await _context.Books.ToListAsync();
                return Ok(new { success = true, data = books });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("books")]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                var user = await _context.Users.FindAsync(userId);
                if (user.Username != "admin")
                {
                    return Unauthorized(new { success = false, message = "Admin access required." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }

                book.CreatedDate = DateTime.UtcNow;
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, bookId = book.BookId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("books/{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                var user = await _context.Users.FindAsync(userId);
                if (user.Username != "admin")
                {
                    return Unauthorized(new { success = false, message = "Admin access required." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }

                var existingBook = await _context.Books.FindAsync(id);
                if (existingBook == null)
                {
                    return NotFound(new { success = false, message = "Book not found." });
                }

                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.Price = book.Price;
                existingBook.Stock = book.Stock;
                existingBook.ImgURL = book.ImgURL;
                await _context.SaveChangesAsync();
                return Ok(new { success = true, bookId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("books/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                var user = await _context.Users.FindAsync(userId);
                if (user.Username != "admin")
                {
                    return Unauthorized(new { success = false, message = "Admin access required." });
                }

                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound(new { success = false, message = "Book not found." });
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Book deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}