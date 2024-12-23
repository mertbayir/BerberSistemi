using System.ComponentModel.DataAnnotations;

namespace odev.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Berber adı gereklidir.")]
        public string BarberName { get; set; }

        [Required(ErrorMessage = "Hizmet seçimi gereklidir.")]
        public string Service { get; set; }

        [Required(ErrorMessage = "Tarih gereklidir.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Saat gereklidir.")]
        public TimeSpan Time { get; set; }
        public string Status { get; set; } = "Beklemede";
        public int Duration { get; set; }
        public int Price { get; set; } 

    }

}
