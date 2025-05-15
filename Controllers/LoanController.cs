using Microsoft.AspNetCore.Mvc;
using KutuphaneProgrami.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace KutuphaneProgrami.Controllers
{
    public class LoanController : Controller
    {
        private readonly LibraryContext _context;
        public LoanController(LibraryContext context) { _context = context; }

        private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;

        public IActionResult Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var loans = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Student)
                .Where(l => l.UserId == userId)
                .ToList();
            return View(loans);
        }

        public IActionResult Create(int? bookId)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            ViewBag.Books = _context.Books.Where(b => b.IsAvailable && b.UserId == userId).ToList();
            ViewBag.Students = _context.Students.ToList();
            ViewBag.SelectedBookId = bookId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(int BookId, int StudentId, DateTime DueDate)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var book = _context.Books.FirstOrDefault(b => b.Id == BookId);
            var student = _context.Students.FirstOrDefault(s => s.Id == StudentId);
            if (book != null && student != null && book.IsAvailable)
            {
                var loan = new Loan
                {
                    BookId = BookId,
                    StudentId = StudentId,
                    LoanDate = DateTime.Now,
                    DueDate = DueDate,
                    ReturnDate = null,
                    UserId = HttpContext.Session.GetInt32("UserId") ?? 0,
                    StudentName = student.Name,
                    StudentSurname = student.Surname
                };
                book.IsAvailable = false;
                _context.Loans.Add(loan);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Books = _context.Books.Where(b => b.IsAvailable).ToList();
            ViewBag.Students = _context.Students.ToList();
            ViewBag.Error = "Kitap veya öğrenci seçimi hatalı!";
            return View();
        }

        public IActionResult Return(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var loan = _context.Loans.Include(l => l.Book).FirstOrDefault(l => l.Id == id && l.ReturnDate == null);
            if (loan != null)
            {
                if (DateTime.Now > loan.DueDate)
                {
                    TempData["Error"] = "Bu kitap son iade tarihini geçtiği için iade alınamaz!";
                    return RedirectToAction("Index");
                }
                loan.ReturnDate = DateTime.Now;
                loan.Book.IsAvailable = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            
            var loan = _context.Loans.Include(l => l.Book).FirstOrDefault(l => l.Id == id);
            if (loan != null)
            {
                if (loan.ReturnDate == null)
                {
                    loan.Book.IsAvailable = true;
                }
                _context.Loans.Remove(loan);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            
            var loan = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Student)
                .FirstOrDefault(l => l.Id == id);
            
            if (loan == null)
                return NotFound();
                
            return View(loan);
        }

        [HttpPost]
        public IActionResult Edit(int id, DateTime DueDate)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            
            var loan = _context.Loans.FirstOrDefault(l => l.Id == id);
            if (loan != null)
            {
                loan.DueDate = DueDate;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        // GEÇİCİ: Eksik Loan isimlerini doldurmak için
        [HttpPost]
        public IActionResult FixLoanStudentNames()
        {
            var loans = _context.Loans.Include(l => l.Student).Where(l => (l.StudentName == null || l.StudentName == "") && l.StudentId != null).ToList();
            foreach (var loan in loans)
            {
                if (loan.Student != null)
                {
                    loan.StudentName = loan.Student.Name;
                    loan.StudentSurname = loan.Student.Surname;
                }
            }
            _context.SaveChanges();
            return Content($"{loans.Count} kayıt güncellendi.");
        }

        // GEÇİCİ: Silinmiş öğrenciye bağlı Loan kayıtlarını düzelt
        [HttpPost]
        public IActionResult CleanInvalidStudentLoans()
        {
            var validStudentIds = _context.Students.Select(s => s.Id).ToHashSet();
            var invalidLoans = _context.Loans.Where(l => l.StudentId != null && !validStudentIds.Contains(l.StudentId.Value)).ToList();
            foreach (var loan in invalidLoans)
            {
                loan.StudentId = null;
            }
            _context.SaveChanges();
            return Content($"{invalidLoans.Count} kayıt düzeltildi.");
        }
    }
} 