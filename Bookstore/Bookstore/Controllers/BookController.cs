using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Include(b => b.Category).ToListAsync();
            return View(books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books.Include(b => b.Category).FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null) return NotFound();
            return View(book);
        }

        public async Task<IActionResult> Browse(int? categoryId)
        {
            var books = categoryId.HasValue
                ? await _context.Books.Where(b => b.CategoryId == categoryId).Include(b => b.Category).ToListAsync()
                : await _context.Books.Include(b => b.Category).ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(books);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var books = await _context.Books
                .Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm))
                .Include(b => b.Category)
                .ToListAsync();
            return View("Index", books);
        }
    }
}