using AlcoHelper.Data;
using AlcoHelper.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlcoHelper.Controllers
{
    public class ReviewController : Controller
    {
        private readonly AlcoHelperContext _context;

        public ReviewController(AlcoHelperContext context)
        {
            _context = context;
        }

        // GET: /Review/Create?alcoholId=5
        public IActionResult Create(int alcoholId)
        {
            var alcohol = _context.Alcohols.Find(alcoholId);
            if (alcohol == null)
                return NotFound();

            ViewBag.Alcohol = alcohol;
            return View();
        }

        // POST: /Review/Create
        [HttpPost]
        public async Task<IActionResult> Create(Review review)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["LoginMessage"] = "Musisz być zalogowany, aby dodać recenzję.";
                return RedirectToAction("Login", "Account");
            }

            review.UserId = (int)userId;
            review.CreatedAt = DateTime.Now;

            // Debug walidacji
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid:");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($" - {key}: {error.ErrorMessage}");
                    }
                }
                ViewBag.Alcohol = await _context.Alcohols.FindAsync(review.AlcoholId);
                return View(review);
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            Console.WriteLine("[DEBUG] Recenzja zapisana.");
            return RedirectToAction("Index", "Review");
        }


        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewBag.SortOrder = sortOrder;

            var reviews = _context.Reviews
                .Include(r => r.Alcohol)
                .Include(r => r.User)
                .AsQueryable();

            reviews = sortOrder switch
            {
                "alcohol_desc" => reviews.OrderByDescending(r => r.Alcohol.Name),
                "alcohol_asc" => reviews.OrderBy(r => r.Alcohol.Name),
                "rating_desc" => reviews.OrderByDescending(r => r.Rating),
                "rating_asc" => reviews.OrderBy(r => r.Rating),
                "user_desc" => reviews.OrderByDescending(r => r.User.Username),
                "user_asc" => reviews.OrderBy(r => r.User.Username),
                "date_desc" => reviews.OrderByDescending(r => r.CreatedAt),
                "date_asc" => reviews.OrderBy(r => r.CreatedAt),
                _ => reviews.OrderByDescending(r => r.CreatedAt)
            };

            return View(await reviews.ToListAsync());
        }


        public async Task<IActionResult> Details(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Alcohol)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound();

            ViewBag.Comments = await _context.Comments
                .Where(c => c.ReviewId == id)
                .Include(c => c.User)
                .ToListAsync();

            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int reviewId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["LoginMessage"] = "Zaloguj się, aby dodać komentarz.";
                return RedirectToAction("Login", "Account");
            }

            var comment = new Comment
            {
                ReviewId = reviewId,
                Content = content,
                CreatedAt = DateTime.Now,
                UserId = (int)userId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = reviewId });
        }
    }
}
