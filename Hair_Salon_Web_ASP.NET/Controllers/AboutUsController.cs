using Microsoft.AspNetCore.Mvc;

namespace Hair_Salon_Web_ASP.NET.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            return View();
        }
        public IActionResult Terms()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            return View();
        }
    }
}
