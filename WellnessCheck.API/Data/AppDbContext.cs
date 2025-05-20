using Microsoft.EntityFrameworkCore;
using WellnessCheck.API.Entities;

namespace WellnessCheck.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }
    }
}
