using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Bookstore.Controllers;

public class HomeController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        // Get featured books (newest 10 books)
        var featuredBooks = await context.Books
            .Include(b => b.Category)
            .OrderByDescending(b => b.CreatedDate)
            .Take(10)
            .ToListAsync();

        // Get bestseller books (assuming we'll sort by some criteria, here using price as example)
        var bestsellerBooks = await context.Books
            .Include(b => b.Category)
            .OrderByDescending(b => b.Price)
            .Take(8)
            .ToListAsync();

        // Get new arrivals (different set of newest books)
        var newArrivals = await context.Books
            .Include(b => b.Category)
            .OrderByDescending(b => b.CreatedDate)
            .Skip(10) // Skip the ones already shown in featured
            .Take(10)
            .ToListAsync();

        // Get most viewed books (using a different sort criteria for demonstration)
        var mostViewedBooks = await context.Books
            .Include(b => b.Category)
            .OrderBy(b => b.Title)
            .Take(10)
            .ToListAsync();

        ViewData["FeaturedBooks"] = featuredBooks;
        ViewData["BestsellerBooks"] = bestsellerBooks;
        ViewData["NewArrivals"] = newArrivals;
        ViewData["MostViewedBooks"] = mostViewedBooks;

        return View();
    }

    public async Task<IActionResult> Shop(
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string sortOrder = "",
        int pageNumber = 1,
        int pageSize = 9)
    {
        // Start with all books
        var booksQuery = context.Books
            .Include(b => b.Category)
            .AsQueryable();

        // Filter by category if specified
        if (categoryId.HasValue)
        {
            booksQuery = booksQuery.Where(b => b.CategoryId == categoryId.Value);
        }

        // Filter by price range if specified
        if (minPrice.HasValue)
        {
            booksQuery = booksQuery.Where(b => b.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            booksQuery = booksQuery.Where(b => b.Price <= maxPrice.Value);
        }

        // Apply sorting
        booksQuery = sortOrder switch
        {
            "name_asc" => booksQuery.OrderBy(b => b.Title),
            "name_desc" => booksQuery.OrderByDescending(b => b.Title),
            "price_asc" => booksQuery.OrderBy(b => b.Price),
            "price_desc" => booksQuery.OrderByDescending(b => b.Price),
            _ => booksQuery.OrderBy(b => b.Title)
        };

        // Get the total count for pagination
        var totalItems = await booksQuery.CountAsync();

        // Calculate pagination values
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        pageNumber = Math.Max(1, Math.Min(pageNumber, Math.Max(1, totalPages)));

        // Get paged data
        var books = await booksQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Get all categories for the sidebar
        var categories = await context.Categories
            .Select(c => new
            {
                c.CategoryId,
                c.CategoryName,
                BookCount = context.Books.Count(b => b.CategoryId == c.CategoryId)
            })
            .ToListAsync();

        // Get min and max prices for price slider
        var prices = await context.Books
            .Select(b => b.Price)
            .ToListAsync();

        var priceRange = new
        {
            Min = prices.Min(),
            Max = prices.Max()
        };

        // Set up view models and view data
        ViewData["CategoryId"] = categoryId;
        ViewData["MinPrice"] = minPrice ?? priceRange.Min;
        ViewData["MaxPrice"] = maxPrice ?? priceRange.Max;
        ViewData["PriceRangeMin"] = priceRange.Min;
        ViewData["PriceRangeMax"] = priceRange.Max;
        ViewData["SortOrder"] = sortOrder;
        ViewData["PageSize"] = pageSize;
        ViewData["PageNumber"] = pageNumber;
        ViewData["TotalPages"] = totalPages;
        ViewData["TotalItems"] = totalItems;
        ViewData["Categories"] = categories;

        return View(books);
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks(
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string sortOrder = "",
        int pageNumber = 1,
        int pageSize = 9)
    {
        // Start with all books
        var booksQuery = context.Books
            .Include(b => b.Category)
            .AsQueryable();

        // Filter by category if specified
        if (categoryId.HasValue)
        {
            booksQuery = booksQuery.Where(b => b.CategoryId == categoryId.Value);
        }

        // Filter by price range if specified
        if (minPrice.HasValue)
        {
            booksQuery = booksQuery.Where(b => b.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            booksQuery = booksQuery.Where(b => b.Price <= maxPrice.Value);
        }

        // Apply sorting
        booksQuery = sortOrder switch
        {
            "name_asc" => booksQuery.OrderBy(b => b.Title),
            "name_desc" => booksQuery.OrderByDescending(b => b.Title),
            "price_asc" => booksQuery.OrderBy(b => b.Price),
            "price_desc" => booksQuery.OrderByDescending(b => b.Price),
            _ => booksQuery.OrderBy(b => b.Title)
        };

        // Get the total count for pagination
        var totalItems = await booksQuery.CountAsync();

        // Calculate pagination values
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        pageNumber = Math.Max(1, Math.Min(pageNumber, Math.Max(1, totalPages)));

        // Get paged data
        var books = await booksQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Set up view data for the partial view
        ViewData["PageNumber"] = pageNumber;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalPages"] = totalPages;
        ViewData["TotalItems"] = totalItems;
        ViewData["SortOrder"] = sortOrder;

        return PartialView("_BookList", books);
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult LoginRegister()
    {
        return View();
    }

    public IActionResult ProductDetails(int id)
    {
        var book = context.Books
            .Include(b => b.Category)
            .FirstOrDefault(b => b.BookId == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    public IActionResult Cart()
    {
        return View();
    }

    public IActionResult Checkout()
    {
        return View();
    }

    public IActionResult MyAccount()
    {
        return View();
    }
    
    public IActionResult Admin()
    {
        return View();
    }
}