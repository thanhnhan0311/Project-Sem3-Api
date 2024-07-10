namespace arts_core.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int VariantId { get; set; } 
        public Variant? Variant { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int Quanity { get; set; }
        public bool IsChecked { get; set; } = false;
    }
}
