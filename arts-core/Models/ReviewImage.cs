namespace arts_core.Models
{
    public class ReviewImage
    {
        public int Id { get; set; }

        public int ReviewId {  get; set; }

        public virtual Review? Review { get; set; }

        public string? ImageName {  get; set; }
    }
}
