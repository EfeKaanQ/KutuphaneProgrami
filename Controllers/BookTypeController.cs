using Microsoft.AspNetCore.Mvc;
using KutuphaneProgrami.Models;
using System.Linq;

namespace KutuphaneProgrami.Controllers
{
    public class BookTypeController : Controller
    {
        private readonly LibraryContext _context;
        public BookTypeController(LibraryContext context) { _context = context; }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");
            var types = _context.BookTypes.ToList();
            return View(types);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult Create(BookType type)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                _context.BookTypes.Add(type);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(type);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();
            var type = _context.BookTypes.FirstOrDefault(x => x.Id == id);
            if (type == null) return NotFound();
            return View(type);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var type = _context.BookTypes.FirstOrDefault(x => x.Id == id);
            if (type == null) return NotFound();
            _context.BookTypes.Remove(type);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
} 