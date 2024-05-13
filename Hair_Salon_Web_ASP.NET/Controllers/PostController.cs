using Hair_Salon_Web_ASP.NET.Models;
using Hair_Salon_Web_ASP.NET.Repository;
using Hair_Salon_Web_ASP.NET.ServiceSaveImage;
using Hair_Salon_Web_ASP.NET.UserFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hair_Salon_Web_ASP.NET.Controllers
{
    [UserRole(UserRole.Admin)]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RepositoryHairSalon _repo;
        private readonly IFileService _fileService;
        public PostController(ApplicationDbContext db, IFileService fileService)
        {
            this._db = db;
            this._repo = new RepositoryHairSalon(db);
            this._fileService = fileService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            List<Post> posts = await _db.Posts.ToListAsync();
            return View(posts);
        }
    
        public IActionResult Create()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Post post)
        {
            if (post.ImageFile == null)
            {
                ModelState.AddModelError("post_image", "The image is required");
            }
            if (ModelState.IsValid)
            {
                post.date = DateTime.Now;
                var image = _fileService.SaveImage(post.ImageFile);
                post.post_image = image.Item2;
                _db.Add(post);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }
        public IActionResult Edit(int post_id) 
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            if (post_id == null || _db.Posts == null)
            {
                return NotFound();
            }

            Post post= _db.Posts.Find(post_id);
            if (post == null)
            {
                return NotFound();
            }
            
            return View(post);
        }
        [HttpPost]
        public IActionResult Edit(int post_id, Post post) 
        {
            if (post_id != post.post_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Post oldPost = _db.Posts.Find(post_id);
                    if (post.ImageFile == null)
                    {
                      oldPost.title=post.title;
                      oldPost.description=post.description;
                      oldPost.date=post.date;
                    }
                    else
                    {
                        var result = _fileService.SaveImage(post.ImageFile);
                        if (result.Item1 == 1)
                        {
                            var oldImage = oldPost.post_image;
                            oldPost.post_image = result.Item2;
                            _fileService.DeleteImage(oldImage);
                            oldPost.title = post.title;
                            oldPost.description = post.description;
                            oldPost.date = post.date;

                        }

                    }
                    _db.Update(oldPost);
                    _db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.post_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }
        public IActionResult Delete(int post_id)
        {
            Post post = _db.Posts.Find(post_id);
            _fileService.DeleteImage(post.post_image);
            _db.Posts.Remove(post);
                _db.SaveChanges();
            return RedirectToAction("Index");
        }
        private bool PostExists(int id)
        {
            return (_db.Posts?.Any(e => e.post_id == id)).GetValueOrDefault();
        }
    }
  
}
