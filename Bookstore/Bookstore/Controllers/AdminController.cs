using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
<<<<<<< Updated upstream
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
=======
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
>>>>>>> Stashed changes
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> ManageBooks()
        {
            var books = await _context.Books.Include(b => b.Category).ToListAsync();
            return View(books);
        }

        [HttpGet]
        public IActionResult CreateBook()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageBooks");
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageBooks");
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageBooks");
        }
    }
}