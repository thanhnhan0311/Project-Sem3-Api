namespace arts_core.Models
{
    public class Stock
    {
        public int Id { get; set; }

        public int VariantId {  get; set; }

        public float CostPerItem { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Variant? Variant { get; set; }
    }
}
