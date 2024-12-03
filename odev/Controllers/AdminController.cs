using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev.Filters;
using odev.Models;

namespace odev.Controllers
{
    [AuthFilter("Admin")]
    public class AdminController : Controller
    {
        private readonly UserContext _context;

        public AdminController(UserContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewData["Layout"] = "~/Views/Admin/_LayoutAdmin.cshtml";
            return View();
        }
        public IActionResult Adminpanel()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AddBarber()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBarber(Barber barber)
        {
            if (ModelState.IsValid)
            {
                _context.Barbers.Add(barber);
                _context.SaveChanges();
                return RedirectToAction("Adminpanel", "Admin"); // Admin paneline geri döner
            }
            return View(barber); // Hatalı giriş varsa yeniden göster
        }
    }
}
