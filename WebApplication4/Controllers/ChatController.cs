using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Projekt.Models;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;

namespace Projekt.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ChatController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        //wyświetlanie wiadomości oraz wybór odbiorcy
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //pobierz zalogowanego użytkownika
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //pobierz wszystkich użytkowników, bez zalogowanego
            var users = _userManager.Users.Where(u => u.UserName != user.UserName).ToList();
            ViewBag.Users = users;

            //pobierz wiadomości zalogowanego użytkownika
            var messages = _context.ChatMessages
                .Where(m => m.Sender == user.UserName || m.Receiver == user.UserName)
                .OrderBy(m => m.Timestamp) 
                .ToList();

            return View(messages);
        }

        
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
            //Wysyłanie wiadomości
            
            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Zapisz wiadomość w bazie danych
        }
    }
}