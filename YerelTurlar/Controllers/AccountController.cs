using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using YerelTurlar.Data;
using YerelTurlar.Models;

namespace YerelTurlar.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly AppDbContext _context;

    public AccountController(ILogger<AccountController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Login()
    {
        // Kullanıcı zaten giriş yapmışsa ana sayfaya yönlendir
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Şifreyi hashle
            string hashedPassword = HashPassword(model.Password);
            
            // Kullanıcıyı kontrol et
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive);
            
            if (user != null && user.Password == hashedPassword)
            {
                // Session'a kullanıcı bilgilerini sakla
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                HttpContext.Session.SetString("UserEmail", user.Email);
                
                _logger.LogInformation($"Kullanıcı girişi başarılı: {model.Email}");
                
                // Başarılı giriş sonrası ana sayfaya yönlendir
                return RedirectToAction("Index", "Home");
            }
            
            ModelState.AddModelError("", "E-posta adresi veya şifre hatalı.");
            _logger.LogWarning($"Başarısız giriş denemesi: {model.Email}");
        }
        
        return View(model);
    }

    public IActionResult Register()
    {
        // Kullanıcı zaten giriş yapmışsa ana sayfaya yönlendir
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Email zaten kullanılıyor mu kontrol et
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                return View(model);
            }
            
            // Yeni kullanıcı oluştur
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Password = HashPassword(model.Password),
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Yeni kullanıcı kaydı başarılı: {model.Email}");
            
            // Başarılı kayıt sonrası giriş sayfasına yönlendir
            TempData["SuccessMessage"] = "Kaydınız başarıyla oluşturuldu. Lütfen giriş yapın.";
            return RedirectToAction(nameof(Login));
        }
        
        return View(model);
    }

    public IActionResult Logout()
    {
        // Session'ı temizle
        HttpContext.Session.Clear();
        
        _logger.LogInformation("Kullanıcı çıkış yaptı.");
        
        return RedirectToAction("Index", "Home");
    }

    public IActionResult ExternalLogin(string provider)
    {
        // Burada harici giriş işlemleri yapılacak
        // Örneğin: Google, Facebook vb. ile giriş
        
        _logger.LogInformation($"Harici giriş isteği: {provider}");
        
        // Şimdilik sadece log kaydı oluşturup ana sayfaya yönlendiriyoruz
        TempData["InfoMessage"] = $"{provider} ile giriş özelliği henüz aktif değildir.";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Kullanıcıyı e-posta adresine göre kontrol et
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive);
            if (user != null)
            {
                // Gerçek uygulamada burada şifre sıfırlama e-postası gönderme işlemi yapılacak
                _logger.LogInformation($"Şifre sıfırlama isteği: {model.Email}");
            }
            
            // Kullanıcı bulunsa da bulunmasa da aynı mesajı göster (güvenlik)
            TempData["SuccessMessage"] = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
            return RedirectToAction(nameof(Login));
        }
        
        return View(model);
    }
    
    // Şifre hashleme yardımcı metodu
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            
            return builder.ToString();
        }
    }
} 