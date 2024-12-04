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
                    Email = model.User.Email, // Berber adı
                    Password = model.User.Password, // Şifreyi istediğiniz gibi belirleyebilirsiniz
                    Role = "Staff" // Kullanıcı rolü
                };

                _context.Users.Add(user);
                _context.SaveChanges();
;


                return RedirectToAction("Adminpanel", "Admin"); // Admin paneline geri döner
            }
            return View(model); // Hatalı giriş varsa yeniden göster
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
    }
}
