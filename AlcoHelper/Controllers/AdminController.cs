using AlcoHelper.Data;
using AlcoHelper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            return View("AccessDenied");
        }

        var users = _context.Users.ToList(); // zakładając, że masz DbSet<Users>

        return View(users);
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

    // Formularz edycji
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var alcohol = await _context.Alcohols.FindAsync(id);
        if (alcohol == null) return NotFound();

        return View(alcohol);
    }

    // Akcja edycji
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Alcohol alcohol)
    {
        if (id != alcohol.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(alcohol);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlcoholExists(alcohol.Id))
                    return NotFound();
                throw;
            }
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

    private bool AlcoholExists(int id)
    {
        return _context.Alcohols.Any(e => e.Id == id);
    }

}