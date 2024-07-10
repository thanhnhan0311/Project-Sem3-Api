namespace arts_core.Models
{
    public class Product
    {
        public int Id { get; set; } 
        public string? Name { get; set; } 
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive {  get; set; }
        public int CategoryId { get; set; }
        public int WarrantyDuration { get; set; }

        public string? Unit {  get; set; }

        public virtual Category? Category { get; set; }
        public ICollection<Variant> Variants { get; set; } = new List<Variant>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

      
    }
}
