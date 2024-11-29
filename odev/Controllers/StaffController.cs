using Microsoft.AspNetCore.Mvc;
using odev.Filters;
using odev.Models;

namespace odev.Controllers
{
    public class StaffController : Controller
    {
        private readonly UserContext _context;

        public StaffController(UserContext context)
        {
            _context = context;
        }

        [AuthFilter("Staff")]
        public IActionResult Index()
        {
            ViewData["Layout"] = "~/Views/Staff/_LayoutStaff.cshtml";
            return View();
        }
        public IActionResult Staffpanel()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Manage(string barberName)
        {
            // Giriş yapan berberin Id'sine göre randevuları getir
            var appointments = _context.Appointments
                .Where(a => a.Barber.Name == barberName)
                .OrderBy(a => a.DateTime)  // Tarihe göre sıralama
                .Select(a => new
                {
                    a.Id,
                    a.Service,
                    a.DateTime,
                    CustomerName = a.User.Email // Kullanıcı ismini alıyoruz
                })
                .ToList();

            ViewBag.Appointments = appointments;
            return View();
        }
    }
}
