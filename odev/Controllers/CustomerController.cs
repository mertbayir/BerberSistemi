using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev.Filters;
using odev.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace odev.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly UserContext _context;

        public CustomerController(UserContext context)
        {
            _context = context;

        }

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
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account"); // Kullanıcı giriş yapmamışsa
            }


            var today = DateTime.Now;

            if (appointment.Date < today || appointment.Date > today.AddDays(30))
            {
                TempData["AppErr"] = "Randevu 30 Gün İçinden Herhangi Bir Güne Alınabilir.";
                return RedirectToAction("CreateAppointment");
            }

            if (appointment.Date.DayOfWeek == DayOfWeek.Saturday || appointment.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                TempData["AppErr"] = "Haftasonu Randevu Alınamaz.";
                return RedirectToAction("CreateAppointment");
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


            var barberService = _context.ServicePriceDurations
                              .Where(s => s.BarberName == appointment.BarberName && s.Service == appointment.Service)
                              .FirstOrDefault();

            if (barberService != null)
            {
                appointment.Price = barberService.Price;
                appointment.Duration = barberService.Duration;
            }


            appointment.UserName = userName; 
            appointment.Status = "Beklemede";   

            // Randevuyu kaydet
            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("ListApp");
        }

        // Kullanıcının aldığı randevuları listeleme
        public IActionResult ListApp()
        {
            var userName = HttpContext.Session.GetString("UserName");

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
        [HttpGet]
        public IActionResult OneriAl()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OneriAl(IFormFile photo)
        {
            return View();
        }

    }
}
