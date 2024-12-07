namespace odev.Models
{
    public class BarberDailyEarningsViewModel
    {
        public string BarberName { get; set; }
        public List<DailyEarnings> DailyEarnings { get; set; }
        public int TotalAppointments { get; set; } 
        public decimal TotalEarnings { get; set; } 
    }
}
