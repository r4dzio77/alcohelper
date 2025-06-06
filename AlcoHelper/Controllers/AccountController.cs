using AlcoHelper.Models;
using Microsoft.AspNetCore.Mvc;
using AlcoHelper.Data;
using AlcoHelper.ViewModels;
using AlcoHelper.Services;



public class AccountController : Controller
{
    private readonly AlcoHelperContext _context;
    private readonly EmailService _emailService;
    private static List<PasswordResetToken> _resetTokens = new(); // tymczasowo w pamięci

    public AccountController(AlcoHelperContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Taki użytkownik już istnieje");
                return View(model);
            }

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Ten email jest już zarejestrowany");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = model.Password, // Uwaga: powinieneś hashować hasło!
                CreatedAt = DateTime.Now,
                RoleId = 2
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == model.Email);

            if (user == null || user.PasswordHash != model.Password)
            {
                ModelState.AddModelError(string.Empty, "Błędny email lub hasło.");
                return View(model); // Zwracamy model, aby zachować wprowadzone dane
            }

            var roleName = _context.Roles
                .Where(r => r.Id == user.RoleId)
                .Select(r => r.Name)
                .FirstOrDefault() ?? "Unknown";

            HttpContext.Session.SetString("UserName", user.Username);
            HttpContext.Session.SetString("Role", roleName);
            HttpContext.Session.SetInt32("UserId", user.Id);
            Console.WriteLine($"[DEBUG] Session 'Role' ustawione na: {roleName}");
            Console.WriteLine($"[DEBUG] user.RoleId = {user.RoleId}");
            return RedirectToAction("Index", "Home");
        }

        // Jeśli ModelState.IsValid == false, zwracamy widok z błędami walidacji
        return View(model);
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }


    [HttpPost]
    public IActionResult ForgotPassword(string email)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == email);
        if (user == null)
            return View("ForgotPasswordConfirmation");

        var token = Guid.NewGuid().ToString();
        _resetTokens.Add(new PasswordResetToken
        {
            Token = token,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        });

        var resetLink = Url.Action("ResetPassword", "Account", new { token }, Request.Scheme);
        var htmlBody = $@"
<p>Cześć!</p>
<p>Otrzymaliśmy prośbę o zresetowanie hasła do Twojego konta w serwisie <strong>AlcoHelper</strong>.</p>
<p>Aby ustawić nowe hasło, kliknij poniższy przycisk:</p>
<p>
    <a href='{resetLink}' style='
        display: inline-block;
        padding: 10px 20px;
        background-color: #007bff;
        color: white;
        text-decoration: none;
        border-radius: 5px;
        font-weight: bold;
    '>Zresetuj hasło</a>
</p>
<p>Jeśli nie prosiłeś o reset hasła, zignoruj tę wiadomość. Link wygaśnie za 30 minut.</p>
<hr />
<p style='font-size: 0.8em; color: gray;'>Wiadomość wygenerowana automatycznie – prosimy na nią nie odpowiadać.</p>";

        _emailService.Send(user.Email, "[AlcoHelper] Prośba o zresetowanie hasła", htmlBody);


        return View("ForgotPasswordConfirmation");
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        var entry = _resetTokens.SingleOrDefault(t => t.Token == token && t.ExpiresAt > DateTime.UtcNow);
        if (entry == null)
            return View("ResetPasswordInvalid");

        return View(new ResetPasswordViewModel { Token = token });
    }

    [HttpPost]
    public IActionResult ResetPassword(ResetPasswordViewModel model)
    {
        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError("", "Hasła nie są takie same.");
            return View(model);
        }

        var tokenEntry = _resetTokens.SingleOrDefault(t => t.Token == model.Token && t.ExpiresAt > DateTime.UtcNow);
        if (tokenEntry == null)
            return View("ResetPasswordInvalid");

        var user = _context.Users.Find(tokenEntry.UserId);
        user.PasswordHash = model.NewPassword; // TODO: zhashuj
        _context.SaveChanges();

        _resetTokens.Remove(tokenEntry);

        return View("ResetPasswordSuccess");
    }


    public IActionResult Logout()
    {
        HttpContext.Session.Remove("UserName"); // Usuwamy dane użytkownika z sesji
        HttpContext.Session.Remove("Role");
        return RedirectToAction("Index", "Home");
    }
    public IActionResult AccessDenied()
    {
        return View();  // Upewnij się, że masz widok AccessDenied.cshtml
    }
}

