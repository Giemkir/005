using System.ComponentModel.DataAnnotations;

namespace YerelTurlar.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Resim URL'si en fazla 100 karakter olabilir.")]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "İkon kodu en fazla 50 karakter olabilir.")]
        public string IconClass { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // İlişkiler
        public ICollection<Tour>? Tours { get; set; }
    }
} 