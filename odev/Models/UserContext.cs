using Microsoft.EntityFrameworkCore;
using odev.Migrations;

namespace odev.Models
{
    public class UserContext : DbContext
    {
		public DbSet<User> Users { get; set; }
        public DbSet<Barber> Barbers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ServicePriceDuration> ServicePriceDurations { get; set; }
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
    }
}
