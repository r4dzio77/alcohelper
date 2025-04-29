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
            var user = new User
            {
                Username = "exampleUser",
                Email = "user@example.com",
                PasswordHash = "hashed_password", // Tu daj prawdziwy hash
                ProfilePictureUrl = "url_to_picture", // Jeśli jest opcjonalne, to zostaw puste
                Role = "User", // lub Admin, w zależności od roli
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Json(new { message = "Dodano uzytkownika do bazy danych." });
        }
    }
}