using Microsoft.AspNetCore.Mvc;
using BookStore.Services;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var cartItems = await _cartService.GetCartItemsAsync();
                return Ok(new { success = true, data = cartItems });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartRequest model)
        {
            try
            {
                await _cartService.AddToCartAsync(model.BookId, model.Quantity);
                return Ok(new { success = true, message = "Added to cart." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveCartRequest model)
        {
            try
            {
                await _cartService.RemoveFromCartAsync(model.CartId);
                return Ok(new { success = true, message = "Removed from cart." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class CartRequest
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }

    public class RemoveCartRequest
    {
        public int CartId { get; set; }
    }
}