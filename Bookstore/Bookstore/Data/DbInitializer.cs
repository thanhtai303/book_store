using BookStore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Username = "admin",
                        PasswordHash = HashPassword("admin123"),
                        Email = "tai.nguyen.cit20@eiu.edu.vn",
                        CreatedDate = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "nam",
                        PasswordHash = HashPassword("nam123"),
                        Email = "user@user.com",
                        CreatedDate = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}