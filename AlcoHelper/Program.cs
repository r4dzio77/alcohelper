using AlcoHelper.Controllers;
using AlcoHelper.Data;
using Microsoft.EntityFrameworkCore;

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

app.Run();
