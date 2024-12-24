using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev.Filters;
using odev.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System;

namespace odev.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly UserContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CustomerController(UserContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.clarifai.com/v2/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Key {_configuration["Clarifai:ApiKey"]}");
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

            ViewBag.Barbers = barbers;

            return View();
        }

        [HttpPost]
        public IActionResult CreateAppointment(Appointment appointment, string Time)
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
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

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("ListApp");
        }

        [HttpGet]
        public IActionResult GetServicePrice(string barberName, string service)
        {
            var serviceDetail = _context.ServicePriceDurations
                .FirstOrDefault(s => s.BarberName == barberName && s.Service == service);

            if (serviceDetail == null)
            {
                return Json(new { success = false, message = "Hizmet bulunamadı." });
            }

            return Json(new { success = true, price = serviceDetail.Price });
        }


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
        public IActionResult HairAnalysis()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeImage(IFormFile photo)
        {
            try
            {
                using var ms = new MemoryStream();
                await photo.CopyToAsync(ms);
                var imageBytes = ms.ToArray();
                var base64Image = Convert.ToBase64String(imageBytes);

                var requestBody = new
                {
                    inputs = new[]
                    {
                new
                {
                    data = new
                    {
                        image = new
                        {
                            base64 = base64Image
                        }
                    }
                }
            }
                };

                var response = await _httpClient.PostAsJsonAsync("models/aaa03c23b3724a16a56b629203edc62c/outputs", requestBody);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                var concepts = result.outputs[0].data.concepts;

                // Yüz özelliklerini analiz et
                var features = AnalyzeFaceFeatures(concepts);

                // Öneriler oluştur
                var recommendation = GenerateStyleRecommendation(features);
                ViewBag.Analysis = recommendation;
                return View("HairAnalysisResult");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Hata: {ex.Message}";
                return View("Error");
            }
        }
        private List<string> AnalyzeFaceFeatures(dynamic concepts)
        {
            var features = new List<string>();
            bool isBald = false;
            bool hasBeard = false;

            foreach (var concept in concepts)
            {
                double value = Convert.ToDouble(concept.value);
                if (value > 0.6) // Yüksek güven seviyesine göre filtreleme
                {
                    var featureName = concept.name.ToString().ToLower();

                    if (featureName == "bald")
                    {
                        isBald = true; // Kellik durumunu işaretle
                    }
                    else if (featureName == "beard")
                    {
                        hasBeard = true; // Sakal varsa işaretle
                    }
                    else if (featureName == "no-beard")
                    {
                        hasBeard = false; // Sakal yoksa işaretle
                    }
                    else if (featureName == "young" || featureName == "adult" || featureName == "round" || featureName == "square")
                    {
                        features.Add(featureName); // Diğer yüz özelliklerini ekle
                    }
                }
            }

            // Saç ve sakal durumuna göre özellik ekleme
            if (!isBald)
            {
                features.Add("saç"); // Saçı varsa saç özelliklerini ekle
            }
            else
            {
                features.Add("kel"); // Kel ise "kel" özelliğini ekle
            }

            if (hasBeard)
            {
                features.Add("sakal"); // Sakalı varsa sakal özelliklerini ekle
            }
            else
            {
                features.Add("sakalsız"); // Sakalı yoksa "sakalsız" özelliğini ekle
            }

            return features;
        }



        private string GenerateStyleRecommendation(List<string> features)
        {
            var recommendations = new List<string>();

            if (features.Contains("kel") && !features.Contains("saç"))
            {
                recommendations.Add("Kel görünüm için modern ve temiz bir tarz önerilir.");
            }
            else if (features.Contains("saç"))
            {
                recommendations.Add("Saçlar için modern kesim, pompadour veya fade tarzları düşünülebilir.");
            }

            if (features.Contains("sakal"))
            {
                recommendations.Add("Sakal için tam sakal, kirli sakal veya düzenli bakımlı tarz önerilir.");
            }
            else if (features.Contains("sakalsız"))
            {
                recommendations.Add("Sakalsız bir görünüm için temiz yüz ve klasik erkek saç kesimi uygundur.");
            }

            return string.Join("\n", recommendations);
        }


    }
}
