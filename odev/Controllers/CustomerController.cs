using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev.Filters;
using odev.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace odev.Controllers
{
    [AuthFilter("Customer")]
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Layout"] = "~/Views/Customer/_LayoutCustomer.cshtml";
            return View();
        }
        public IActionResult Appointment()
        {
            return View();
        }
        public IActionResult Customerpanel()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private readonly UserContext _context;

        public CustomerController(UserContext context)
        {
            _context = context;


        }
        [HttpGet]
        public IActionResult CreateAppointment()
        {
              var barbers = _context.Barbers.Select(b => new
        {
            b.Id,
            b.Name,
            b.Skills
        }).ToList();

        // Veriyi View'a gönder
        ViewBag.Barbers = barbers;

        return View();
        }

        // Create Appointment - POST Method
        [HttpPost]
        public IActionResult CreateAppointment(Appointment appointment,string Time)
        {
            var userName = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account"); // Kullanıcı giriş yapmamışsa
            }

            // Çakışma kontrolü
            bool isAvailable = !_context.Appointments.Any(a =>
                a.BarberName == appointment.BarberName &&
                a.Date == appointment.Date &&
                a.Time == appointment.Time);

            if (!isAvailable)
            {
                ModelState.AddModelError("", "Seçilen berber için bu saat dolu.");
                ViewBag.Barbers = _context.Barbers.ToList();
                ViewBag.Times = new List<string> { "09:00", "10:30", "12:00", "14:00", "15:30", "17:00" };
                return View(appointment);
            }

            var existingAppointment = _context.Appointments
                                                     .Where(a => a.UserName == appointment.UserName
                                                                 && a.Date.Date == appointment.Date.Date) // Aynı gün randevu
                                                     .FirstOrDefault();

            if (existingAppointment != null)
            {
                // Eğer aynı günde zaten bir randevu varsa, hata mesajı göster
                ModelState.AddModelError("", "Bu gün zaten bir randevunuz var. Lütfen başka bir tarih seçin.");
                return RedirectToAction("Customerpanel"); // Hata ile geri dön
            }

            if (appointment.Date.DayOfWeek == DayOfWeek.Saturday || appointment.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                ModelState.AddModelError("", "Hafta sonu randevu almanız mümkün değildir.");
                return View(appointment); // Hata ile geri dön
            }


            appointment.UserName = userName; 
            appointment.Status = "Valid";   

            // Randevuyu kaydet
            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("ListApp");
        }

        // Kullanıcının aldığı randevuları listeleme
        public IActionResult ListApp()
        {
            var userName = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "User");
            }

            var appointments = _context.Appointments
                .Where(a => a.UserName == userName)
                .ToList();

            return View(appointments);
        }

        [HttpPost]
        public IActionResult DeleteAppointment(int id)
        {
            // İlgili randevuyu bul
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);

            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
                TempData["Message"] = "Randevu başarıyla silindi.";
            }
            else
            {
                TempData["Error"] = "Randevu bulunamadı.";
            }

            return RedirectToAction("ListApp");
        }
    }
}
