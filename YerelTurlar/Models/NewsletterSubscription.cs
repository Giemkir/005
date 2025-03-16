using System;
using System.ComponentModel.DataAnnotations;

namespace YerelTurlar.Models
{
    public class NewsletterSubscription
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(150, ErrorMessage = "E-posta en fazla 150 karakter olabilir.")]
        public string Email { get; set; }

        public DateTime SubscriptionDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
} 