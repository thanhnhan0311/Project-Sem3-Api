using System.Collections.Generic;

namespace arts_core.ReturnModels
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
  
        public IEnumerable<VariantDTO> Variants { get; set; }

        public IEnumerable<ReviewDTO> Reviews { get; set; } 

        public IEnumerable<ProductImageDTO> ProductImages { get; set; } 
    }
}
