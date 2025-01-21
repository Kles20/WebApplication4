using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Student")]
    public class GradesStudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public GradesStudentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var subjects = await _context.Subjects
                .Include(s => s.Grades) //pobierz przedmioty i oceny
                .Select(s => new Subject
                {
                    Id = s.Id,
                    Name = s.Name,
                    Grades = s.Grades.Where(g => g.UserId == userId).ToList() //tylko oceny zalogowanego użytkownika
                })
                .ToListAsync();

            return View(subjects);
        }
    }
}
