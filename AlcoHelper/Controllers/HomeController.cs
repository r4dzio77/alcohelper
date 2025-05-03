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

        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.UserName = userName;
            ViewBag.Role = role;

            // Pobierz pierwszy alkohol z bazy wraz z recenzjami
            var alcohol = _context.Alcohols
                .Include(a => a.Reviews)
                .FirstOrDefault();

            if (alcohol != null)
            {
                // Przygotuj obiekt do przekazania do widoku
                var alcoholData = new
                {
                    Id = alcohol.Id,
                    Name = alcohol.Name ?? "Nieznana nazwa",
                    Type = alcohol.Type ?? "Nieznany typ",
                    Country = alcohol.Country ?? "Nieznany kraj",
                    AlcoholPercentage = alcohol.AlcoholPercentage,
                    Description = alcohol.Description ?? "Brak opisu",
                    ImageUrl = alcohol.ImageUrl,
                    Reviews = alcohol.Reviews?.Select(r => new
                    {
                        Rating = r.Rating,
                        Comment = r.Comment ?? "Brak komentarza",
                        CreatedAt = r.CreatedAt
                    }).ToList()
                };

                ViewBag.AlcoholData = JsonSerializer.Serialize(alcoholData);
            }
            else
            {
                // Jeœli brak alkoholi w bazie, przeka¿ pusty obiekt
                ViewBag.AlcoholData = "null";
            }
            // Pobieranie alkoholi tylko zatwierdzonych
            var approvedAlcohols = _context.Alcohols.Where(a => a.IsApproved).ToList();

            return View(approvedAlcohols);  // Przekazywanie tylko zatwierdzonych alkoholi do widoku
        }
    }
}