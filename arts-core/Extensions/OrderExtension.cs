using arts_core.Models;

namespace arts_core.Extensions
{
    public static class OrderExtension
    {
        public static float CalculateTotalPrice(this List<Order> orders)
        {
            return (float)orders.Sum(o => o.TotalPrice);
        }
        public static List<VariantsCount> GetVariantsCountOfOrders(this List<Order> orders)
        {
            var variantscount = orders
              .GroupBy(o => o.Variant)
              .Select(g => new VariantsCount
              {
                  VariantId = g.Key.Id,
                  VariantCode = g.Key.VariantCode,
                  Count = g.Count(),
                  TotalQuanity = g.Sum(v => v.Quanity)
              })
              .OrderByDescending(g => g.Count)
              .ToList();
            return variantscount;
        }
    }

    public class VariantsCount
    {
        public int VariantId { get; set; }
        public int Count { get; set; }
        public string VariantCode { get; set; } = string.Empty;
        public int TotalQuanity { get; set; }
    }
}