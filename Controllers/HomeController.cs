using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KutuphaneProgrami.Models;
using System.Linq;

namespace KutuphaneProgrami.Controllers;

public class HomeController : Controller
{
    private readonly LibraryContext _context;
    public HomeController(LibraryContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Account");
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        ViewBag.UserName = user != null ? user.Name + " " + user.Surname : "Kullanıcı";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
