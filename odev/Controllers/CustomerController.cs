using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev.Filters;
using odev.Models;

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
            // Berber listesi (sabit veri)
            var barbers = new List<dynamic>
    {
        new { Id = 1, Name = "Mert Bayır", Services = new List<string> { "Saç Kesimi", "Saç Boyama" } },
        new { Id = 2, Name = "Ahmet Sönmez", Services = new List<string> { "Saç Kesimi", "Sakal Kesimi", "Cilt Bakımı" } },
        new { Id = 3, Name = "Hakan Bilgili", Services = new List<string> { "Tasarım Tıraşı", "Çocuk Tıraşı", "Damat Tıraşı" } }
    };

            // Sabit randevu saatleri
            var availableTimes = new List<string>
    {
        "09:00", "10:30", "12:00", "14:00", "15:30", "17:00"
    };

            ViewBag.Barbers = barbers;
            ViewBag.AvailableTimes = availableTimes;

            return View();
        }

        // 2. Randevu oluşturma işlemi
        [HttpPost]
        public async Task<IActionResult> CreateAppointment(int barberId, string service, DateTime dateTime)
        {
            var userId = HttpContext.Session.GetInt32("UserId");  // Giriş yapan kullanıcının Id'sini al
            if (userId == null)
            {
                return RedirectToAction("Customerpanel", "Customer");  // Giriş yapılmadıysa Login sayfasına yönlendir
            }

            // Yeni bir randevu oluştur
            var appointment = new Appointment
            {
                UserId = userId.Value,
                BarberId = barberId,
                Service = service,
                DateTime = dateTime
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("AppointmentConfirmation");
        }

        // 3. Randevu onayı ekranı
        public IActionResult AppointmentConfirmation()
        {
            return View();
        }

    }
}
