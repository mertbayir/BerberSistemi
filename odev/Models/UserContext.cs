using Microsoft.EntityFrameworkCore;

namespace odev.Models
{
    public class UserContext : DbContext
    {
		public DbSet<User> Users { get; set; }
        public DbSet<Barber> Barbers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
    }
}
