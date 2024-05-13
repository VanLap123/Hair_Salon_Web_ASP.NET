using Hair_Salon_Web_ASP.NET.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Hair_Salon_Web_ASP.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index(string title)
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            }
            IEnumerable<Post> lstPost = _db.Posts.ToList();
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
           
                if (!String.IsNullOrEmpty(title))
                {
                    List<Post> lstPost1 = _db.Posts.Where(s => s.title.ToUpper().Contains(title.ToUpper())).ToList();
                    if(lstPost1.Count()!=0)               
                    {
                        return View(lstPost1);
                    }
                    else
                    {
                        ViewBag.SearchMessage = "the post is not found";
                    }
                }

            lstPost =lstPost.Reverse();
                       
            return View(lstPost.ToList());
          
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
