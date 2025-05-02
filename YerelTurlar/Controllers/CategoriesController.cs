using Microsoft.AspNetCore.Mvc;

namespace YerelTurlar.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            // Kategori ID'sini ViewBag'e ekle
            ViewBag.CategoryId = id;

            return View();
        }
    }
} 