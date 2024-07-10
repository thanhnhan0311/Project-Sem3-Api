namespace arts_core.ReturnModels
{
    public class OrderDashBoard
    {
        public int SuccessOrder { get; set; }

        public int CancelOrder { get; set; }

        public int DeliveryOrder { get; set; }

        public int AcceptedOrder { get; set; }

        public int DeniedOrder {  get; set; }

        public int RefundOrder { get; set; }    

        public int ExchangeOrder {  get; set; }
    }
}
