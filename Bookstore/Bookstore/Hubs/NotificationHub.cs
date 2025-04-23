using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BookStore.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendOrderNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}