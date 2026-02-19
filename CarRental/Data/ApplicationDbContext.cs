using CarRental.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<RentalContract> RentalContracts { get; set; }
        public DbSet<PriceTariff> PriceTariffs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Car>()
                .HasMany(c => c.RentalContracts)
                .WithOne(r => r.Car)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.RentalContracts)
                .WithOne(r => r.Client)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
