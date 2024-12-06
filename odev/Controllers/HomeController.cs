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
            return View(); // Views/Home/Upload.cshtml dosyasýný döndürür
        }

        // Fotoðraf yüklemek için POST action metodu
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            if (photo != null && photo.Length > 0)
            {
                // Fotoðrafý kaydetme iþlemi (örnek kod)
                var filePath = Path.Combine("wwwroot/uploads", photo.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // API'ye gönder ve sonuçlarý al
                string apiResult = await AIPhotoHandle(filePath);
                return View("Result", apiResult); // Sonuç sayfasýný döndür
            }

            return View("Error"); // Hata durumunda error view'ýný göster
        }

        // API ile iþlem yapan özel metod
        private async Task<string> AIPhotoHandle(string photoPath)
        {
            string apiKey = "1ca8b7ba-cb45-4625-bea9-818c6fd64ea3"; // API anahtarýný kontrol et
            string apiUrl = "https://api.deepai.org/api/hair-style"; // Kullandýðýn API adresini kontrol et

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
                        // Yanýt baþarýlý deðilse detaylarý yazdýr
                        var errorDetails = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"API Error: {response.StatusCode}, Details: {errorDetails}");
                        throw new Exception($"API request failed: {response.StatusCode}, Details: {errorDetails}");
                    }

                    // Baþarýlý ise yanýtý döndür
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    // Hata durumunda ayrýntýlý loglama
                    Console.WriteLine($"HttpRequestException: {ex.Message}");
                    throw; // Hata tekrar fýrlatýlýyor
                }
            }
        }
    }

}
