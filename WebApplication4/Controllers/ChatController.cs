using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Projekt.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projekt.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private static readonly List<ChatMessage> Messages = new();
        private readonly UserManager<IdentityUser> _userManager;

        public ChatController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Widok czatu - wyświetlanie wiadomości oraz wybór odbiorcy
        [HttpGet]


        public async Task<IActionResult> Index()
        {
            // Pobierz zalogowanego użytkownika
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Pobierz wszystkich użytkowników, poza zalogowanym
            var users = _userManager.Users.Where(u => u.UserName != user.UserName).ToList();

            ViewBag.Users = users;  // Przekaż użytkowników do widoku

            var messages = Messages.Where(m => m.Sender == user.UserName || m.Receiver == user.UserName).ToList() ?? new List<ChatMessage>();
            return View(messages);
        }


        // Wysyłanie wiadomości
        [HttpPost]
        public async Task<IActionResult> SendMessage(string receiver, string message)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(receiver) || string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Receiver and message are required.");
            }

            var chatMessage = new ChatMessage
            {
                Sender = user.UserName,
                Receiver = receiver,
                Message = message,
                Timestamp = DateTime.Now
            };

            Messages.Add(chatMessage);

            return RedirectToAction("Index");
        }
    }
}