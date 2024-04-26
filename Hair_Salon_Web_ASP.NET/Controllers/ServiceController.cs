using Hair_Salon_Web_ASP.NET.Models;
using Hair_Salon_Web_ASP.NET.ServiceSaveImage;
using Hair_Salon_Web_ASP.NET.UserFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Hair_Salon_Web_ASP.NET.Controllers
{
   [UserRole(UserRole.Admin)]
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileService _fileService;
        public ServiceController(ApplicationDbContext db,IFileService fileService)
        {
            this._db = db;
            this._fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            List<Service> services = await _db.Services.ToListAsync();
            return View(services);
        }
        public IActionResult Create()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Service service)
        {
            if (ModelState.IsValid)
            {
                var image = _fileService.SaveImage(service.ImageFile);
                service.ser_image = image.Item2;
                _db.Add(service);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        public IActionResult Edit( int ser_id)
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            Service service = _db.Services.Find(ser_id);

            if (service == null)
            {
                return NotFound();
            }
            return View(service);
          
        }
        [HttpPost]
        public IActionResult Edit(int ser_id, Service service)
        {
            try
            {
                Service oldService = _db.Services.Find(ser_id);
                if (service.ImageFile == null)
                {
                    oldService.name = service.name;
                    oldService.price = service.price;
                    oldService.time_to_cut = service.time_to_cut;
                    oldService.description = service.description;
                }
                else
                {
                    var result = _fileService.SaveImage(service.ImageFile);
                    if (result.Item1 == 1)
                    {
                        var oldImage = oldService.ser_image;
                        oldService.ser_image = result.Item2;
                        _fileService.DeleteImage(oldImage);
                        oldService.name = service.name;
                        oldService.price = service.price;
                        oldService.time_to_cut = service.time_to_cut;
                        oldService.description = service.description;
                    }

                }

                _db.Services.Update(oldService);
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(service.ser_id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Redirect sau khi cập nhật thành công
            return RedirectToAction(nameof(Index));
        }


        private bool ServiceExists(int id)
        {
            return (_db.Services?.Any(e => e.ser_id == id)).GetValueOrDefault();
        }
        public IActionResult Delete(int ser_id)
        {
            Service obj = _db.Services.Find(ser_id);
            if (obj != null)
            {
                _db.Services.Remove(obj);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
