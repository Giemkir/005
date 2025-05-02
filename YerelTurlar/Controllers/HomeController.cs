using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YerelTurlar.Models;

namespace YerelTurlar.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult AboutUs()
    {
        return RedirectToAction("About");
    }

    public IActionResult Tours()
    {
        return View();
    }

    public IActionResult Destinations()
    {
        return View();
    }

    public IActionResult Guides()
    {
        return View();
    }

    public IActionResult GuideDetail(int id)
    {
        // Burada id parametresine göre rehber bilgilerini getirme işlemi yapılacak
        // Şimdilik statik bir sayfa gösteriyoruz
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(ContactViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Burada form verilerini işleme kodları eklenecek
            // Örneğin: e-posta gönderme, veritabanına kaydetme vb.
            
            // İşlem başarılı olduğunda kullanıcıya bilgi mesajı gösterme
            TempData["SuccessMessage"] = "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız.";
            return RedirectToAction(nameof(Contact));
        }
        
        return View(model);
    }

    public IActionResult ThankYou()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Subscribe(NewsletterSubscription model)
    {
        if (ModelState.IsValid)
        {
            // Burada abonelik verilerini işleme kodları eklenecek
            // Örneğin: veritabanına kaydetme, e-posta gönderme vb.
            
            // Şimdilik sadece log kaydı oluşturuyoruz
            _logger.LogInformation($"Yeni newsletter aboneliği: {model.Email}");
            
            // İşlem başarılı olduğunda kullanıcıya bilgi mesajı gösterme
            TempData["SubscribeSuccess"] = "E-bülten aboneliğiniz başarıyla oluşturuldu. Teşekkür ederiz!";
            
            // Kullanıcıyı geldiği sayfaya geri yönlendirme
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }
        
        // Hata durumunda kullanıcıya bilgi mesajı gösterme
        TempData["SubscribeError"] = "Abonelik işlemi sırasında bir hata oluştu. Lütfen tekrar deneyiniz.";
        return Redirect(Request.Headers["Referer"].ToString() ?? "/");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
