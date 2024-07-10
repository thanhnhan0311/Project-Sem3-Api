namespace arts_core.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public float ShipFee { get; set; }
        public int PaymentTypeId { get; set; }
        public Type? PaymentType { get; set; }
        public int DeliveryTypeId { get; set; }
        public Type? DeliveryType { get; set; }
        public int AddressId { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Address? Address { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
 
    }
}
