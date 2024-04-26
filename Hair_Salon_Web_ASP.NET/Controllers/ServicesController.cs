using Hair_Salon_Web_ASP.NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hair_Salon_Web_ASP.NET.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ServicesController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            List<Service> services = await _db.Services.ToListAsync();
            return View(services);
            
        }
    }
}
