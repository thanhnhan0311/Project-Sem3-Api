namespace arts_core.RequestModels
{
    public class RequestCancel
    {
        public int OrderId { get; set; }

        public string? Reason { get; set; }
        
    }
}
