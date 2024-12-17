using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev.Models;
using odev.Filters;

namespace odev.Controllers
{
    public class UserController : Controller
    {
        private readonly UserContext _context;
        public UserController(UserContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcıyı ekle
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Customerpanel", "Customer");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error); // Konsola yaz
                }

                TempData["ErrorMessage"] = "Kayıt sırasında bir hata oluştu. Lütfen tekrar deneyin.";
                return View();
            }
        }


        public IActionResult Adminpanel()
        {
            return View();
        }

        public IActionResult Staffpanel()
        {
            return View();
        }

        public IActionResult Customerpanel()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
        .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Kullanıcı bulunduysa, role bilgisini session'a kaydet
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("Role", user.Role);  // Kullanıcının rolünü kaydediyoruz

                TempData["UserName"] = user.UserName;

                // Role göre yönlendirme yap
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Adminpanel", "Admin");
                }
                else if (user.Role == "Staff")
                {
                    return RedirectToAction("Staffpanel", "Staff");
                }
                else if (user.Role == "Customer")
                {
                    return RedirectToAction("Customerpanel", "Customer");
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı.";
                return View();
            }
        }
    }
}
