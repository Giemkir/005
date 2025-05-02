using System;
using System.ComponentModel.DataAnnotations;

namespace YerelTurlar.Models
{
    public class NewsletterSubscription
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        public DateTime SubscribedAt { get; set; } = DateTime.Now;
        
        public bool IsActive { get; set; } = true;
    }
} 