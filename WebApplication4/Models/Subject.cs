namespace WebApplication1.Models
{
    public class Subject
    {
        public int Id { get; set; } // Klucz główny
        public string Name { get; set; }
        public List<Grade> Grades { get; set; } = new List<Grade>();

        // Właściwość obliczająca średnią ocen
        public decimal Average
        {
            get
            {
                if (Grades.Count > 0)
                {
                    // Obliczamy średnią z ocen i konwertujemy na decimal
                    return (decimal)Grades.Average(g => (double)g.Value);
                }
                return 0; // Jeśli brak ocen, średnia wynosi 0
            }
        }
    }

    public class Grade
    {
        public int Id { get; set; }
        public decimal Value { get; set; } // Make sure this is decimal
        public DateTime DateAdded { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
