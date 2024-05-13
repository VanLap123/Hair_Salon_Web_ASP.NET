using Hair_Salon_Web_ASP.NET.Models;
using Hair_Salon_Web_ASP.NET.Repository;
using Hair_Salon_Web_ASP.NET.UserFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hair_Salon_Web_ASP.NET.Controllers
{
    [UserRole(UserRole.Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RepositoryHairSalon _repo;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
            this._repo = new RepositoryHairSalon(db);
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            List<User> users = await _db.Users.ToListAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            return View();
        }
        [HttpPost]
        public IActionResult Create(User user)
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            User obj = _db.Users.SingleOrDefault(u => u.phone_number == user.phone_number);
            if (string.IsNullOrEmpty(user.name))
            {
                ModelState.AddModelError("name", "Name is required");
            }

            if (string.IsNullOrEmpty(user.address))
            {
                ModelState.AddModelError("address", "Address is required");
            }

            if (obj != null)
            {
                ModelState.AddModelError("phone_number", "Phone number was Registered");

            }
            if (user.password != user.ConfirmPassword)
            {

                ModelState.AddModelError("ConfirmPassword", "Password and Confirm Password is not match");
            }

            if (ModelState.IsValid)
            {
               
                        _repo.CreateUser(user);
                        return RedirectToAction("Index");

            }
            return View(user);
        }

        public IActionResult Edit(int user_id)
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            User user = _db.Users.Find(user_id);
           
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(int user_id,User user)
        {

            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");

            User obj = _db.Users.SingleOrDefault(u => u.phone_number == user.phone_number);
            if (string.IsNullOrEmpty(user.name))
            {
                ModelState.AddModelError("name", "Name is required");
            }

            if (string.IsNullOrEmpty(user.address))
            {
                ModelState.AddModelError("address", "Address is required");
            }

           /* if (obj != null)
            {
                ModelState.AddModelError("phone_number", "Phone number was Registered");

            }*/

            if (ModelState.IsValid)
            {
                int costParameter = 12;
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password, costParameter);
                user.password = hashedPassword;
                user.user_id = user_id;
                _db.Users.Update(user);
                 _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }
        public IActionResult Delete(int user_id)
        {
            User obj = _db.Users.Find(user_id);
            if (obj != null)
            {
                _db.Users.Remove(obj);
                _db.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }
    }
}
