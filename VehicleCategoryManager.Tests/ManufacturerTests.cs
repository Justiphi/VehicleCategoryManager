using VehicleCategoryManager.Persistance.Models;
using VehicleCategoryManager.Persistance.Services;

namespace VehicleCategoryManager.Tests
{
    public class ManufacturerTests
    {
        private readonly TestDatabase _database;
        private readonly VehicleService _vehicleService;
        private readonly ManufacturerService _manufacturerService;

        public ManufacturerTests()
        {
            _database = new TestDatabase();
            _vehicleService = new VehicleService(_database.Factory);
            _manufacturerService = new ManufacturerService(_database.Factory);
        }

        [Fact]
        public void AddManufacturer_WhenCalled_RecordAdded()
        {
            var manufacturer = new Manufacturer("Porsche");
            var added = _manufacturerService.AddManufacturer(manufacturer);
            Assert.NotEqual(0, added);
        }

        [Fact]
        public void AddManufacturer_WhenNameExists_CannotAdd()
        {
            var manufacturer = new Manufacturer("Toyota");
            Assert.ThrowsAny<InvalidDataException>(() => 
                _manufacturerService.AddManufacturer(manufacturer)
            );
        }

        [Fact]
        public void DeleteManufacturerWithVehicle_WhenCalled_CannotDelete()
        {
            var newVehicle = new Vehicle(500, "owner", (short)DateTime.Now.Year, 1);
            var added = _vehicleService.AddVehicle(newVehicle);
            Assert.NotEqual(0, added);

            Assert.ThrowsAny<InvalidOperationException>(() => 
                _manufacturerService.DeleteManufacturer(1)
            );
        }


    }
}
