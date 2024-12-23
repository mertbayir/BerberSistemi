﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using odev.Filters;
using odev.Models;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace odev.Controllers
{
    [Authorize(Roles ="Admin")]
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


                return RedirectToAction("Adminpanel", "Admin"); 
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult Manage()
        {
            var users = _context.Users.ToList(); 
            return View(users);
        }

        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user); 
                _context.SaveChanges(); 
            }

            return RedirectToAction("Manage");
        }

        public IActionResult BarberEarnings()
        {
            var barberEarnings = _context.Appointments
         .GroupBy(a => new { a.BarberName, Date = a.Date.Date }) 
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
             TotalAppointments = g.Sum(x => x.AppointmentsCount), 
             TotalEarnings = g.Sum(x => x.TotalEarnings) 
         })
         .ToList();

            return View(barberEarnings);

        }

    }
}
