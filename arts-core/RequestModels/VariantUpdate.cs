using Microsoft.Identity.Client;

namespace arts_core.RequestModels
{
    public class VariantUpdate
    {
        public int AvailableQuanity {  get; set; }

        public int Id { get; set; }

        public float Price { get; set; }

        public float SalePrice { get; set; }

        public int Quanity { get; set; }

        public string? VariantImage { get; set; }


    }
}
