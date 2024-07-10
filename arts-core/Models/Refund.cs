namespace arts_core.Models
{
    public class Refund
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public string? ReasonRefund { get; set; }
        public string? ResponseRefund { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public float AmountRefund { get; set; }
        public ICollection<StoreImage>? Images { get; set; }

    }
}
