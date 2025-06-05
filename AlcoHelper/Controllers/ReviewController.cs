using AlcoHelper.Data;
using AlcoHelper.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AlcoHelper.Controllers
{
    public class ReviewController : Controller
    {
        private readonly AlcoHelperContext _context;

        public ReviewController(AlcoHelperContext context)
        {
            _context = context;
        }

        public IActionResult Create(int alcoholId)
        {
            Console.WriteLine($"Received alcoholId: {alcoholId}"); // Debugowanie

            var alcohol = _context.Alcohols.Find(alcoholId); // Szukamy alkoholu po ID

            if (alcohol == null)
            {
                Console.WriteLine("No alcohol found with this ID");
                return NotFound();
            }

            Console.WriteLine($"Alcohol found: {alcohol.Name}"); // Debugowanie
            ViewBag.Alcohol = alcohol;
            return View();
        }





        // Akcja do zapisania recenzji
        [HttpPost]
        public async Task<IActionResult> Create(Review review)
        {
            // Przypisanie UserId z sesji
            var userId = HttpContext.Session.GetInt32("UserId"); // Załóżmy, że przechowujesz ID użytkownika w sesji
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            review.UserId = (int)userId;

            // Debugowanie
            Console.WriteLine($"Received AlcoholId: {review.AlcoholId}");
            Console.WriteLine($"Received UserId: {review.UserId}");

            if (ModelState.IsValid)
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Review");
            }

            return View(review);
        }










        // Akcja wyświetlająca listę recenzji
        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews.Include(r => r.Alcohol).ToListAsync();
            return View(reviews);
        }

        // Akcja wyświetlająca szczegóły recenzji
        public async Task<IActionResult> Details(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Alcohol) // Ładowanie alkoholu
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            // Pobieranie komentarzy powiązanych z recenzją
            var comments = await _context.Comments
                .Where(c => c.ReviewId == id)
                .Include(c => c.User) // Ładowanie użytkownika, który dodał komentarz
                .ToListAsync();

            // Przekazywanie komentarzy do widoku
            ViewBag.Comments = comments;

            return View(review);
        }



        // Akcja dodająca komentarz
        [HttpPost]
        public async Task<IActionResult> AddComment(int reviewId, string content)
        {
            // Zamiast Identity, używamy użytkownika z sesji
            var userId = HttpContext.Session.GetInt32("UserId"); // Zakładając, że przechowujesz ID użytkownika w sesji
            if (userId == null)
            {
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
