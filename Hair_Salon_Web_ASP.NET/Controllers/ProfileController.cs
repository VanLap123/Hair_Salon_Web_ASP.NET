using Hair_Salon_Web_ASP.NET.Models;
using Hair_Salon_Web_ASP.NET.Repository;
using Hair_Salon_Web_ASP.NET.UserFilters;
using Microsoft.AspNetCore.Mvc;

namespace Hair_Salon_Web_ASP.NET.Controllers
{
    [UserRole(UserRole.Admin, UserRole.Employee,UserRole.Client)]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RepositoryHairSalon _repo;
        public ProfileController(ApplicationDbContext db)
        {
            _db = db;
            this._repo = new RepositoryHairSalon(db);
        }
        public IActionResult ViewProfile(string phone_number)
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            User user = _db.Users.Where(u=>u.phone_number==phone_number).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        public IActionResult Edit(int user_id)
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            User user = _db.Users.Find(user_id);
            ViewBag.phone_number = user.phone_number;
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(int user_id, User user)
        {
            string phone_number = HttpContext.Session.GetString("phone_number");
            if (ModelState.IsValid)
            {
                if (phone_number!=user.phone_number)
                {
                    HttpContext.Session.SetString("phone_number", user.phone_number);
                }
                user.user_id = user_id;
                _db.Users.Update(user);
                _db.SaveChanges();
               
                return RedirectToAction("Index","Home");
            }
            return View(user);
        }
    }
}
