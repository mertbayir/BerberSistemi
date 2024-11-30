using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev.Models;

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
                return RedirectToAction("Customerpanel","Customer");
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

            if (email == "mert" && password == "sau")
            {
                HttpContext.Session.SetString("Email", email);
                return RedirectToAction("Adminpanel","Admin"); 
            }
            else if (email == "ahmet" && password == "sau")
            {
                HttpContext.Session.SetString("Email", email);
                return RedirectToAction("Staffpanel","Staff");
            }
            else
            {
                var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user != null)
                {
                    // Kullanıcı bulunduysa, session'a giriş bilgisini kaydet
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("Role", user.Role);

                    // Kullanıcıyı başarılı bir şekilde yönlendir
                    return RedirectToAction("Customerpanel", "Customer");
                }
                else
                {
                    // Hatalı giriş bilgisi
                    ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı.";
                    return RedirectToAction("Login","User");
                }

            }
        }


    }
}
