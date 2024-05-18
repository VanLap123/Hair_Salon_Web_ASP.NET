using Microsoft.AspNetCore.Mvc;

namespace Hair_Salon_Web_ASP.NET.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            return View();
        }
        public IActionResult NotFound() 
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            return View();
        }
    }
}
