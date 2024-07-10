﻿namespace arts_core.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public virtual ICollection<ReviewImage>? ReviewImages { get; set; } = new List<ReviewImage>();
        public virtual Order? Order { get; set; }
    }
}
