﻿namespace Projekt.Models
{
    public class ChatMessage
    {
        public int Id { get; set; } // Klucz główny w bazie danych
        public string Sender { get; set; }  // Nadawca wiadomości
        public string Receiver { get; set; } // Odbiorca wiadomości
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

}