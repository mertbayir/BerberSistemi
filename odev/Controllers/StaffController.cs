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

    }
}
