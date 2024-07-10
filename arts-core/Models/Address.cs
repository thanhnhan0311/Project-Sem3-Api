namespace arts_core.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string? FullName {  get; set; }

        public int UserId { get; set; }

        public string? PhoneNumber { get; set; }

        public string? AddressDetail {  get; set; }

        public string? Province { get; set; }    

        public string? District {  get; set; }

        public string? Ward { get; set; }

        public bool IsDefault {  get; set; }

        public virtual User? User { get; set; }

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();


    }
}
