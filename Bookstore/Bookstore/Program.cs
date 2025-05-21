using BookStore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

// Add global authorization filter to require authentication for all controllers except those with [AllowAnonymous]
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Book}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Apply migrations
    context.Database.Migrate();

    // Seed Admin and User roles
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }

    // Seed admin user
    if (await userManager.FindByEmailAsync("admin@bookstore.com") == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin@bookstore.com",
            Email = "admin@bookstore.com"
        };
        await userManager.CreateAsync(adminUser, "Admin@123");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Seed sample user
    if (await userManager.FindByEmailAsync("user@bookstore.com") == null)
    {
        var sampleUsers = new ApplicationUser
        {
            UserName = "user@bookstore.com",
            Email = "user@bookstore.com"
        };
        await userManager.CreateAsync(sampleUsers, "User@123");
        await userManager.AddToRoleAsync(sampleUsers, "User");
    }

    // Seed Categories
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new Category { Name = "Fiction" },
            new Category { Name = "Non-Fiction" },
            new Category { Name = "Science" }
        );
        await context.SaveChangesAsync();
    }

    // Seed Books
    if (!context.Books.Any())
    {
        var fictionCategory = await context.Categories.FirstAsync(c => c.Name == "Fiction");
        var nonFictionCategory = await context.Categories.FirstAsync(c => c.Name == "Non-Fiction");
        var scienceCategory = await context.Categories.FirstAsync(c => c.Name == "Science");

        context.Books.AddRange(
            new Book
            {
                Title = "Pride and Prejudice",
                Author = "Jane Austen",
                YearOfPublication = 1813,
                LongDescription = "A classic novel about love, society, and family in 19th-century England.",
                CategoryId = fictionCategory.CategoryId,
                Stock = 50,
                Price = 9.99m,
                ShortDescription = "A romantic tale of Elizabeth Bennet and Mr. Darcy.",
                Publisher = "Penguin Classics",
                ImageS = "/images/pride_s.jpg",
                ImageM = "/images/pride_m.jpg",
                ImageL = "/images/pride_l.jpg"
            },
            new Book
            {
                Title = "1984",
                Author = "George Orwell",
                YearOfPublication = 1949,
                LongDescription = "A dystopian novel exploring surveillance and totalitarianism.",
                CategoryId = fictionCategory.CategoryId,
                Stock = 30,
                Price = 12.99m,
                ShortDescription = "A chilling vision of a dystopian future.",
                Publisher = "Signet Classics",
                ImageS = "/images/1984_s.jpg",
                ImageM = "/images/1984_m.jpg",
                ImageL = "/images/1984_l.jpg"
            },
            new Book
            {
                Title = "Sapiens: A Brief History of Humankind",
                Author = "Yuval Noah Harari",
                YearOfPublication = 2011,
                LongDescription = "An exploration of human history from the Stone Age to the modern era.",
                CategoryId = nonFictionCategory.CategoryId,
                Stock = 20,
                Price = 15.99m,
                ShortDescription = "A concise history of humanity.",
                Publisher = "Harper",
                ImageS = "/images/sapiens_s.jpg",
                ImageM = "/images/sapiens_m.jpg",
                ImageL = "/images/sapiens_l.jpg"
            },
            new Book
            {
                Title = "A Brief History of Time",
                Author = "Stephen Hawking",
                YearOfPublication = 1988,
                LongDescription = "A landmark book explaining complex cosmological concepts.",
                CategoryId = scienceCategory.CategoryId,
                Stock = 25,
                Price = 14.99m,
                ShortDescription = "An introduction to cosmology for the general reader.",
                Publisher = "Bantam Books",
                ImageS = "/images/time_s.jpg",
                ImageM = "/images/time_m.jpg",
                ImageL = "/images/time_l.jpg"
            },
            new Book
            {
                Title = "The Hobbit",
                Author = "J.R.R. Tolkien",
                YearOfPublication = 1937,
                LongDescription = "A fantasy adventure about Bilbo Baggins and a quest for treasure.",
                CategoryId = fictionCategory.CategoryId,
                Stock = 40,
                Price = 11.99m,
                ShortDescription = "Bilbo's journey with dwarves and a dragon.",
                Publisher = "Houghton Mifflin",
                ImageS = "/images/hobbit_s.jpg",
                ImageM = "/images/hobbit_m.jpg",
                ImageL = "/images/hobbit_l.jpg"
            }
        );
        await context.SaveChangesAsync();
    }

    // Seed Cart and CartItems for sample user
    var sampleUser = await userManager.FindByEmailAsync("user@bookstore.com");
    if (!context.Carts.Any(c => c.UserId == sampleUser.Id))
    {
        var cart = new Cart
        {
            UserId = sampleUser.Id,
            CartItems = new List<CartItem>
            {
                new CartItem
                {
                    BookId = context.Books.First(b => b.Title == "Pride and Prejudice").BookId,
                    Quantity = 2
                },
                new CartItem
                {
                    BookId = context.Books.First(b => b.Title == "Sapiens: A Brief History of Humankind").BookId,
                    Quantity = 1
                }
            }
        };
        context.Carts.Add(cart);
        await context.SaveChangesAsync();
    }

    // Seed Order and OrderItems for sample user
    if (!context.Orders.Any(o => o.UserId == sampleUser.Id))
    {
        var book1 = context.Books.First(b => b.Title == "1984");
        var book2 = context.Books.First(b => b.Title == "The Hobbit");
        var order = new Order
        {
            UserId = sampleUser.Id,
            OrderDate = DateTime.Now.AddDays(-1),
            Total = (2 * book1.Price) + (1 * book2.Price),
            Status = "Completed",
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    BookId = book1.BookId,
                    Quantity = 2,
                    Price = book1.Price
                },
                new OrderItem
                {
                    BookId = book2.BookId,
                    Quantity = 1,
                    Price = book2.Price
                }
            }
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();
    }
}

app.Run();