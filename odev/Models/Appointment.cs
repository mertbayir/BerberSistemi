namespace odev.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BarberId { get; set; }
        public string Service { get; set; }
        public DateTime DateTime { get; set; }
        public User User { get; set; }
        public Barber Barber { get; set; }
    }

}
