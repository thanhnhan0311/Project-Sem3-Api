namespace arts_core.Models
{
    public class Message
    {
        public int Id   { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string? From { get; set; } 
        public string? MessageContent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
