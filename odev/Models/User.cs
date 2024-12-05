using System.ComponentModel.DataAnnotations;

namespace odev.Models
{
    public class User
    {
        [Required(ErrorMessage = "Bu Alan Boş Bırakılamaz")]
        [Display(Name = "Ad Soyad :")] 
        public string UserName { get; set; }

        [Required(ErrorMessage="Bu Alan Boş Bırakılamaz")]
        [Display(Name = "Kullanıcı Adı (İsim Soyisim şeklinde giriniz) :")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Bu Alan Boş Bırakılamaz")]
        [Display(Name = "Şifre :")]
        public string Password { get; set; }
        public int ID { get; set; }
        public string Role { get; set; } = "Customer";  

    }
}
