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
        if (userId == null)
            return RedirectToAction("Login", "Account");

        var user = await _context.Users.FindAsync(userId.Value);
        if (user == null)
            return RedirectToAction("Login", "Account");

        var favorites = await _context.FavoriteAlcos
            .Include(f => f.Alcohol)
            .Where(f => f.UserId == userId.Value)
            .Select(f => f.Alcohol)
            .ToListAsync();

        var reviews = await _context.Reviews
            .Include(r => r.Alcohol)
            .Where(r => r.UserId == userId.Value)
            .ToListAsync();

        var model = new AccountSettingsViewModel
        {
            User = user,
            FavoriteAlcohols = favorites,
            UserReviews = reviews
        };

        return View(model);
    }

    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return RedirectToAction("Login", "Account");

        var hasher = new PasswordHasher<User>();

        // ✅ Sprawdź czy stare hasło pasuje do hasha
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);
        if (result == PasswordVerificationResult.Failed)
        {
            TempData["PasswordChangeError"] = "Stare hasło jest niepoprawne.";
            return View(model);
        }

        // ✅ Sprawdź czy nowe hasła się zgadzają
        if (model.NewPassword != model.ConfirmPassword)
        {
            TempData["PasswordChangeError"] = "Nowe hasła nie są takie same.";
            return View(model);
        }

        // ✅ Zapisz nowy hash hasła
        user.PasswordHash = hasher.HashPassword(user, model.NewPassword);
        await _context.SaveChangesAsync();

        TempData["PasswordChangeSuccess"] = "Hasło zostało zmienione.";
        return RedirectToAction("Index");
    }


}
