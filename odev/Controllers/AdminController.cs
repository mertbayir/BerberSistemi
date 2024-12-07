using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev.Filters;
using odev.Models;
using System.Reflection.Metadata;

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
            var model = new BarberUserViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult AddBarber(BarberUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Barbers.Add(model.Barber);
                _context.SaveChanges();


                var user = new User
                {
                    Email = model.User.Email,
                    Password = model.User.Password, 
                    UserName = model.User.UserName,
                    Role = "Staff" 
                };

                _context.Users.Add(user);
                _context.SaveChanges();
                ;


                return RedirectToAction("Adminpanel", "Admin"); // Admin paneline geri döner
            }
            else
            {
                return View(model); // Hatalı giriş varsa yeniden göster
            }
        }

        public IActionResult Manage()
        {
            var users = _context.Users.ToList(); // Veritabanından tüm kullanıcıları al
            return View(users);
        }

        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id); // Kullanıcıyı id'ye göre bul
            if (user != null)
            {
                _context.Users.Remove(user); // Kullanıcıyı sil
                _context.SaveChanges(); // Değişiklikleri kaydet
            }

            return RedirectToAction("Manage"); // Yönetim sayfasına geri dön
        }

        public IActionResult BarberEarnings()
        {
            var barberEarnings = _context.Appointments
         .GroupBy(a => new { a.BarberName, Date = a.Date.Date }) // Berber ve tarihi gruplandır
         .Select(g => new
         {
             BarberName = g.Key.BarberName,
             Date = g.Key.Date,
             TotalEarnings = g.Sum(a => a.Price),
             AppointmentsCount = g.Count()
         })
         .ToList()
         .GroupBy(a => a.BarberName)
         .Select(g => new BarberDailyEarningsViewModel
         {
             BarberName = g.Key,
             DailyEarnings = g.Select(x => new DailyEarnings
             {
                 Date = x.Date,
                 TotalEarnings = x.TotalEarnings
             }).ToList(),
             TotalAppointments = g.Sum(x => x.AppointmentsCount), // Toplam randevu sayısı
             TotalEarnings = g.Sum(x => x.TotalEarnings) // Toplam kazanç
         })
         .ToList();

            return View(barberEarnings);

        }
    }
}
