using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AlcoHelper.Controllers
{
    public class ChatController : Controller
    {
        private readonly GroqChatService _chatService;

        public ChatController()
        {
            _chatService = new GroqChatService("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");
        }

        private List<JsonObject> GetChatHistory()
        {
            var historyJson = HttpContext.Session.GetString("ChatHistory");
            return string.IsNullOrEmpty(historyJson) 
                ? new List<JsonObject>() 
                : JsonSerializer.Deserialize<List<JsonObject>>(historyJson);
        }

        private void SaveChatHistory(List<JsonObject> history)
        {
            HttpContext.Session.SetString("ChatHistory", JsonSerializer.Serialize(history));
        }

        [HttpPost]
        public IActionResult ClearChat()
        {
            HttpContext.Session.Remove("ChatHistory");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var lastUserId = HttpContext.Session.GetInt32("LastUserId");

            if (!currentUserId.HasValue) return RedirectToAction("Index", "Home");

            if (lastUserId.HasValue && lastUserId != currentUserId)
            {
                HttpContext.Session.Remove("ChatHistory");
            }
            
            HttpContext.Session.SetInt32("LastUserId", currentUserId.Value);

            var chatHistory = GetChatHistory();
            
            if (chatHistory.Count == 0)
            {
                chatHistory.Add(_chatService.CreateSystemMessage());
                SaveChatHistory(chatHistory);
            }


            ViewBag.ChatHistory = chatHistory;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string userMessage)
        {
            var chatHistory = GetChatHistory();
            
            // Dodaj wiadomość użytkownika
            chatHistory.Add(_chatService.CreateUserMessage(userMessage));
            
            // Wyślij do API i dodaj odpowiedź
            var response = await _chatService.SendMessageAsync(chatHistory);
            chatHistory.Add(_chatService.CreateAssistantMessage(response));
            
            // Zapisz historię
            SaveChatHistory(chatHistory);

            ViewBag.ChatHistory = chatHistory;
            return View();
        }
    }
}
