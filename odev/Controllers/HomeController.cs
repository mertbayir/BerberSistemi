using Microsoft.AspNetCore.Mvc;
using odev.Models;
using System.Diagnostics;

namespace odev.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View(); // Views/Home/Upload.cshtml dosyas�n� d�nd�r�r
        }

        // Foto�raf y�klemek i�in POST action metodu
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            if (photo != null && photo.Length > 0)
            {
                // Foto�raf� kaydetme i�lemi (�rnek kod)
                var filePath = Path.Combine("wwwroot/uploads", photo.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // API'ye g�nder ve sonu�lar� al
                string apiResult = await AIPhotoHandle(filePath);
                return View("Result", apiResult); // Sonu� sayfas�n� d�nd�r
            }

            return View("Error"); // Hata durumunda error view'�n� g�ster
        }

        // API ile i�lem yapan �zel metod
        private async Task<string> AIPhotoHandle(string photoPath)
        {
            string apiKey = "1ca8b7ba-cb45-4625-bea9-818c6fd64ea3"; // API anahtar�n� kontrol et
            string apiUrl = "https://api.deepai.org/api/hair-style"; // Kulland���n API adresini kontrol et

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Api-Key", apiKey);

                var formData = new MultipartFormDataContent();
                formData.Add(new ByteArrayContent(await System.IO.File.ReadAllBytesAsync(photoPath)), "image", Path.GetFileName(photoPath));

                try
                {
                    var response = await client.PostAsync(apiUrl, formData);

                    if (!response.IsSuccessStatusCode)
                    {
                        // Yan�t ba�ar�l� de�ilse detaylar� yazd�r
                        var errorDetails = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"API Error: {response.StatusCode}, Details: {errorDetails}");
                        throw new Exception($"API request failed: {response.StatusCode}, Details: {errorDetails}");
                    }

                    // Ba�ar�l� ise yan�t� d�nd�r
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    // Hata durumunda ayr�nt�l� loglama
                    Console.WriteLine($"HttpRequestException: {ex.Message}");
                    throw; // Hata tekrar f�rlat�l�yor
                }
            }
        }
    }

}
