namespace odev.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string BarberName { get; set; }
        public string Service { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Status { get; set; } = "Beklemede";
        public int Duration { get; set; }
        public int Price { get; set; } 

    }

}
