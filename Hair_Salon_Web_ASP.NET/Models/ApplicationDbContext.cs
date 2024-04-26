using Microsoft.EntityFrameworkCore;
using Hair_Salon_Web_ASP.NET.Models;

namespace Hair_Salon_Web_ASP.NET.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options){}
        public DbSet<User> Users { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
