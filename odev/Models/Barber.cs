using System.ComponentModel.DataAnnotations;

namespace odev.Models
{
    public class Barber
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Yetenek gereklidir.")]
        public string Skills { get; set; }

        [Required(ErrorMessage = "Özel Yetenek gereklidir.")]
        public string Specialization {  get; set; }
    }
}
