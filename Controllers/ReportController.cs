using Microsoft.AspNetCore.Mvc;
using KutuphaneProgrami.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace KutuphaneProgrami.Controllers
{
    public class ReportController : Controller
    {
        private readonly LibraryContext _context;
        public ReportController(LibraryContext context) { _context = context; }

        private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;

        [HttpGet]
        [Route("Report/Index")]
        public IActionResult Index(int? classId, int? bookId, string studentName, string[] filterType, bool? undeliveredOnly)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            IEnumerable<Loan> loans = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Student)
                .ThenInclude(s => s.Class)
                .Where(l => l.UserId == userId);

            if (classId.HasValue && classId.Value > 0)
                loans = loans.Where(l => (l.Student != null && l.Student.ClassId == classId.Value));
            if (bookId.HasValue && bookId.Value > 0)
                loans = loans.Where(l => l.BookId == bookId.Value);
            if (!string.IsNullOrWhiteSpace(studentName))
                loans = loans.Where(l =>
                    (l.Student != null
                        ? (l.Student.Name + " " + l.Student.Surname)
                        : (l.StudentName + " " + l.StudentSurname)
                    ).Contains(studentName)
                );

            if (undeliveredOnly == true)
            {
                loans = loans.Where(l => l.ReturnDate == null);
            }

            if (filterType != null && filterType.Contains("mostBorrowed"))
            {
                var grouped = loans.GroupBy(l => new {
                        StudentId = l.StudentId,
                        UserName = l.Student != null ? l.Student.Name + " " + l.Student.Surname : (l.StudentName + " " + l.StudentSurname)
                    })
                    .OrderByDescending(g => g.Count())
                    .ToList();

                var reportGroups = grouped.Select(g => new MostBorrowedReportGroupVM {
                    UserName = g.Key.UserName ?? g.Select(l => l.StudentName + " " + l.StudentSurname).FirstOrDefault(),
                    BookCount = g.Count(),
                    Books = g.Select(l => new LoanReportVM {
                        BookTitle = l.Book.Title,
                        UserName = l.Student != null ? l.Student.Name + " " + l.Student.Surname : (l.StudentName + " " + l.StudentSurname),
                        LoanDate = l.LoanDate,
                        ReturnDate = l.ReturnDate,
                        LastReturnDate = l.DueDate
                    }).ToList()
                }).ToList();

                ViewBag.MostBorrowed = true;
                ViewBag.FilterType = filterType;
                ViewBag.UndeliveredOnly = undeliveredOnly;
                return View("Index", reportGroups);
            }
            else if (filterType != null && filterType.Contains("mostRead"))
            {
                var readLoans = loans.Where(l => l.ReturnDate != null);
                var grouped = readLoans.GroupBy(l => new {
                        StudentId = l.StudentId,
                        UserName = l.Student != null ? l.Student.Name + " " + l.Student.Surname : (l.StudentName + " " + l.StudentSurname)
                    })
                    .OrderByDescending(g => g.Count())
                    .ToList();

                var reportGroups = grouped.Select(g => new MostBorrowedReportGroupVM {
                    UserName = g.Key.UserName ?? g.Select(l => l.StudentName + " " + l.StudentSurname).FirstOrDefault(),
                    BookCount = g.Count(),
                    Books = g.Select(l => new LoanReportVM {
                        BookTitle = l.Book.Title,
                        UserName = l.Student != null ? l.Student.Name + " " + l.Student.Surname : (l.StudentName + " " + l.StudentSurname),
                        LoanDate = l.LoanDate,
                        ReturnDate = l.ReturnDate,
                        LastReturnDate = l.DueDate
                    }).ToList()
                }).ToList();

                ViewBag.MostRead = true;
                ViewBag.FilterType = filterType;
                ViewBag.UndeliveredOnly = undeliveredOnly;
                return View("Index", reportGroups);
            }
            else if (filterType != null && filterType.Contains("leastReturnDays"))
            {
                loans = loans.Where(l => l.ReturnDate == null)
                             .OrderBy(l => (l.DueDate - DateTime.Now).TotalDays)
                             .ThenBy(l => l.DueDate);
            }

            var reports = loans.Select(l => new LoanReportVM
            {
                BookTitle = l.Book.Title,
                UserName = l.Student != null ? l.Student.Name + " " + l.Student.Surname : (l.StudentName + " " + l.StudentSurname),
                LoanDate = l.LoanDate,
                ReturnDate = l.ReturnDate,
                LastReturnDate = l.DueDate
            }).ToList();

            ViewBag.Classes = _context.Classes.Where(c => c.UserId == userId).ToList();
            ViewBag.Books = _context.Books.Where(b => b.UserId == userId).ToList();
            ViewBag.SelectedClassId = classId;
            ViewBag.SelectedBookId = bookId;
            ViewBag.StudentName = studentName;
            ViewBag.FilterType = filterType;
            ViewBag.UndeliveredOnly = undeliveredOnly;
            return View(reports);
        }
    }

    public class LoanReportVM
    {
        public string BookTitle { get; set; }
        public string UserName { get; set; }
        public System.DateTime LoanDate { get; set; }
        public System.DateTime? ReturnDate { get; set; }
        public System.DateTime LastReturnDate { get; set; }
    }

    public class MostBorrowedReportGroupVM
    {
        public string UserName { get; set; }
        public int BookCount { get; set; }
        public List<LoanReportVM> Books { get; set; }
    }
} 