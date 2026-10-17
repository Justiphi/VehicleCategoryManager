using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VehicleCategoryManager.Persistance.Services;

namespace VehicleCategoryManager.Persistance
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddSingleton(
                    sp => new DataContextFactory(
                        new DbContextOptionsBuilder<DataContext>()
                            .UseSqlServer(connectionString)
                            .Options));

            services.AddScoped<VehicleService>();
            services.AddScoped<WeightClassService>();
            services.AddScoped<ManufacturerService>();

            return services;
        }
    }
}
