using Microsoft.AspNetCore.Http;

namespace YerelTurlar.ViewModels
{
    public class SiteSettingsViewModel
    {
        // Genel Ayarlar
        public string? SiteName { get; set; }
        public string? SiteDescription { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }
        public string? LogoUrl { get; set; }
        public string? FaviconUrl { get; set; }
        public IFormFile? LogoFile { get; set; }
        public IFormFile? FaviconFile { get; set; }
        
        // Sosyal Medya ve SEO
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        
        // Ödeme Ayarları
        public string? Currency { get; set; } = "TRY";
        public bool CreditCardEnabled { get; set; } = true;
        public bool PaypalEnabled { get; set; } = false;
        public bool BankTransferEnabled { get; set; } = true;
        public bool CashOnDeliveryEnabled { get; set; } = true;
        public string? BankDetails { get; set; }
        
        // Email Ayarları
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; } = 587;
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        public bool SmtpSsl { get; set; } = true;
        public string? EmailSender { get; set; }
        public string? EmailSenderName { get; set; }
    }
} 