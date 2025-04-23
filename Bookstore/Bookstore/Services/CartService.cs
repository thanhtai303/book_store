using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddToCartAsync(int bookId, int quantity)
        {
            var userId = GetUserId();
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.Stock < quantity)
            {
                throw new Exception("Book not available.");
            }

            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    UserId = userId,
                    BookId = bookId,
                    Quantity = quantity
                };
                _context.Carts.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                _context.Carts.Update(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Cart>> GetCartItemsAsync()
        {
            var userId = GetUserId();
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .ToListAsync();
        }

        public async Task ClearCartAsync()
        {
            var userId = GetUserId();
            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        private int GetUserId()
        {
            var userIdStr = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                throw new Exception("User not logged in.");
            }
            return int.Parse(userIdStr);
        }
    }
}