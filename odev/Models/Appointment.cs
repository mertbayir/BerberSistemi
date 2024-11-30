namespace odev.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string BarberName { get; set; }
        public string Service { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; } = "Valid";

    }

}
