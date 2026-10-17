using Microsoft.EntityFrameworkCore;
using VehicleCategoryManager.Persistance.Models;

namespace VehicleCategoryManager.Persistance
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleWeightClass> VehicleWeightClasses { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manufacturer>().HasData(
                    new Manufacturer("Mazda", 1),
                    new Manufacturer("Mercedes", 2),
                    new Manufacturer("Honda", 3),
                    new Manufacturer("Ferrari", 4),
                    new Manufacturer("Toyota", 5)
                );

            modelBuilder.Entity<VehicleWeightClass>().HasData(
                    new VehicleWeightClass("Light", "icons/light-weight.svg", 0, 500, 1),
                    new VehicleWeightClass("Medium", "icons/med-weight.svg", 500, 2500, 2),
                    new VehicleWeightClass("Heavy", "icons/heavy-weight.svg", 2500, null, 3)

                );
        }
    }
}
