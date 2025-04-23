using BookStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(int bookId, int quantity);
        Task RemoveFromCartAsync(int cartId);
        Task<List<Cart>> GetCartItemsAsync();
        Task ClearCartAsync();
    }
}