using AlcoHelper.Models;
using AlcoHelper.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AlcoHelper.Controllers
{
    public class TagController : Controller
    {
        private readonly AlcoHelperContext _context;

        public TagController(AlcoHelperContext context)
        {
            _context = context;
        }

        // Akcja do dodawania tagów
        public IActionResult AddTags()
        {
            // Sprawdzenie, czy tagi już istnieją
            if (!_context.Tags.Any())
            {
                // Dodanie kilku przykładowych tagów
                var tags = new List<Tag>
                {
                    new Tag { Name = "Owocowy" },
                    new Tag { Name = "Słodki" },
                    new Tag { Name = "Gorzki" },
                    new Tag { Name = "Czerwony" },
                    new Tag { Name = "Biały" }
                };

                _context.Tags.AddRange(tags); // Dodaj tagi do kontekstu
                _context.SaveChanges(); // Zapisz zmiany w bazie danych
            }

            return RedirectToAction("Index", "Home"); // Przekierowanie na stronę główną po dodaniu
        }
    }
}
