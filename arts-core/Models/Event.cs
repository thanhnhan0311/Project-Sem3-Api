namespace arts_core.Models
{
    public class Event
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Banner { get; set; } 
        public int EventTypeId { get; set; }
        public Type? EventType { get; set; }
    }
}
