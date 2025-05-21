using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutConfirmed()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    BookId = ci.BookId,
                    Quantity = ci.Quantity,
                    Price = ci.Book.Price
                }).ToList(),
                Total = cart.CartItems.Sum(ci => ci.Quantity * ci.Book.Price)
            };

            foreach (var item in cart.CartItems)
            {
                var book = await _context.Books.FindAsync(item.BookId);
                if (book != null)
                {
                    book.Stock -= item.Quantity;
                }
            }

            _context.Orders.Add(order);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Book");
        }
    }
}