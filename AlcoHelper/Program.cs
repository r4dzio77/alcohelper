using AlcoHelper.Controllers;
using AlcoHelper.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dodanie pamięci podręcznej i sesji
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Ustawienie czasu wygaśnięcia sesji
    options.Cookie.HttpOnly = true; // Ustawienie cookie jako HttpOnly
});

// Dodanie uwierzytelniania opartego na ciasteczkach
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";              // Ścieżka do logowania
        options.AccessDeniedPath = "/Account/AccessDenied"; // Ścieżka, gdy brak dostępu
    });

builder.Services.AddAuthorization();

// Rejestracja kontrolera
builder.Services.AddControllersWithViews();

// Rejestracja AlcoholController i innych usług, takich jak PDFGenerator
builder.Services.AddScoped<AlcoholController>();
builder.Services.AddScoped<PDFGenerator>(); // ✅ poprawnie


// Rejestracja DbContext
builder.Services.AddDbContext<AlcoHelperContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Obsługa wyjątków
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// Uwierzytelnianie i autoryzacja – kolejność jest istotna
app.UseAuthentication();
app.UseAuthorization();

// Konfiguracja tras
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
