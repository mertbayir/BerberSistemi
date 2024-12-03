namespace odev.Services
{
    public class AIService
    {

        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "1ca8b7ba-cb45-4625-bea9-818c6fd64ea3"; // DeepAI API anahtarınızı buraya koyun

        public AIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetHairstyleRecommendation(IFormFile uploadedImage)
        {
            // API'ye gönderilecek form verilerini hazırlıyoruz
            using (var content = new MultipartFormDataContent())
            {
                // Fotoğrafı ekliyoruz
                var fileContent = new StreamContent(uploadedImage.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                content.Add(fileContent, "image", uploadedImage.FileName);

                // API'ye istek gönderiyoruz
                var response = await _httpClient.PostAsync("https://api.deepai.org/api/neuraltalk", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("API isteği başarısız oldu.");
                }

                // JSON yanıtını alıyoruz
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // JSON verisini işlememiz gerekiyor. Ancak burada DeepAI'nin
                // döndürdüğü stil verisi genellikle farklı formatlarda olabilir.
                // Geri döndürülen saç stillerini liste olarak döndürebileceğiniz basit bir örnek:

                var hairstyleRecommendations = ParseResponse(jsonResponse);

                return hairstyleRecommendations;
            }
        }

        // JSON yanıtını işlemek için basit bir metot (örnek)
        private List<string> ParseResponse(string response)
        {
            // Burada JSON verisini işleyip, saç stilleri veya öneriler içeren bir liste oluşturmalısınız
            // Bu sadece basit bir örnek; JSON parsing işlemi gerçek API'nin döndüğü yapıya göre yapılmalıdır.
            var hairstyleRecommendations = new List<string>();

            // Örnek olarak, yanıtı metin olarak döndürüyoruz.
            hairstyleRecommendations.Add(response); // Bu kısmı, gerçek JSON işleme kodu ile değiştirebilirsiniz.

            return hairstyleRecommendations;
        }



    }
}
