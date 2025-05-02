using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YerelTurlar.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        
        // Kullanıcı (Değerlendirmeyi yapan kişi)
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        
        // Tur
        public int TourId { get; set; }
        [ForeignKey("TourId")]
        public Tour? Tour { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [Required]
        [MinLength(3)]
        [MaxLength(500)]
        public string? Comment { get; set; }
        
        public bool IsApproved { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 