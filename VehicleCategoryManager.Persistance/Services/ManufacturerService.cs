using VehicleCategoryManager.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace VehicleCategoryManager.Persistance.Services
{
    public class ManufacturerService : ServiceBase
    {

        public ManufacturerService(DataContextFactory contextFactory) : base(contextFactory) { }

        public bool ValidateRecord(DataContext context, Manufacturer manufacturer)
        {
            if (string.IsNullOrWhiteSpace(manufacturer.ManufacturerName))
            {
                return false;
            }

            if(context.Manufacturers.Where(m => m.ManufacturerName == manufacturer.ManufacturerName && m.ManufacturerId != manufacturer.ManufacturerId).Any())
            {
                return false;
            }

            return true;
        }

        public int AddManufacturer(Manufacturer manufacturer)
        {
            using (var context = _contextFactory.CreateContext())
            {
                if (!ValidateRecord(context, manufacturer))
                {
                    return 0;
                }
            
                context.Manufacturers.Add(manufacturer);
                context.SaveChanges();
            }

            return manufacturer.ManufacturerId;
        }

        public bool UpdateManufacturer(Manufacturer manufacturer)
        {
            Manufacturer? manufacturerInstanced = null;

            using (var context = _contextFactory.CreateContext())
            {
                //ensure new record is valid before updating
                if (!ValidateRecord(context, manufacturer))
                {
                    return false;
                }

                manufacturerInstanced = context.Manufacturers.Find(manufacturer.ManufacturerId);

                //ensure record exists before updating
                if (manufacturerInstanced == null)
                {
                    return false;
                }

                manufacturerInstanced.ManufacturerName = manufacturer.ManufacturerName;

                context.Manufacturers.Update(manufacturerInstanced);
                context.SaveChanges();
            }

            return true;
        }

        public bool DeleteManufacturer(int manufacturerId)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var manufacturer = context.Manufacturers.Include(m => m.Vehicles).FirstOrDefault(m => m.ManufacturerId == manufacturerId);
                if (manufacturer == null)
                {
                    return false;
                }

                if(manufacturer.Vehicles != null && manufacturer.Vehicles.Count > 0)
                {
                    // Cannot delete manufacturer with associated vehicles
                    return false;
                }

                context.Manufacturers.Remove(manufacturer);
                context.SaveChanges();
            }

            return true;
        }

        public Manufacturer? GetManufacturerById(int manufacturerId)
        {
            Manufacturer? manufacturer = null;

            using (var context = _contextFactory.CreateContext())
            {
                manufacturer = context.Manufacturers.Find(manufacturerId);
            }

            //if not found, will return null
            return manufacturer;
        }

        public IEnumerable<Manufacturer> GetManufacturerList()
        {
            List<Manufacturer> manufacturers = new List<Manufacturer>();
            
            using (var context = _contextFactory.CreateContext())
            {
                manufacturers = context.Manufacturers.ToList();
            }

            //if not found, will return null
            return manufacturers;
        }
    }
}
