using Microsoft.AspNetCore.Mvc;
using KutuphaneProgrami.Models;
using System.Linq;

namespace KutuphaneProgrami.Controllers
{
    public class StudentController : Controller
    {
        private readonly LibraryContext _context;
        public StudentController(LibraryContext context) { _context = context; }

        private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;

        public IActionResult Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var students = _context.Students
                .Where(s => s.UserId == userId)
                .Select(s => new StudentListVM
                {
                    Id = s.Id,
                    Name = s.Name,
                    Surname = s.Surname,
                    Number = s.Number,
                    ClassName = s.Class != null ? s.Class.Name : "-",
                    IsActive = s.IsActive
                }).ToList();
            return View(students);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            ViewBag.Classes = _context.Classes.Where(c => c.UserId == userId).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                student.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                _context.Students.Add(student);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Classes = _context.Classes.Where(c => c.UserId == HttpContext.Session.GetInt32("UserId")).ToList();
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(student);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                var loans = _context.Loans.Where(l => l.StudentId == id).ToList();
                foreach (var loan in loans)
                {
                    loan.StudentId = null;
                }
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                return NotFound();
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            ViewBag.Classes = _context.Classes.Where(c => c.UserId == userId).ToList();
            return View(student);
        }

        [HttpPost]
        public IActionResult Edit(Student student)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                student.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                _context.Students.Update(student);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Classes = _context.Classes.Where(c => c.UserId == HttpContext.Session.GetInt32("UserId")).ToList();
            return View(student);
        }
    }

    public class StudentListVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Number { get; set; }
        public string ClassName { get; set; }
        public bool IsActive { get; set; }
    }
} 