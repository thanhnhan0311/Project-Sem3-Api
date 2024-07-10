namespace arts_core.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? IconImage { get; set; } 
        public ICollection<Product>? Products { get; set; }
    }
}
