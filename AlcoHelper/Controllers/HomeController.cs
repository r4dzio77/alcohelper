using Microsoft.AspNetCore.Mvc;
using AlcoHelper.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AlcoHelper.Data;

namespace AlcoHelper.Controllers
{
    public class HomeController : Controller
    {
        private readonly AlcoHelperContext _context;

        public HomeController(AlcoHelperContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchTerm)
        {
            var userName = HttpContext.Session.GetString("UserName");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.UserName = userName;
            ViewBag.Role = role;

            ViewBag.SearchTerm = searchTerm;
            // Pobieranie alkoholi z powi¹zanymi recenzjami i tagami
            var approvedAlcohols = _context.Alcohols
                .Include(a => a.Reviews)            // Za³adowanie recenzji alkoholu
                .Include(a => a.AlcoholTags)        // Za³adowanie tagów
                .ThenInclude(at => at.Tag)          // Za³adowanie samego tagu
                .Where(a => a.IsApproved)           // Tylko zatwierdzone alkohole
                .AsQueryable();

            // Filtrujemy alkohole na podstawie nazwy, jeœli u¿ytkownik poda³ zapytanie w formularzu
            if (!string.IsNullOrEmpty(searchTerm))
            {
                approvedAlcohols = approvedAlcohols.Where(a => a.Name.Contains(searchTerm));
            }

            // Pobranie listy alkoholi po zastosowaniu filtrów
            var alcoholList = approvedAlcohols.ToList();

            // Przekazanie danych do widoku
            ViewBag.AlcoholData = alcoholList.Select(alcohol => new
            {
                alcohol.Id,
                alcohol.Name,
                alcohol.Type,
                alcohol.Country,
                alcohol.AlcoholPercentage,
                alcohol.Description,
                alcohol.ImageUrl,
                Reviews = alcohol.Reviews?.Select(r => new
                {
                    Rating = r.Rating,
                    Comment = r.Comment ?? "Brak komentarza",
                    CreatedAt = r.CreatedAt
                }).ToList(),
                Tags = alcohol.AlcoholTags.Select(at => at.Tag.Name).ToList() // Pobranie nazw tagów
            }).ToList();

            return View(alcoholList);  // Przekazanie alkoholi z recenzjami i tagami do widoku
        }
    }
}
