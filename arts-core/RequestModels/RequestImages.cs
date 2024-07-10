namespace arts_core.RequestModels
{
    public class RequestImages
    {
        public int ProductId {  get; set; }
        public ICollection<IFormFile>? Images { get; set; }
    }
}
