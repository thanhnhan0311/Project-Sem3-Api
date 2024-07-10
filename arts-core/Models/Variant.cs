using System.ComponentModel.DataAnnotations.Schema;

namespace arts_core.Models
{
    public class Variant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string? VariantImage { get; set; }
        public int Quanity { get; set; }
        public int AvailableQuanity { get; set; }
        public float Price { get; set; }
        public float SalePrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Active { get; set; } = true;

        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<VariantAttribute>? VariantAttributes { get; set; }
        public virtual ICollection<Type>? Types { get; set; }
        public ICollection<User>? Users { get; set; }


        [NotMapped]
        public string VariantCode
        {
            get
            {
                var categoryId = Product?.CategoryId;
                if (categoryId < 10)
                {
                    return "0" + categoryId + PadLeftVariantId();
                }
                return categoryId.ToString();
            }
        }

        private string PadLeftVariantId()
        {
            var variantString = Id.ToString();
            if (variantString.Length < 5)
            {
                return variantString.PadLeft(5, '0');
            }
            return variantString;
        }
    }
}
