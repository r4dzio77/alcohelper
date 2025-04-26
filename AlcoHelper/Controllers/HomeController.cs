using Microsoft.AspNetCore.Mvc;
using AlcoHelper.Models;
using System.Text.Json;

namespace AlcoHelper.Controllers
{
    public class HomeController : Controller
    {
        private readonly AlcoholController _alcoholController;

        public HomeController(AlcoholController alcoholController)
        {
            _alcoholController = alcoholController;
        }

        public IActionResult Index()
        {
            // Wywo³aj akcjê Test z AlcoholController
            var result = _alcoholController.Test() as JsonResult;
            if (result?.Value != null)
            {
                // Przekonwertuj dane na JSON i przeka¿ do widoku
                ViewBag.AlcoholData = JsonSerializer.Serialize(result.Value);
            }
            return View();
        }
    }
}