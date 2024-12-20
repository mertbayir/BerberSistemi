using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using odev.Filters;
using odev.Models;

namespace odev.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly UserContext _context;

        public StaffController(UserContext context)
        {
            _context = context;
        }
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

        public IActionResult ManageApp()
        {
            string staffEmail = HttpContext.Session.GetString("UserName");


            var appointments = _context.Appointments
                         .Where(a => a.BarberName == staffEmail) // BarberName yerine uygun alanı kullanın
                         .OrderBy(a => a.Date)
                         .ThenBy(a => a.Time)
                         .ToList();

            return View(appointments);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAppointment(int id)
        {
            // Randevuyu bul
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound(); // Eğer randevu bulunamazsa hata döndür
            }

            if (appointment != null)
            {
                appointment.Status = "İptal"; // Randevuyu onayla
                _context.SaveChanges();
            }

            // Randevu listesine geri dön
            return RedirectToAction("ManageApp");
        }

        [HttpPost]
        public IActionResult ConfirmAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                appointment.Status = "Onay"; // Randevuyu onayla
                _context.SaveChanges();
            }

            return RedirectToAction("ManageApp"); // İşlemden sonra tekrar randevuları listele
        }

    }
}
