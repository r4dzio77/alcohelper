using Microsoft.AspNetCore.Mvc;
using AlcoHelper.Models;
using System.Collections.Generic;
using AlcoHelper.Data;

namespace AlcoHelper.Controllers
{
    public class AlcoholController : Controller
    {
        private readonly AlcoHelperContext _context;

        public AlcoholController(AlcoHelperContext context)
        {
            _context = context;
        }

        public IActionResult Test()
        {
            var alcohol = new Alcohol
            {
                Id = 1,
                Name = "Whisky Jack Daniel's",
                //ImageUrl = "huhuhu", // Dodaj jakąś wartość dla ImageUrl
                Reviews = new List<Review>
                {
                    new Review { Id = 1, Rating = 5, Comment = "Super whisky!" }
                }
            };

            _context.Alcohols.Add(alcohol);
            _context.SaveChanges();

            return Json(new { message = "Dodano alkohol do bazy danych." });
        }
    }
}