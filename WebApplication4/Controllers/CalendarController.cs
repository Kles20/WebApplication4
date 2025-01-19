using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Akcja do generowania kalendarza
        public IActionResult Index()
        {
            var model = new CalendarViewModel();
            model.Days = GenerateCalendar(DateTime.Now.Year, DateTime.Now.Month);

            // Pobierz wydarzenia z bazy danych
            var events = _context.Events.ToList();

            // Dodaj wydarzenia do odpowiednich dni w kalendarzu
            foreach (var week in model.Days)
            {
                foreach (var day in week)
                {
                    if (day != null)
                    {
                        foreach (var eventItem in events)
                        {
                            if (day.Date.Date == eventItem.Date.Date)
                            {
                                day.Events.Add(eventItem.Title);
                            }
                        }
                    }
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                return View("AuthenticatedCalendar", model);  // Widok dla zalogowanych użytkowników
            }
            else
            {
                return View("GuestCalendar", model);  // Widok dla niezalogowanych użytkowników
            }
        }

        // Funkcja do generowania kalendarza
        private List<List<Day>> GenerateCalendar(int year, int month)
        {
            var days = new List<List<Day>>();
            var firstDayOfMonth = new DateTime(year, month, 1);
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var currentDay = firstDayOfMonth;
            var week = new List<Day>();

            // Dodaj puste dni na początku miesiąca
            for (int i = 0; i < (int)currentDay.DayOfWeek; i++)
            {
                week.Add(null);  // Puste dni na początku miesiąca
            }

            // Wypełnij kalendarz rzeczywistymi dniami
            for (int day = 1; day <= daysInMonth; day++)
            {
                if (week.Count == 7)
                {
                    days.Add(week);
                    week = new List<Day>();
                }
                week.Add(new Day { Date = currentDay });
                currentDay = currentDay.AddDays(1);
            }

            // Dodaj pozostałe dni do ostatniego tygodnia
            if (week.Count > 0)
            {
                while (week.Count < 7)
                {
                    week.Add(null);
                }
                days.Add(week);
            }

            return days;
        }

        // Akcja do dodawania wydarzenia
        [HttpPost]
        [Authorize]  // Tylko dla zalogowanych użytkowników
        public async Task<IActionResult> AddEvent([FromBody] EventModel eventModel)
        {
            if (ModelState.IsValid)
            {
                var eventToAdd = new Event
                {
                    Date = DateTime.Parse(eventModel.Date),
                    Title = eventModel.Title
                    // Nie zapisujemy już UserId, ponieważ usunęliśmy pole UserId
                };

                _context.Events.Add(eventToAdd);
                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest("Błąd podczas dodawania wydarzenia.");
        }
    }
}
