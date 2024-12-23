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

            var currentDate = DateTime.Now.Date;
            var currentTime = DateTime.Now.TimeOfDay;

            var appointments = _context.Appointments
                .Where(a => a.BarberName == staffEmail)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.Time)
                .ToList();

            var pastAppointments = appointments
                .Where(a => a.Date < currentDate || (a.Date == currentDate && a.Time < currentTime))
                .ToList();

            var futureAppointments = appointments
                .Where(a => a.Date > currentDate || (a.Date == currentDate && a.Time >= currentTime))
                .ToList();

            ViewBag.PastAppointments = pastAppointments;
            ViewBag.FutureAppointments = futureAppointments;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound(); 
            }

            if (appointment != null)
            {
                appointment.Status = "İptal"; 
                _context.SaveChanges();
            }

            return RedirectToAction("ManageApp");
        }

        [HttpPost]
        public IActionResult ConfirmAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                appointment.Status = "Onay"; 
                _context.SaveChanges();
            }

            return RedirectToAction("ManageApp"); 
        }

    }
}
