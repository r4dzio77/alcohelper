using AlcoHelper.Models;
using Microsoft.AspNetCore.Mvc;
using AlcoHelper.Data;
using AlcoHelper.ViewModels;

public class AccountController : Controller
{
    private readonly AlcoHelperContext _context;

    public AccountController(AlcoHelperContext context)
    {
        _context = context;
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
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            Console.WriteLine($"[DEBUG] Session 'Role' ustawione na: {roleName}");
            Console.WriteLine($"[DEBUG] user.RoleId = {user.RoleId}");
            return RedirectToAction("Index", "Home");
        }

        // Jeśli ModelState.IsValid == false, zwracamy widok z błędami walidacji
        return View(model);
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

