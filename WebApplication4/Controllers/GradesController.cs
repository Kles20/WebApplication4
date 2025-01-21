using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class GradesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public GradesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //akcja do wyświetlania przedmiotów i ocen
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Grades)
                .ToListAsync();

            var allUsers = await _userManager.Users.ToListAsync(); //pobierz użytkowników

            
            var studentUsers = new List<IdentityUser>();
            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Student"))
                {
                    studentUsers.Add(user);
                }
            }

            var model = new GradesViewModel
            {
                Subjects = subjects,
                Users = studentUsers
            };

            return View(model); //tylko użytkownicy z rolą studenta
        }

        public async Task<IActionResult> ListUsers()
        {
            var users = await _userManager.Users.ToListAsync(); 

            return View(users); //przekaż użytkowników do widoku
        }

        [HttpPost]
        public async Task<IActionResult> AddGrade(string subjectName, double grade, string userId)
        {
            if (grade < 1 || grade > 6)
            {
                return BadRequest("Ocena musi być w przedziale 1-6.");
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Name == subjectName);

            var user = await _userManager.FindByIdAsync(userId); //znajdź ucznia po userId

            if (user == null || !await _userManager.IsInRoleAsync(user, "Student"))
            {
                return BadRequest("Można wystawiać oceny tylko uczniom.");
            }

            if (subject != null)
            {
                var newGrade = new Grade
                {
                    Value = (decimal)grade,
                    DateAdded = DateTime.Now,
                    SubjectId = subject.Id,
                    UserId = userId
                };

                _context.Grades.Add(newGrade);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddSubject(string newSubjectName)
        {
            if (!string.IsNullOrWhiteSpace(newSubjectName) && !await _context.Subjects.AnyAsync(s => s.Name == newSubjectName))
            {
                var newSubject = new Subject
                {
                    Name = newSubjectName
                };

                _context.Subjects.Add(newSubject);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
