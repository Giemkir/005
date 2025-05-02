using System.ComponentModel.DataAnnotations;

namespace YerelTurlar.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ad alanı zorunludur.")]
    [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
    [Display(Name = "Ad")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Soyad alanı zorunludur.")]
    [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
    [Display(Name = "Soyad")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [Display(Name = "E-posta")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Telefon alanı zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    [Display(Name = "Telefon")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Şifre alanı zorunludur.")]
    [StringLength(100, ErrorMessage = "Şifre en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Şifre tekrar alanı zorunludur.")]
    [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre (Tekrar)")]
    public string? ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Kullanım koşullarını kabul etmelisiniz.")]
    [Display(Name = "Kullanım Koşulları")]
    public bool AcceptTerms { get; set; }
} 