using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YerelTurlar.Data;
using YerelTurlar.Models;
using YerelTurlar.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace YerelTurlar.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AppDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
            
            // Admin kullanıcısı henüz yoksa oluştur
            SeedAdminUser().Wait();
        }
        
        // Demo için admin kullanıcısı oluşturma metodu
        private async Task SeedAdminUser()
        {
            try
            {
                // Admin kullanıcısını kontrol et
                var adminExists = await _context.Users.AnyAsync(u => u.Email == "admin@yerelturlar.com" && u.IsAdmin);
                
                // Admin kullanıcısı yoksa oluştur
                if (!adminExists)
                {
                    var admin = new User
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        Email = "admin@yerelturlar.com",
                        Phone = "5551234567",
                        Password = HashPassword("admin123"),
                        IsAdmin = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        EmailConfirmed = true
                    };
                    
                    _context.Users.Add(admin);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Admin kullanıcısı başarıyla oluşturuldu.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin kullanıcısı oluşturulurken hata meydana geldi.");
            }
        }

        // Şifre hashleme fonksiyonu
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Admin giriş sayfası
        public IActionResult Login()
        {
            // Eğer kullanıcı zaten admin olarak giriş yapmışsa, dashboard'a yönlendir
            if (HttpContext.Session.GetInt32("AdminId") != null)
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        // Demo giriş işlemi - otomatik admin girişi yapar
        public async Task<IActionResult> DemoLogin()
        {
            try
            {
                // Demo admin kullanıcısını kontrol et
                var admin = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == "admin@yerelturlar.com" && u.IsAdmin);
                
                if (admin == null)
                {
                    // Admin yoksa oluşturma işlemini tekrar çalıştır
                    await SeedAdminUser();
                    admin = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == "admin@yerelturlar.com" && u.IsAdmin);
                        
                    if (admin == null)
                    {
                        TempData["ErrorMessage"] = "Demo giriş yapılamadı. Lütfen normal giriş yapmayı deneyin.";
                        return RedirectToAction("Login");
                    }
                }
                
                // Session'a admin bilgilerini kaydet
                HttpContext.Session.SetInt32("AdminId", admin.Id);
                HttpContext.Session.SetString("AdminName", $"{admin.FirstName} {admin.LastName}");
                
                // Demo giriş bilgisini session'a kaydet
                HttpContext.Session.SetString("IsDemoLogin", "true");
                
                _logger.LogInformation($"Demo giriş yapıldı: {admin.Email}");
                
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Demo giriş sırasında hata oluştu");
                TempData["ErrorMessage"] = "Demo giriş yapılamadı. Bir hata oluştu.";
                return RedirectToAction("Login");
            }
        }

        // Admin giriş işlemi
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Email ve şifreye göre kullanıcıyı bul
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive);

            if (user == null)
            {
                ModelState.AddModelError("", "Geçersiz email veya şifre");
                return View(model);
            }

            // Şifreyi kontrol et
            string hashedPassword = HashPassword(model.Password);
            if (user.Password != hashedPassword)
            {
                ModelState.AddModelError("", "Geçersiz email veya şifre");
                return View(model);
            }

            // Admin yetkisi kontrolü
            if (!user.IsAdmin)
            {
                ModelState.AddModelError("", "Bu sayfaya erişim yetkiniz bulunmamaktadır");
                return View(model);
            }

            // Session'a admin bilgilerini kaydet
            HttpContext.Session.SetInt32("AdminId", user.Id);
            HttpContext.Session.SetString("AdminName", $"{user.FirstName} {user.LastName}");

            return RedirectToAction("Dashboard");
        }

        // Admin Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Admin giriş kontrolü
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                // İstatistikler için verileri çekelim
                ViewBag.TotalUsers = _context.Users.Count();
                ViewBag.TotalCategories = _context.Categories.Count();
                ViewBag.TotalTours = _context.Tours.Count();
                ViewBag.TotalReviews = _context.Reviews.Count();

                // Son kaydolan kullanıcıları çekelim
                ViewBag.RecentUsers = await _context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                // Demo giriş kontrolü
                ViewBag.IsDemoLogin = HttpContext.Session.GetString("IsDemoLogin") == "true";

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard verilerini çekerken hata oluştu.");
                return View("Error");
            }
        }

        // Admin Çıkış İşlemi
        public IActionResult Logout()
        {
            // Admin session bilgilerini temizle
            HttpContext.Session.Remove("AdminId");
            HttpContext.Session.Remove("AdminName");
            HttpContext.Session.Remove("IsDemoLogin");

            return RedirectToAction("Login");
        }

        // Kullanıcı Listesi
        public async Task<IActionResult> Users()
        {
            // Admin giriş kontrolü
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // Kullanıcı detayları
        public async Task<IActionResult> EditUser(int id)
        {
            // Eğer admin girişi yapılmamışsa login sayfasına yönlendir
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Kullanıcı güncelleme
        [HttpPost]
        public async Task<IActionResult> EditUser(User user)
        {
            // Eğer admin girişi yapılmamışsa login sayfasına yönlendir
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                // Kullanıcı bilgilerini güncelle
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.IsActive = user.IsActive;
                existingUser.IsAdmin = user.IsAdmin;

                // Eğer yeni şifre belirtilmişse güncelle
                if (!string.IsNullOrEmpty(user.Password))
                {
                    existingUser.Password = HashPassword(user.Password);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Users");
            }

            return View(user);
        }

        // Kullanıcı silme
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Eğer admin girişi yapılmamışsa login sayfasına yönlendir
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        // Kategorileri listele
        public async Task<IActionResult> Categories()
        {
            // Eğer admin girişi yapılmamışsa login sayfasına yönlendir
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var categories = await _context.Categories
                .Include(c => c.Tours)
                .ToListAsync();
            return View(categories);
        }

        // Turları listele
        public async Task<IActionResult> Tours()
        {
            // Eğer admin girişi yapılmamışsa login sayfasına yönlendir
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            // ViewBag'e kategorileri ve kullanıcıları ekleyelim
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Guides = await _context.Users.Where(u => u.IsActive).ToListAsync();

            var tours = await _context.Tours
                .Include(t => t.Category)
                .ToListAsync();
            return View(tours);
        }

        // Kategori bilgilerini getir (AJAX için)
        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            
            return Json(new
            {
                id = category.Id,
                name = category.Name,
                description = category.Description,
                iconClass = category.IconClass,
                imageUrl = category.ImageUrl,
                isActive = category.IsActive
            });
        }
        
        // Tur bilgilerini getir (AJAX için)
        [HttpGet]
        public async Task<IActionResult> GetTour(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }
            
            return Json(new
            {
                id = tour.Id,
                name = tour.Name,
                description = tour.Description,
                categoryId = tour.CategoryId,
                price = tour.Price,
                discountedPrice = tour.DiscountedPrice,
                location = tour.Location,
                duration = tour.Duration,
                maxParticipants = tour.MaxParticipants,
                guideId = tour.GuideId,
                imageUrl = tour.ImageUrl,
                isFeatured = tour.IsFeatured,
                isActive = tour.IsActive
            });
        }
        
        // Kategori ekleme
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category, IFormFile ImageFile)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            
            if (ModelState.IsValid)
            {
                // Resim yükleme işlemi
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "categories", fileName);
                    
                    // Dizin yoksa oluştur
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    
                    category.ImageUrl = "/uploads/categories/" + fileName;
                }
                
                category.CreatedAt = DateTime.Now;
                
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Categories));
            }
            
            return View(category);
        }
        
        // Kategori düzenleme
        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category, IFormFile ImageFile)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = await _context.Categories.FindAsync(category.Id);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }
                    
                    // Mevcut değerleri güncelle
                    existingCategory.Name = category.Name;
                    existingCategory.Description = category.Description;
                    existingCategory.IconClass = category.IconClass;
                    existingCategory.IsActive = category.IsActive;
                    
                    // Resim yükleme işlemi
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "categories", fileName);
                        
                        // Dizin yoksa oluştur
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                        
                        existingCategory.ImageUrl = "/uploads/categories/" + fileName;
                    }
                    
                    _context.Update(existingCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(e => e.Id == category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Categories));
            }
            
            return RedirectToAction(nameof(Categories));
        }
        
        // Kategori silme
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            
            var category = await _context.Categories
                .Include(c => c.Tours)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (category == null)
            {
                return NotFound();
            }
            
            // Kategoriye ait tur var mı kontrol et
            if (category.Tours != null && category.Tours.Any())
            {
                TempData["Error"] = "Bu kategoriye ait turlar var. Önce bu turları başka bir kategoriye taşıyın veya silin.";
                return RedirectToAction(nameof(Categories));
            }
            
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Categories));
        }
        
        // Tur ekleme
        [HttpPost]
        public async Task<IActionResult> AddTour(Tour tour, IFormFile ImageFile)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            
            if (ModelState.IsValid)
            {
                // Resim yükleme işlemi
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "tours", fileName);
                    
                    // Dizin yoksa oluştur
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    
                    tour.ImageUrl = "/uploads/tours/" + fileName;
                }
                
                tour.CreatedAt = DateTime.Now;
                
                _context.Tours.Add(tour);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Tours));
            }
            
            // Validasyon hatası durumunda
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Guides = await _context.Users.Where(u => u.IsActive).ToListAsync();
            
            return View(tour);
        }
        
        // Tur düzenleme
        [HttpPost]
        public async Task<IActionResult> EditTour(Tour tour, IFormFile ImageFile)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var existingTour = await _context.Tours.FindAsync(tour.Id);
                    if (existingTour == null)
                    {
                        return NotFound();
                    }
                    
                    // Mevcut değerleri güncelle
                    existingTour.Name = tour.Name;
                    existingTour.Description = tour.Description;
                    existingTour.CategoryId = tour.CategoryId;
                    existingTour.Price = tour.Price;
                    existingTour.DiscountedPrice = tour.DiscountedPrice;
                    existingTour.Location = tour.Location;
                    existingTour.Duration = tour.Duration;
                    existingTour.MaxParticipants = tour.MaxParticipants;
                    existingTour.GuideId = tour.GuideId;
                    existingTour.IsFeatured = tour.IsFeatured;
                    existingTour.IsActive = tour.IsActive;
                    
                    // Resim yükleme işlemi
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "tours", fileName);
                        
                        // Dizin yoksa oluştur
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                        
                        existingTour.ImageUrl = "/uploads/tours/" + fileName;
                    }
                    
                    _context.Update(existingTour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Tours.Any(e => e.Id == tour.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Tours));
            }
            
            // Validasyon hatası durumunda
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Guides = await _context.Users.Where(u => u.IsActive).ToListAsync();
            
            return View(tour);
        }
        
        // Tur silme
        [HttpPost]
        public async Task<IActionResult> DeleteTour(int id)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }
            
            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Tours));
        }

        // Değerlendirmeler
        public IActionResult Reviews()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            var reviews = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Tour)
                .ToList();

            ViewBag.Tours = _context.Tours.ToList();

            return View(reviews);
        }

        // Değerlendirme Detayı
        [HttpGet("Admin/GetReview/{id}")]
        public IActionResult GetReview(int id)
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Yetkisiz erişim" });

            var review = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Tour)
                .FirstOrDefault(r => r.Id == id);

            if (review == null)
                return Json(new { success = false, message = "Değerlendirme bulunamadı" });

            return Json(review);
        }

        // Değerlendirme Onaylama
        [HttpGet("Admin/ApproveReview/{id}")]
        public IActionResult ApproveReview(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            var review = _context.Reviews.Find(id);
            if (review == null)
            {
                TempData["ErrorMessage"] = "Değerlendirme bulunamadı";
                return RedirectToAction(nameof(Reviews));
            }

            review.IsApproved = true;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Değerlendirme başarıyla onaylandı";
            return RedirectToAction(nameof(Reviews));
        }

        // Değerlendirme Silme
        [HttpGet("Admin/DeleteReview/{id}")]
        public IActionResult DeleteReview(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            var review = _context.Reviews.Find(id);
            if (review == null)
            {
                TempData["ErrorMessage"] = "Değerlendirme bulunamadı";
                return RedirectToAction(nameof(Reviews));
            }

            _context.Reviews.Remove(review);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Değerlendirme başarıyla silindi";
            return RedirectToAction(nameof(Reviews));
        }

        // Rezervasyonlar
        public IActionResult Bookings()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            var bookings = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tour)
                .ToList();

            ViewBag.Tours = _context.Tours.ToList();

            return View(bookings);
        }

        // Rezervasyon Detayı
        [HttpGet("Admin/GetBooking/{id}")]
        public IActionResult GetBooking(int id)
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Yetkisiz erişim" });

            var booking = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tour)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
                return Json(new { success = false, message = "Rezervasyon bulunamadı" });

            return Json(booking);
        }

        // Rezervasyon Durum Güncelleme
        [HttpPost]
        public IActionResult UpdateBookingStatus(int id, string status, bool isPaid, string notes)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                TempData["ErrorMessage"] = "Rezervasyon bulunamadı";
                return RedirectToAction(nameof(Bookings));
            }

            booking.Status = status;
            booking.IsPaid = isPaid;
            
            if (!string.IsNullOrEmpty(notes))
            {
                booking.Notes = notes;
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Rezervasyon durumu başarıyla güncellendi";
            return RedirectToAction(nameof(Bookings));
        }

        // Rezervasyon Silme
        [HttpGet("Admin/DeleteBooking/{id}")]
        public IActionResult DeleteBooking(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                TempData["ErrorMessage"] = "Rezervasyon bulunamadı";
                return RedirectToAction(nameof(Bookings));
            }

            _context.Bookings.Remove(booking);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Rezervasyon başarıyla silindi";
            return RedirectToAction(nameof(Bookings));
        }

        // Site Ayarları
        public IActionResult Settings()
        {
            // Eğer admin girişi yapılmamışsa login sayfasına yönlendir
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var settings = GetSiteSettings();
            return View(settings);
        }
        
        // Site ayarlarını getir
        private SiteSettingsViewModel GetSiteSettings()
        {
            // Örnek olarak varsayılan değerler ile bir model döndürüyoruz
            // Gerçek uygulamada bu veriler veritabanından çekilecektir
            return new SiteSettingsViewModel
            {
                SiteName = "Yerel Turlar",
                SiteDescription = "Yerel rehberler ile unutulmaz turlar",
                ContactEmail = "info@yerelturlar.com",
                ContactPhone = "+90 555 123 4567",
                Address = "Büyükdere Cad. No:123 Şişli, İstanbul",
                LogoUrl = "/img/logo.png",
                FaviconUrl = "/img/favicon.ico",
                
                // Sosyal Medya ve SEO
                FacebookUrl = "https://facebook.com/yerelturlar",
                TwitterUrl = "https://twitter.com/yerelturlar",
                InstagramUrl = "https://instagram.com/yerelturlar",
                YoutubeUrl = "https://youtube.com/yerelturlar",
                MetaTitle = "Yerel Turlar | Yerel Rehberler ile Turlar",
                MetaDescription = "Yerel rehberler eşliğinde unutulmaz turlar, kültür gezileri ve macera aktiviteleri",
                MetaKeywords = "tur, gezi, yerel rehber, macera, kültür turu, seyahat",
                
                // Ödeme Ayarları
                Currency = "TRY",
                CreditCardEnabled = true,
                PaypalEnabled = false,
                BankTransferEnabled = true,
                CashOnDeliveryEnabled = true,
                BankDetails = "Yerel Turlar A.Ş.\nBanka: XYZ Bank\nIBAN: TR12 3456 7890 1234 5678 90",
                
                // Email Ayarları
                SmtpServer = "smtp.example.com",
                SmtpPort = 587,
                SmtpUsername = "info@yerelturlar.com",
                SmtpPassword = "**********",
                SmtpSsl = true,
                EmailSender = "info@yerelturlar.com",
                EmailSenderName = "Yerel Turlar"
            };
        }
        
        // Genel ayarları güncelle
        [HttpPost]
        public async Task<IActionResult> UpdateGeneralSettings(SiteSettingsViewModel model)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            
            try
            {
                // Logo yükleme işlemi
                if (model.LogoFile != null && model.LogoFile.Length > 0)
                {
                    var logoFileName = Path.GetFileName(model.LogoFile.FileName);
                    var logoFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "settings", logoFileName);
                    
                    // Dizin yoksa oluştur
                    Directory.CreateDirectory(Path.GetDirectoryName(logoFilePath));
                    
                    using (var stream = new FileStream(logoFilePath, FileMode.Create))
                    {
                        await model.LogoFile.CopyToAsync(stream);
                    }
                    
                    model.LogoUrl = "/uploads/settings/" + logoFileName;
                }
                
                // Favicon yükleme işlemi
                if (model.FaviconFile != null && model.FaviconFile.Length > 0)
                {
                    var faviconFileName = Path.GetFileName(model.FaviconFile.FileName);
                    var faviconFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "settings", faviconFileName);
                    
                    // Dizin yoksa oluştur
                    Directory.CreateDirectory(Path.GetDirectoryName(faviconFilePath));
                    
                    using (var stream = new FileStream(faviconFilePath, FileMode.Create))
                    {
                        await model.FaviconFile.CopyToAsync(stream);
                    }
                    
                    model.FaviconUrl = "/uploads/settings/" + faviconFileName;
                }
                
                // Gerçek uygulamada veritabanına kaydetme işlemleri burada yapılır
                
                TempData["SuccessMessage"] = "Genel ayarlar başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Genel ayarlar güncellenirken hata oluştu.");
                TempData["ErrorMessage"] = "Genel ayarlar güncellenirken bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Settings));
        }
        
        // Sosyal medya ve SEO ayarlarını güncelle
        [HttpPost]
        public IActionResult UpdateSocialSettings(SiteSettingsViewModel model)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            
            try
            {
                // Gerçek uygulamada veritabanına kaydetme işlemleri burada yapılır
                
                TempData["SuccessMessage"] = "Sosyal medya ve SEO ayarları başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sosyal medya ve SEO ayarları güncellenirken hata oluştu.");
                TempData["ErrorMessage"] = "Sosyal medya ve SEO ayarları güncellenirken bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Settings));
        }
        
        // Ödeme ayarlarını güncelle
        [HttpPost]
        public IActionResult UpdatePaymentSettings(SiteSettingsViewModel model)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            
            try
            {
                // Gerçek uygulamada veritabanına kaydetme işlemleri burada yapılır
                
                TempData["SuccessMessage"] = "Ödeme ayarları başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme ayarları güncellenirken hata oluştu.");
                TempData["ErrorMessage"] = "Ödeme ayarları güncellenirken bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Settings));
        }
        
        // Email ayarlarını güncelle
        [HttpPost]
        public IActionResult UpdateEmailSettings(SiteSettingsViewModel model)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            
            try
            {
                // Gerçek uygulamada veritabanına kaydetme işlemleri burada yapılır
                
                TempData["SuccessMessage"] = "Email ayarları başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email ayarları güncellenirken hata oluştu.");
                TempData["ErrorMessage"] = "Email ayarları güncellenirken bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Settings));
        }
        
        // Test email gönder
        [HttpPost]
        public IActionResult SendTestEmail(string testEmail, string testEmailSubject, string testEmailMessage)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }
            
            try
            {
                // Gerçek uygulamada email gönderme işlemleri burada yapılır
                // Burada sadece başarılı mesajı döndürüyoruz
                
                TempData["SuccessMessage"] = $"Test email başarıyla {testEmail} adresine gönderildi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test email gönderilirken hata oluştu.");
                TempData["ErrorMessage"] = "Test email gönderilirken bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Settings));
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetInt32("AdminId") != null;
        }
    }
} 