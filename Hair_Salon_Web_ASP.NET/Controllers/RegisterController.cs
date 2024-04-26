
using Hair_Salon_Web_ASP.NET.Models;
using Hair_Salon_Web_ASP.NET.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Hair_Salon_Web_ASP.NET.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RepositoryHairSalon _repo;
        public RegisterController(ApplicationDbContext db)
        {
            this._db = db;
            this._repo = new RepositoryHairSalon(db);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(User user)
        {
            
            user.role = "Client";
            if (ModelState.IsValid)
            {
                User obj = _db.Users.SingleOrDefault(u => u.phone_number == user.phone_number);
                if (obj == null)
                {
                    if (user.password == user.ConfirmPassword)
                    {
                        
                        _repo.CreateUser(user);
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError("ConfirmPassword", "Passwords must match.");
                        return View(user);
                    }

                }
                else
                {
                   
                    ModelState.AddModelError("phone_number", "Phone number was Registered");
                }
               
            }
            return View(user);
            
        }
    }
}
