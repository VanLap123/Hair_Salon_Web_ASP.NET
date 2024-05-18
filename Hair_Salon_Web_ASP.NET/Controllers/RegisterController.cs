
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
            if (user.password.Length <8)
            {

                ModelState.AddModelError("password", "Password must be at least 8 characters ");
            }

            user.role = "Client";
            if (ModelState.IsValid)
            {        
                        _repo.CreateUser(user);
                        return RedirectToAction("Index", "Login");

            }
            return View(user);
            
        }
    }
}
