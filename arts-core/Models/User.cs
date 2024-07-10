namespace arts_core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; } 
        public string? Fullname { get; set; } 
        public string? Password { get; set; } 
        public string? Address { get; set; } 
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; } 
        public bool Verifired { get; set; } = false;
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 

        public DateTime? Dob {  get; set; }
        public bool Active { get; set; } = false ;
        public int? RoleTypeId { get; set; }
        public Type? RoleType { get; set; }
        public int? RestrictedTypeId { get; set; }
        public Type? RestrictedType { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Message>? Messages { get; set; }
        public ICollection<Variant>? Variants { get; set; }

        public string? Gender { get; set; }

        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    }
}
