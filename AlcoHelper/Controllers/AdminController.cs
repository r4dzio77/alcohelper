using AlcoHelper.Data;
using AlcoHelper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlcoHelper.ViewModels;
public class AdminController : Controller
{
    private readonly AlcoHelperContext _context;

    public AdminController(AlcoHelperContext context)
    {
        _context = context;
    }

    // Strona główna panelu administratora
    public IActionResult Dashboard()
    {
        // Pobranie roli użytkownika z sesji
        var role = HttpContext.Session.GetString("Role");
        var userName = HttpContext.Session.GetString("UserName");

        if (string.IsNullOrEmpty(userName) || role != "Admin")
        {
            // Jeśli użytkownik nie jest zalogowany lub nie ma roli Admin, przekieruj na stronę AccessDenied
            return RedirectToAction("AccessDenied", "Account");
        }

        return View();
    }

    [HttpGet]
    public IActionResult ManageUsers()
    {
        var role = HttpContext.Session.GetString("Role");

        if (role != "Admin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var users = _context.Users.ToList(); // zakładając, że masz DbSet<Users>
        var roles = _context.Roles.ToList();

        var model = new UserRolesViewModel {
            Users = users,
            Roles = roles
        };

        return View(model);
    }
    
    [HttpPost]
    public IActionResult UpdateUserRoles(Dictionary<int, int> RolesForUsers)
    {
        var currentUserId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
        
        if (RolesForUsers == null || !RolesForUsers.Any())
        {
            TempData["Error"] = "Brak danych do aktualizacji.";
            return RedirectToAction("ManageUsers");
        }

        foreach (var entry in RolesForUsers)
        {
            var userId = entry.Key;
            var newRoleId = entry.Value;

            if (userId == currentUserId)
            {
                continue;
            }

            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.RoleId = newRoleId;
            }
        }

        _context.SaveChanges();

        TempData["Success"] = "Role użytkowników zostały zaktualizowane.";
        return RedirectToAction("ManageUsers");
    }

    // Lista wszystkich alkoholi
    public async Task<IActionResult> Index()
    {
        return View(await _context.Alcohols.OrderByDescending(a => a.AddedDate).ToListAsync());
    }

    // Formularz dodawania
    public IActionResult Create()
    {
        return View();
    }

    // Akcja dodawania
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Alcohol alcohol)
    {
        if (ModelState.IsValid)
        {
            _context.Add(alcohol);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(alcohol);
    }


    

    // Akcja usuwania
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var alcohol = await _context.Alcohols.FindAsync(id);
        if (alcohol != null)
        {
            _context.Alcohols.Remove(alcohol);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("ManageUsers");
    }

    private bool AlcoholExists(int id)
    {
        return _context.Alcohols.Any(e => e.Id == id);
    }

}