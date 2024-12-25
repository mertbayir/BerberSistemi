namespace odev.Models
{
    public class BarberUserViewModel
    {
        public Barber Barber { get; set; }
        public User User { get; set; }
        public List<ServicePriceDuration> Services { get; set; } = new List<ServicePriceDuration>();

    }
}
