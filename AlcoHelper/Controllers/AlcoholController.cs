using AlcoHelper.Data;
using AlcoHelper.Models;
using AlcoHelper.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace AlcoHelper.Controllers
{
    public class AlcoholController : Controller
    {
        private readonly AlcoHelperContext _context;
        private readonly PDFGenerator _pdfGenerator;

        public AlcoholController(AlcoHelperContext context, PDFGenerator pdfGenerator)
        {
            _context = context;
            _pdfGenerator = pdfGenerator ?? throw new ArgumentNullException(nameof(pdfGenerator));
        }


        // Akcja dla formularza dodawania alkoholu
        public async Task<IActionResult> Add()
        {
            var tags = _context.Tags?
                .OrderBy(t => t.Name)
                .ToList() ?? new List<Tag>();

            var countries = (await GetCountries()) ?? new List<string>();

            ViewBag.Tags = tags;
            ViewBag.Countries = countries;

            return View(new AddAlcoholViewModel());

        }

        // Metoda do pobierania krajów z API
        public async Task<List<string>> GetCountries()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetStringAsync("https://restcountries.com/v3.1/all?fields=name");

                var jArray = JArray.Parse(response);

                var countryNames = jArray
                    .Select(c => c["name"]?["common"]?.ToString())
                    .Where(name => !string.IsNullOrEmpty(name))
                    .OrderBy(name => name)
                    .ToList();

                return countryNames;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching countries: {ex.Message}");
                return new List<string> { "Polska", "Niemcy", "Francja" }; // Fallback list
            }
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
        public IActionResult Add(AddAlcoholViewModel model)
        {
            var userName = HttpContext.Session.GetString("UserName");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.UserName = userName;
            ViewBag.Role = role;

            if (ModelState.IsValid)
            {
                string imageFilePath = null;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = Path.GetFileName(model.ImageFile.FileName);
                    var fullPath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(stream);
                    }

                    // Zapisujemy tylko ścieżkę względną
                    imageFilePath = "/uploads/" + fileName;
                }

                var alcohol = new Alcohol
                {
                    Name = model.Name,
                    Type = model.Type,
                    Country = model.Country,
                    AlcoholPercentage = model.AlcoholPercentage,
                    Description = model.Description,
                    ImageUrl = imageFilePath,
                    AddedDate = DateTime.Now,
                    IsApproved = false
                };

                _context.Alcohols.Add(alcohol);
                _context.SaveChanges();

                // Tagi
                if (model.TagIds != null)
                {
                    foreach (var tagId in model.TagIds)
                    {
                        _context.AlcoholTags.Add(new AlcoholTag
                        {
                            AlcoholId = alcohol.Id,
                            TagId = tagId
                        });
                    }
                }

                _context.SaveChanges();

                TempData["Message"] = "Alkohol dodany! Czeka na zatwierdzenie przez admina.";
                return RedirectToAction("Index", "Home");
            }

            // Przy błędach trzeba załadować dane do ViewBag
            ViewBag.Tags = _context.Tags.OrderBy(t => t.Name).ToList();
            ViewBag.Countries = GetCountries().Result;

            return View(model);
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

        public IActionResult Details(int id)
        {
            var alcohol = _context.Alcohols.Find(id);
            if (alcohol == null)
            {
                return NotFound();
            }

            return View(alcohol);  // Widok szczegółów alkoholu
        }

        // Akcja generująca PDF i zwracająca go do pobrania
        public IActionResult DownloadPDF(int id)
        {
            var alcohol = _context.Alcohols.Find(id);
            if (alcohol == null)
            {
                return NotFound();
            }

            // Generujemy PDF za pomocą _pdfGenerator
            var pdfData = _pdfGenerator.GenerateAlcoholPDF(alcohol);

            // Zwracamy plik PDF do pobrania
            return File(pdfData, "application/pdf", $"{alcohol.Name}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var alcohol = await _context.Alcohols
                .Include(a => a.AlcoholTags)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (alcohol == null)
                return NotFound();

            var model = new AddAlcoholViewModel
            {
                Id = alcohol.Id,
                Name = alcohol.Name,
                Type = alcohol.Type,
                Country = alcohol.Country,
                AlcoholPercentage = alcohol.AlcoholPercentage,
                Description = alcohol.Description,
                ExistingImageUrl = alcohol.ImageUrl,
                TagIds = alcohol.AlcoholTags.Select(at => at.TagId).ToList()
            };

            ViewBag.Countries = await GetCountries();
            ViewBag.Tags = _context.Tags.OrderBy(t => t.Name).ToList();

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddAlcoholViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Countries = await GetCountries();
                ViewBag.Tags = _context.Tags.OrderBy(t => t.Name).ToList();
                return View(model);
            }

            var alcohol = await _context.Alcohols.FindAsync(id);
            if (alcohol == null)
                return NotFound();

            alcohol.Name = model.Name;
            alcohol.Type = model.Type;
            alcohol.Country = model.Country;
            alcohol.AlcoholPercentage = model.AlcoholPercentage;
            alcohol.Description = model.Description;

            // Obsługa zdjęcia
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.Countries = await GetCountries();
                    ViewBag.Tags = _context.Tags.OrderBy(t => t.Name).ToList();
                    ModelState.AddModelError("ImageFile", "Dozwolone tylko pliki: .jpg, .jpeg, .png");
                    return View(model);
                }

                if (!string.IsNullOrEmpty(alcohol.ImageUrl))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", alcohol.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                var fileName = Guid.NewGuid().ToString() + extension;
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fullPath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                alcohol.ImageUrl = "/uploads/" + fileName;
            }
            else
            {
                alcohol.ImageUrl = model.ExistingImageUrl;
            }

            await _context.SaveChangesAsync();

            // Zaktualizuj tagi
            var oldTags = _context.AlcoholTags.Where(at => at.AlcoholId == alcohol.Id);
            _context.AlcoholTags.RemoveRange(oldTags);
            await _context.SaveChangesAsync();

            if (model.TagIds != null)
            {
                foreach (var tagId in model.TagIds)
                {
                    _context.AlcoholTags.Add(new AlcoholTag
                    {
                        AlcoholId = alcohol.Id,
                        TagId = tagId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }




    }
}