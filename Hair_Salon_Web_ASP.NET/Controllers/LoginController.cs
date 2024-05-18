
using Hair_Salon_Web_ASP.NET.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hair_Salon_Web_ASP.NET.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LoginController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (TempData["LoginMessage"] != null)
            {
                ViewData["LoginMessage"] = TempData["LoginMessage"];
            }
            return View();
        }

       [HttpPost]
        public async Task<IActionResult> Login(string phone_number, string password)
        {
            User obj = _db.Users.FirstOrDefault(x => x.phone_number == phone_number);

            if (obj != null)
            {
                int costParameter = 12;
                
                bool isValid = BCrypt.Net.BCrypt.Verify(password,obj.password);
               
                if (isValid)
                {

                    HttpContext.Session.SetInt32("user_id", obj.user_id);
                    HttpContext.Session.SetString("name", obj.name);
                    HttpContext.Session.SetString("phone_number", obj.phone_number);
                    HttpContext.Session.SetString("role", obj.role);
                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid phone number or password");
                    return View("Index");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid phone number or password");
                return View("Index");
            }
        }

        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string phone_number, string password,string new_password)
        {
           
            User obj = _db.Users.FirstOrDefault(x => x.phone_number == phone_number);
            if (string.IsNullOrEmpty(new_password))
            {
                ViewBag.new_password = "New password can not empty";
                return View();
            }
            if (new_password.Length < 8)
            {

                ViewBag.new_password = "Password must be at least 8 characters";
                return View();
            }
            if (obj != null)
            {
                int costParameter = 12;

                bool isValid = BCrypt.Net.BCrypt.Verify(password, obj.password);

                if (isValid)
                {
                   
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(new_password, costParameter);
                    obj.password = hashedPassword.ToString();
                    _db.Users.Update(obj);
                    _db.SaveChanges();
                    return View("Index");
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid phone number or password");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid phone number or password");
                return View();
            }
            

        }


        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
