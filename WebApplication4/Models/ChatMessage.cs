namespace Projekt.Models
{
    public class ChatMessage
    {
        public int Id { get; set; } //klucz główny
        public string Sender { get; set; }  //nadawca 
        public string Receiver { get; set; } //odbiorca
        public string Message { get; set; } //treśc
        public DateTime Timestamp { get; set; } //data i godzina
    }

}