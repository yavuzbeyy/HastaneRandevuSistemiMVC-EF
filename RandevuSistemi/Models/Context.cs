using Microsoft.EntityFrameworkCore;
using RandevuSistemi.Models.Entities;

namespace RandevuSistemi.Models
{
    public class Context : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=RandevuSistemi2;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        public DbSet<AnaBilimDali> AnaBilimDallari { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Doktor> Doktorlar { get; set; }
        public DbSet<Poliklinik> Poliklinikler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
        public DbSet<Hizmetler> Hizmetler { get; set; }
        public DbSet<CalismaSaatleri> CalismaSaatleri { get; set; }
    }
}
