using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using BookStore.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using BookStore.Hubs;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public OrderController(ApplicationDbContext context, ICartService cartService, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _cartService = cartService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                var orders = await _context.OrderHistories
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                    .ToListAsync();
                return Ok(new { success = true, data = orders });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest model)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                var cartItems = await _cartService.GetCartItemsAsync();

                if (!cartItems.Any())
                {
                    return BadRequest(new { success = false, message = "Cart is empty." });
                }

                var order = new OrderHistory
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = cartItems.Sum(c => c.Quantity * c.Book.Price),
                    OrderDetails = cartItems.Select(c => new OrderDetail
                    {
                        BookId = c.BookId,
                        Quantity = c.Quantity,
                        Price = c.Book.Price
                    }).ToList()
                };

                foreach (var item in cartItems)
                {
                    var book = await _context.Books.FindAsync(item.BookId);
                    if (book.Stock < item.Quantity)
                    {
                        return BadRequest(new { success = false, message = $"Not enough stock for {book.Title}." });
                    }
                    book.Stock -= item.Quantity;
                }

                _context.OrderHistories.Add(order);
                await _cartService.ClearCartAsync();
                await _context.SaveChangesAsync();

                await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new { message = $"Order #{order.OrderId} placed successfully!" });

                return Ok(new { success = true, orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class CheckoutRequest
    {
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
    }
}
