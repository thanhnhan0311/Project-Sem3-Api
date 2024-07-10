namespace arts_core.RequestModels
{
    public class PaymentRequest
    {
        public int PaymentTypeId { get; set; }
        public int AddressId { get; set; }
        public int DeliveryTypeId { get; set; }
    }
}
