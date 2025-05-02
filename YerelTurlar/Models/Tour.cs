using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YerelTurlar.Models
{
    public class Tour
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tur adı zorunludur.")]
        [StringLength(150, ErrorMessage = "Tur adı en fazla 150 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tur fiyatı zorunludur.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountedPrice { get; set; }

        [StringLength(100, ErrorMessage = "Konum en fazla 100 karakter olabilir.")]
        public string Location { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Süre en fazla 50 karakter olabilir.")]
        public string Duration { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Görsel URL en fazla 500 karakter olabilir.")]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsFeatured { get; set; } = false;
        
        public bool IsActive { get; set; } = true;
        
        public int MaxParticipants { get; set; } = 10;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // İlişkiler
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [ForeignKey("Guide")]
        public int? GuideId { get; set; }
        public User? Guide { get; set; }

        public ICollection<Review>? Reviews { get; set; }
    }
} 