using System.ComponentModel.DataAnnotations;

namespace YerelTurlar.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Lütfen adınızı giriniz")]
        [Display(Name = "Adınız")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Lütfen e-posta adresinizi giriniz")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [Display(Name = "E-posta Adresiniz")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Lütfen konu giriniz")]
        [Display(Name = "Konu")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Lütfen mesajınızı giriniz")]
        [Display(Name = "Mesajınız")]
        public string? Message { get; set; }
    }
} 