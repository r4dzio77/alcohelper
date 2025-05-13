using Microsoft.AspNetCore.Mvc;
using AlcoHelper.Models;
using System.Collections.Generic;
using AlcoHelper.Data;
using AlcoHelper.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace AlcoHelper.Controllers
{
    public class AlcoholController : Controller
    {
        private readonly AlcoHelperContext _context;

        public AlcoholController(AlcoHelperContext context)
        {
            _context = context;
        }

        // Akcja dla formularza dodawania alkoholu
        public async Task<IActionResult> Add()
        {
            var tags = _context.Tags.ToList(); // Pobierz wszystkie tagi z bazy danych
            ViewBag.Tags = tags; // Przekaż tagi do widoku Add.cshtml

            // Pobieranie krajów z zewnętrznego API
            var countries = await GetCountries();
            ViewBag.Countries = countries; // Przekaż kraje do widoku Add.cshtml

            return View();
        }

        // Metoda do pobierania krajów z API
        public async Task<List<string>> GetCountries()
        {
            var client = new HttpClient();
            var response = await client.GetStringAsync("https://restcountries.com/v3.1/all");
            var countries = JsonConvert.DeserializeObject<List<Country>>(response);

            return countries.Select(c => c.Name.Common).ToList();
        }

        public class Country
        {
            public Name Name { get; set; }
        }

        public class Name
        {
            public string Common { get; set; }
        }

        [HttpPost]
        public IActionResult Add(AddAlcoholViewModel model, IFormFile ImageUrl)
        {
            var userName = HttpContext.Session.GetString("UserName");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.UserName = userName;
            ViewBag.Role = role;

            if (ModelState.IsValid)
            {
                string imageFilePath = null;

                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = Path.GetFileName(ImageUrl.FileName);
                    var fullPath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        ImageUrl.CopyTo(stream);
                    }

                    // Zapisujemy tylko ścieżkę względną
                    imageFilePath = "/uploads/" + fileName;
                }

                // Tworzenie obiektu Alcohol
                var alcohol = new Alcohol
                {
                    Name = model.Name,
                    Type = model.Type,
                    Country = model.Country,
                    AlcoholPercentage = model.AlcoholPercentage,
                    Description = model.Description,
                    ImageUrl = imageFilePath, // Ścieżka do pliku obrazu
                    AddedDate = DateTime.Now,
                    IsApproved = false // Na starcie niezatwierdzony
                };

                // Dodanie alkoholu do bazy danych
                _context.Alcohols.Add(alcohol);
                _context.SaveChanges();

                // Zapisz wybrane tagi do tabeli pośredniczącej AlcoholTag
                foreach (var tagId in model.TagIds)
                {
                    var alcoholTag = new AlcoholTag
                    {
                        AlcoholId = alcohol.Id,
                        TagId = tagId
                    };
                    _context.AlcoholTags.Add(alcoholTag);
                }

                // Zapisz zmiany
                _context.SaveChanges();

                TempData["Message"] = "Alkohol dodany! Czeka na zatwierdzenie przez admina.";
                return RedirectToAction("Index", "Home");
            }

            return View(model); // Wróć do widoku z błędami, jeśli coś nie jest w porządku
        }

        public IActionResult PendingApproval()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;

            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                return View("AccessDenied");
            }

            var pendingAlcohols = _context.Alcohols
                .Where(a => a.IsApproved == false)
                .ToList();

            return View(pendingAlcohols);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            var alcohol = _context.Alcohols.FirstOrDefault(a => a.Id == id);

            if (alcohol == null)
            {
                return NotFound();
            }

            alcohol.IsApproved = true;  // Zatwierdzamy alkohol
            _context.SaveChanges();

            TempData["Message"] = "Alkohol zatwierdzony!";
            return RedirectToAction("PendingApproval");  // Po zatwierdzeniu wracamy do listy oczekujących alkoholi
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var alcohol = _context.Alcohols.FirstOrDefault(a => a.Id == id);

            if (alcohol == null)
            {
                return NotFound();
            }

            _context.Alcohols.Remove(alcohol);  // Usuwamy alkohol z bazy
            _context.SaveChanges();

            TempData["Message"] = "Alkohol odrzucony!";
            return RedirectToAction("PendingApproval");  // Po odrzuceniu wracamy do listy oczekujących alkoholi
        }
    }
}
