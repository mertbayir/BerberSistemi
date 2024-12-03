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

        public IActionResult ManageApp()
        {
            string staffEmail = HttpContext.Session.GetString("Email");


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

            // Randevuyu sil
            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            // Randevu listesine geri dön
            return RedirectToAction("ManageApp");
        }

        [HttpPost]
        public IActionResult ConfirmAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                appointment.Status = "Approved"; // Randevuyu onayla
                _context.SaveChanges();
            }

            return RedirectToAction("ManageApp"); // İşlemden sonra tekrar randevuları listele
        }

    }
}
