namespace odev.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int ID { get; set; }
        public string Role { get; set; } = "Customer";  

    }
}
