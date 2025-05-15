using Microsoft.AspNetCore.Mvc;
using KutuphaneProgrami.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace KutuphaneProgrami.Controllers
{
    public class BookController : Controller
    {
        private readonly LibraryContext _context;

        public BookController(LibraryContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        public IActionResult Index(bool? showUndelivered)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            
            var query = _context.Books
                .Include(b => b.BookType)
                .Where(b => b.UserId == userId);

            if (showUndelivered == true)
            {
                query = query.Where(b => !b.IsAvailable);
            }

            var books = query.Select(b => new BookListVM
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                IsAvailable = b.IsAvailable,
                BookTypeName = b.BookType != null ? b.BookType.Name : "-"
            }).ToList();

            ViewBag.ShowUndelivered = showUndelivered;
            return View(books);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            ViewBag.BookTypes = _context.BookTypes.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                book.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.BookTypes = _context.BookTypes.ToList();
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(book);
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();
            ViewBag.BookTypes = _context.BookTypes.ToList();
            return View(book);
        }

        [HttpPost]
        public IActionResult Edit(Book book)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                book.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                _context.Books.Update(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.BookTypes = _context.BookTypes.ToList();
            return View(book);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var book = _context.Books
                .Include(b => b.Loans)
                .FirstOrDefault(b => b.Id == id);

            if (book != null)
            {
                // Check if there are any active loans for this book
                var hasActiveLoans = book.Loans.Any(l => l.ReturnDate == null);
                if (hasActiveLoans)
                {
                    TempData["Error"] = "Bu kitap şu anda ödünç verilmiş durumda. Önce iade alınması gerekiyor.";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    // First delete all loan records for this book
                    var loans = _context.Loans.Where(l => l.BookId == id);
                    _context.Loans.RemoveRange(loans);
                    _context.SaveChanges();

                    // Then delete the book
                    _context.Books.Remove(book);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Kitap silinirken bir hata oluştu: " + ex.Message;
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }

    public class BookListVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public bool IsAvailable { get; set; }
        public string BookTypeName { get; set; }
    }
} 