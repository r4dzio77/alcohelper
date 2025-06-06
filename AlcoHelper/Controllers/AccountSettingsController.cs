using AlcoHelper.Data;
using AlcoHelper.Models;
using AlcoHelper.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class AccountSettingsController : Controller
{
    private readonly AlcoHelperContext _context;

    public AccountSettingsController(AlcoHelperContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        Console.WriteLine($"[DEBUG] UserId z sesji: {userId}");

        if (userId == null)
        {
            Console.WriteLine("[DEBUG] UserId jest null. Przekierowanie do logowania.");
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users.FindAsync(userId.Value);
        if (user == null)
        {
            Console.WriteLine($"[DEBUG] Nie znaleziono użytkownika o Id={userId}. Przekierowanie do logowania.");
            return RedirectToAction("Login", "Account");
        }
        Console.WriteLine($"[DEBUG] Użytkownik znaleziony: {user.Username} (Id: {user.Id})");

        var favorites = await _context.FavoriteAlcos
            .Include(f => f.Alcohol)
            .Where(f => f.UserId == userId.Value)
            .Select(f => f.Alcohol)
            .ToListAsync();
        Console.WriteLine($"[DEBUG] Znaleziono {favorites.Count} ulubionych alkoholi.");

        var reviews = await _context.Reviews
            .Include(r => r.Alcohol)
            .Where(r => r.UserId == userId.Value)
            .ToListAsync();
        Console.WriteLine($"[DEBUG] Znaleziono {reviews.Count} recenzji użytkownika.");

        var model = new AccountSettingsViewModel
        {
            User = user,
            FavoriteAlcohols = favorites,
            UserReviews = reviews
        };

        Console.WriteLine("[DEBUG] Przygotowano model dla widoku AccountSettings/Index.");

        return View(model);
    }


    // POST: /AccountSettings/ChangePassword
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return RedirectToAction("Index");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return RedirectToAction("Login", "Account");

        var hasher = new PasswordHasher<User>();

        var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            TempData["PasswordChangeError"] = "Stare hasło jest niepoprawne.";
            return RedirectToAction("Index");
        }

        if (model.NewPassword != model.ConfirmPassword)
        {
            TempData["PasswordChangeError"] = "Nowe hasła nie są takie same.";
            return RedirectToAction("Index");
        }

        user.PasswordHash = hasher.HashPassword(user, model.NewPassword);
        await _context.SaveChangesAsync();

        TempData["PasswordChangeSuccess"] = "Hasło zostało zmienione.";
        return RedirectToAction("Index");
    }

}
