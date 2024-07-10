namespace arts_core.RequestModels
{
    public class RequestReview
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public string? Comment { get; set; }
        public ICollection<IFormFile>? Images { get; set; }
        public int Rating { get; set; }
    }
}
