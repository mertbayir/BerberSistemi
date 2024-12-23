using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev.Filters;
using odev.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace odev.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly UserContext _context;

        public CustomerController(UserContext context)
        {
            _context = context;

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
        public IActionResult CreateAppointment(Appointment appointment,string Time)
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
        public async Task<IActionResult> UploadPhoto()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("Lütfen bir fotoğraf yükleyin.");
            }

            using var memoryStream = new MemoryStream();
            await photo.CopyToAsync(memoryStream);
            var photoBytes = memoryStream.ToArray();

            try
            {
                var apiResult = await CallFacePlusPlusAPI(photoBytes);
                var suggestions = ProcessFacePlusPlusResult(apiResult);

                return View("Suggestions", suggestions);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata oluştu: {ex.Message}");
            }
        }

        private async Task<string> CallFacePlusPlusAPI(byte[] photoBytes)
        {
            var apiKey = "nX2zV78ns8oJ_UGTJyjOPAwBnayEo777"; 
            var apiSecret = "vLYzT67UMwc_7XT0X5H8EbeKp_N2SqFm"; 

            var httpClient = new HttpClient();

            var content = new MultipartFormDataContent
            {
                { new StringContent(apiKey), "api_key" },
                { new StringContent(apiSecret), "api_secret" },
                { new ByteArrayContent(photoBytes), "image_file", "photo.jpg" }
            };

            var response = await httpClient.PostAsync("https://api-us.faceplusplus.com/facepp/v3/detect", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API çağrısı başarısız oldu: {response.StatusCode}, Yanıt: {responseContent}");
            }

            return responseContent;
        }

        private List<string> ProcessFacePlusPlusResult(string apiResult)
        {
            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(apiResult);

            if (jsonResult.ContainsKey("faces") && jsonResult["faces"] is Newtonsoft.Json.Linq.JArray faces && faces.Count > 0)
            {
                var suggestions = new List<string>();

                foreach (var face in faces)
                {
                    var attributes = face["attributes"];
                    var gender = attributes["gender"]["value"].ToString();
                    var age = int.Parse(attributes["age"]["value"].ToString());

                    if (gender == "Male")
                    {
                        suggestions.Add(age < 30 ? "Kısa saç ve hafif sakal" : "Orta uzunlukta saç ve yoğun sakal");
                    }
                    else
                    {
                        suggestions.Add("Saç şeklinizi koruyun.");
                    }
                }

                return suggestions;
            }

            return new List<string> { "Hiçbir yüz algılanamadı. Lütfen başka bir fotoğraf yükleyin." };
        }



    }
}
