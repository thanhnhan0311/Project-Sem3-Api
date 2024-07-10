namespace arts_core.Models
{
    public class CustomResult
    {
        public CustomResult(int status, string message, object data)
        {
            Status = status;
            Message = message;
            Data = data;
        }

        public int Status { get; set; }

        public string Message { get; set; }

        public object? Data { get; set; }
    }
}
