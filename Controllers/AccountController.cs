using Microsoft.AspNetCore.Mvc;
using KutuphaneProgrami.Models;
using System.Linq;

namespace KutuphaneProgrami.Controllers
{
    public class AccountController : Controller
    {
        private readonly LibraryContext _context;

        public AccountController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string name, string surname, string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Name == name && u.Surname == surname && u.Email == email && u.Password == password);
            if (user != null)
            {
                // Check if user still exists in the database
                if (_context.Users.Any(u => u.Id == user.Id))
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Error = "Bilgiler yanlış!";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string name, string surname, string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                var newUser = new User { Name = name, Surname = surname, Email = email, Password = password };
                _context.Users.Add(newUser);
                _context.SaveChanges();
                // Kayıt sonrası login sayfasına yönlendir
                return RedirectToAction("Login");
            }
            ViewBag.Error = "Bu email ile zaten kayıt var!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }
    }
} 