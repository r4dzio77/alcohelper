using Microsoft.AspNetCore.Mvc;
using AlcoHelper.Models;
using Microsoft.EntityFrameworkCore;
using AlcoHelper.Data;
using System.Collections.Generic;
using System.Linq;

namespace AlcoHelper.Controllers
{
    public class HomeController : Controller
    {
        private readonly AlcoHelperContext _context;

        public HomeController(AlcoHelperContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchTerm = null, List<int> selectedTagIds = null, string sortOrder = null)
        {
            var userName = HttpContext.Session.GetString("UserName");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.UserName = userName;
            ViewBag.Role = role;

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedTagIds = selectedTagIds ?? new List<int>();
            ViewBag.SortOrder = sortOrder ?? "name_asc";

            var approvedAlcohols = _context.Alcohols
                .Include(a => a.Reviews)
                .Include(a => a.AlcoholTags)
                .ThenInclude(at => at.Tag)
                .Where(a => a.IsApproved)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                approvedAlcohols = approvedAlcohols.Where(a => a.Name.ToLower().Contains(lowerSearchTerm));
            }


            if (selectedTagIds != null && selectedTagIds.Any())
            {
                approvedAlcohols = approvedAlcohols.Where(a => a.AlcoholTags.Any(at => selectedTagIds.Contains(at.TagId)));
            }

            // Sortowanie wg sortOrder
            switch (sortOrder)
            {
                case "name_desc":
                    approvedAlcohols = approvedAlcohols.OrderByDescending(a => a.Name);
                    break;
                case "alcohol_asc":
                    approvedAlcohols = approvedAlcohols.OrderBy(a => a.AlcoholPercentage);
                    break;
                case "alcohol_desc":
                    approvedAlcohols = approvedAlcohols.OrderByDescending(a => a.AlcoholPercentage);
                    break;
                default: // name_asc
                    approvedAlcohols = approvedAlcohols.OrderBy(a => a.Name);
                    break;
            }

            var alcoholList = approvedAlcohols.ToList();

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
                Tags = alcohol.AlcoholTags.Select(at => at.Tag.Name).ToList()
            }).ToList();

            ViewBag.Tags = _context.Tags.ToList();

            var userId = HttpContext.Session.GetInt32("UserId");

            var userFavorites = userId.HasValue
                ? _context.FavoriteAlcos
                    .Where(f => f.UserId == userId.Value)
                    .Select(f => f.AlcoholId)
                    .ToList()
                : new List<int>();

            ViewBag.UserFavorites = userFavorites;


            return View(alcoholList);
        }
    }
}