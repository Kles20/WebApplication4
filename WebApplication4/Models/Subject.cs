using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class Subject
    {
        public int Id { get; set; } //Klucz główny
        public string Name { get; set; }
        public List<Grade> Grades { get; set; } = new List<Grade>();

        public decimal Average
        {
            get
            {
                if (Grades.Count > 0)
                {       
                    return (decimal)Grades.Average(g => (double)g.Value); //obliczamy średnią z ocen i konwertujemy na decimal
                }
                return 0; //jeśli brak ocen to średnia wynosi 0
            }
        }
    }

    public class Grade
    {
        public int Id { get; set; }
        public decimal Value { get; set; } 
        public DateTime DateAdded { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public string UserId { get; set; } 
        public IdentityUser User { get; set; }  // Powiązanie z użytkownikiem
    }
}
