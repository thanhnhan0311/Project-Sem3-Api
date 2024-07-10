namespace arts_core.RequestModels
{
    public class ChangePasswordRequest
    {
        public string PreviousPassword { get; set; }

        public string NewPassword { get; set; } 
    }
}
