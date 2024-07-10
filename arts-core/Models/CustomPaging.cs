namespace arts_core.Models
{
    public class CustomPaging
    {
        public int Status { get; set; }

        public string? Message { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public bool HasPrevious => CurrentPage > 1;

        public bool HasNext => CurrentPage < TotalPages;

        public object? Data { get; set; }
    }
}
