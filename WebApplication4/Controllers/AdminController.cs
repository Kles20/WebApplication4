using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //lista użytkowników
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = roles
                });
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
            {
                TempData["Error"] = "Użytkownik lub rola nie została wybrana.";
                return RedirectToAction("AdminPanel");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Nie znaleziono użytkownika.";
                return RedirectToAction("AdminPanel");
            }

            if (await _userManager.IsInRoleAsync(user, role))
            {
                var result = await _userManager.RemoveFromRoleAsync(user, role);
                if (result.Succeeded)
                {
                    TempData["Success"] = $"Rola '{role}' została usunięta od użytkownika '{user.UserName}'.";
                }
                else
                {
                    TempData["Error"] = "Wystąpił błąd podczas usuwania roli.";
                }
            }
            else
            {
                TempData["Error"] = $"Użytkownik '{user.UserName}' nie posiada roli '{role}'.";
            }

            return RedirectToAction("Index");
        }
        //przypisanie roli userowi
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest("Podana rola nie istnieje.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                return BadRequest("Nie udało się przypisać roli.");
            }

            return RedirectToAction("Index");
        }
    }
}
