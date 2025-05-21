using BookStore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Users if none exist
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

            // Seed Categories if none exist
            if (!context.Categories.Any())
            {
                var categoryNames = new List<string>
                {
                    "Fiction",
                    "Non-fiction",
                    "Science",
                    "History",
                    "Computer Science",
                    "Biography",
                    "Art",
                    "Philosophy",
                    "Self-help",
                    "Psychology"
                };

                var categoryFaker = new Faker<Category>()
                    .RuleFor(c => c.CategoryName, f => f.PickRandom(categoryNames))
                    .RuleFor(c => c.Description, f => f.Lorem.Sentence(5))
                    .RuleFor(c => c.CreatedDate, f => f.Date.Past(1));

                var categories = categoryFaker.Generate(categoryNames.Count);

                // Ensure each category has a unique name
                categories = categories
                    .GroupBy(c => c.CategoryName)
                    .Select(g => g.First())
                    .ToList();

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Books if none exist
            if (!context.Books.Any())
            {
                var categories = await context.Categories.ToListAsync();

                var bookFaker = new Faker<Book>()
                    .RuleFor(b => b.Title, f => f.Commerce.ProductName())
                    .RuleFor(b => b.Author, f => f.Name.FullName())
                    .RuleFor(b => b.ImgURL, f => f.Image.PicsumUrl(200, 300))
                    .RuleFor(b => b.Price, f => Math.Round(f.Random.Decimal(9.99m, 99.99m), 2))
                    .RuleFor(b => b.Stock, f => f.Random.Int(5, 100))
                    .RuleFor(b => b.CreatedDate, f => f.Date.Past(1))
                    // .RuleFor(b => b.ShortDescription, f => f.Lorem.Sentence(10))
                    // .RuleFor(b => b.LongDescription, f => f.Lorem.Paragraph(5))
                    .RuleFor(b => b.CategoryId, f => f.PickRandom(categories).CategoryId);

                var books = bookFaker.Generate(50);
                context.Books.AddRange(books);
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