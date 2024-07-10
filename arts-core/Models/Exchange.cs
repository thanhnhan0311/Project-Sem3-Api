namespace arts_core.Models
{
    public class Exchange
    {
        public int Id { get; set; }
        public int? OriginalOrderId { get; set; }
        public Order? OriginalOrder { get; set; }
        public int? NewOrderId { get; set; }
        public Order? NewOrder { get; set; }
        public string? ReasonExchange { get; set; }
        public string? ResponseExchange { get; set; }
        public DateTime ExchangeDate { get; set; }
        public DateTime ExchangeUpdateDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public ICollection<StoreImage>? Images { get; set; }

    }
}
