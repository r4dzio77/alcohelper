using Microsoft.AspNetCore.Mvc;

namespace AlcoHelper.Controllers
{
    public class ChatController : Controller
    {
        private readonly GroqChatService _chatService;

        public ChatController()
        {
            _chatService = new GroqChatService("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");
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.UserMessage = HttpContext.Session.GetString("UserMessage");
            ViewBag.BotResponse = HttpContext.Session.GetString("BotResponse");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string userMessage)
        {
            var response = await _chatService.SendMessageAsync(userMessage);
            HttpContext.Session.SetString("UserMessage", userMessage);
            HttpContext.Session.SetString("BotResponse", response);

            ViewBag.UserMessage = userMessage;
            ViewBag.BotResponse = response;

            return View();
        }
    }
}