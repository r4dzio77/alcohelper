using AlcoHelper.Controllers;
using AlcoHelper.Data;
using AlcoHelper.Services;
using Microsoft.EntityFrameworkCore;
using AlcoHelper.Models; // potrzebne do dostępu do User, Role
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Dodanie pamięci podręcznej i sesji
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

// 🔹 Uwierzytelnianie na ciasteczkach
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<EmailService>();

// 🔹 Rejestracja MVC i API
builder.Services.AddControllersWithViews();
builder.Services.AddControllers(); // ← to dodaje obsługę API

// 🔹 Rejestracja kontrolerów i usług
builder.Services.AddScoped<AlcoholController>();
builder.Services.AddScoped<PDFGenerator>();

// 🔹 Rejestracja DbContext z SQLite
builder.Services.AddDbContext<AlcoHelperContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Konfiguracja CORS – przydatne do udostępnienia API frontendowi
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// 🔹 Obsługa wyjątków i HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAll"); // ← Włączenie CORS
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Routing dla MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 🔹 Routing dla API
app.MapControllers();

// 🔹 SEED: dodanie użytkowników i ról
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AlcoHelperContext>();
    var hasher = new PasswordHasher<User>();

    // Dodaj role, jeśli nie istnieją
    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" }
        );
        context.SaveChanges();
    }

    // Dodaj admina
    if (!context.Users.Any(u => u.Email == "admin@alco.pl"))
    {
        var admin = new User
        {
            Username = "Admin",
            Email = "admin@alco.pl",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow
        };
        admin.PasswordHash = hasher.HashPassword(admin, "admin123");
        context.Users.Add(admin);
    }

    // Dodaj użytkownika
    if (!context.Users.Any(u => u.Email == "user@alco.pl"))
    {
        var user = new User
        {
            Username = "User",
            Email = "user@alco.pl",
            RoleId = 2,
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = hasher.HashPassword(user, "user123");
        context.Users.Add(user);
    }

    context.SaveChanges();
}

app.Run();
