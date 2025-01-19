using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class GradesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Akcja do wyświetlania przedmiotów i ocen
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Grades)
                .ToListAsync();

            return View(subjects);
        }

        [HttpPost]
        public async Task<IActionResult> AddGrade(string subjectName, double grade)
        {
            if (grade < 1 || grade > 6)
            {
                return BadRequest("Ocena musi być w przedziale 1-6.");
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Name == subjectName);

            if (subject != null)
            {
                var newGrade = new Grade
                {
                    Value = (decimal)grade, // jawne rzutowanie z double na decimal
                    DateAdded = DateTime.Now,
                    SubjectId = subject.Id
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
