using Microsoft.AspNetCore.Mvc;
using odev.Services;

namespace odev.Controllers
{
    public class AIController : Controller
    {
        private readonly AIService _aiService;

        public AIController(AIService aiService)
        {
            _aiService = aiService;
        }

        public IActionResult GetRecommendation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetRecommendation(IFormFile uploadedImage)
        {
            if (uploadedImage == null || uploadedImage.Length == 0)
            {
                ModelState.AddModelError("", "Lütfen bir fotoğraf yükleyin.");
                return View();
            }

            try
            {
                // AIService üzerinden saç stili önerisini alıyoruz
                var recommendation = await _aiService.GetHairstyleRecommendation(uploadedImage);

                // Öneriyi View'a gönderiyoruz
                ViewBag.Recommendation = recommendation;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }

            return View();
        }
    }

}

