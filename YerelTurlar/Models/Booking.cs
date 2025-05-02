using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YerelTurlar.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }
        
        public int TourId { get; set; }
        
        [ForeignKey("TourId")]
        public Tour? Tour { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int ParticipantCount { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        public bool IsPaid { get; set; } = false;
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 