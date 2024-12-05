namespace odev.Models
{
    public class ServicePriceDuration
    {
        public int Id { get; set; }  
        public string BarberName { get; set; }
        public string Service {  get; set; }
        public int Price { get; set; }
        public int Duration { get; set; }
    }
}
