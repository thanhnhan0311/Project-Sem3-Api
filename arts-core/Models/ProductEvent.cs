namespace arts_core.Models
{
    public class ProductEvent
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public int VariantId { get; set; }
        public Variant? Variant { get; set; }
        public float Price { get; set; }
        public float SalePrice { get; set; }
    }
}
