using Microsoft.AspNetCore.Mvc;
using KutuphaneProgrami.Models;
using System.Linq;

namespace KutuphaneProgrami.Controllers
{
    public class ClassController : Controller
    {
        private readonly LibraryContext _context;
        public ClassController(LibraryContext context) { _context = context; }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var classes = _context.Classes.Where(c => c.UserId == userId).ToList();
            return View(classes);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Class cls)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                cls.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                _context.Classes.Add(cls);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cls);
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");
            
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var cls = _context.Classes.FirstOrDefault(c => c.Id == id && c.UserId == userId);
            
            if (cls == null)
                return NotFound();
                
            return View(cls);
        }

        [HttpPost]
        public IActionResult Edit(Class cls)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");
            
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                cls.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                _context.Classes.Update(cls);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cls);
        }

        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");
            
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var cls = _context.Classes.FirstOrDefault(c => c.Id == id && c.UserId == userId);
            
            if (cls != null)
            {
                _context.Classes.Remove(cls);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
} 