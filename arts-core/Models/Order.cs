using System.ComponentModel.DataAnnotations.Schema;

namespace arts_core.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int VariantId { get; set; }
        public Variant? Variant { get; set; }
        public int Quanity { get; set; }
        public int OrderStatusId { get; set; }
        public virtual Type? OrderStatusType { get; set; }
        public float? TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Refund? Refund { get; set; }
        public Exchange? Exchange { get; set; }
        public Exchange? NewOrderExchange { get; set; }
        public int PaymentId { get; set; }
        public Payment? Payment { get; set; }
        public bool IsCancel { get; set; } = false;
        public string? CancelReason {  get; set; }
        public int? ReviewId {  get; set; }
        public Review? Review { get; set; }

        [NotMapped]
        public string OrderCode
        {
            get
            {
                var deleveryIdCodeString = Payment?.DeliveryTypeId;
                var variantIdCodeString = Variant?.VariantCode;
                var orderIdCodeString = PadLeftOrderId();

                return deleveryIdCodeString + variantIdCodeString + orderIdCodeString;
            }
        }

        private string PadLeftOrderId()
        {
            var orderIdString = Id.ToString();
            if (orderIdString.Length < 8)
            {
                return orderIdString.PadLeft(8, '0');
            }
            return orderIdString;
        }
    }
}
