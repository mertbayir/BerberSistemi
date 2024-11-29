using Microsoft.AspNetCore.Mvc;
using odev.Filters;

namespace odev.Controllers
{
    [AuthFilter("Admin")]
    public class AdminController : Controller
    {
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
    }
}
