namespace arts_core.Models
{
    public class StoreImage
    {
        public int Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
        public int? RefundId { get; set; }
        public int? ExchangeId { get; set; }
        public Exchange? Exchange { get; set; }
        public Refund? Refund { get; set; }
    }
}
