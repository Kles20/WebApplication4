namespace WebApplication1.Models
{
    public class Day
    {
        public DateTime Date { get; set; }  //Data dnia
        public List<string> Events { get; set; } = new List<string>();  //Lista wydarzeń dla tego dnia
    }
}
