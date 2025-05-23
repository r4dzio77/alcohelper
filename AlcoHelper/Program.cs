using AlcoHelper.Controllers;
using AlcoHelper.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
// Dodanie sesji
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

// Dodanie uwierzytelniania opartego na ciasteczkach
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";              // �cie�ka do logowania
        options.AccessDeniedPath = "/Account/AccessDenied"; // �cie�ka gdy brak dost�pu
    });

builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AlcoholController>();

// Dodanie DbContext z MySQL
builder.Services.AddDbContext<AlcoHelperContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 42))));

var app = builder.Build();

app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Uwierzytelnianie i autoryzacja � w tej kolejno�ci!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
