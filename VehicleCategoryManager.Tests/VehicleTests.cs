using VehicleCategoryManager.Persistance.Models;
using VehicleCategoryManager.Persistance.Services;

namespace VehicleCategoryManager.Tests
{
    public class VehicleTests
    {
        private readonly TestDatabase _database;
        private readonly VehicleService _vehicleService;

        public VehicleTests()
        {
            _database = new TestDatabase();
            _vehicleService = new VehicleService(_database.Factory);
        }

        [Fact]
        public void AddVehicle_WhenCalled_YearValidationWorks()
        {
            //cannot add year without 4 digits
            var vehicle = new Vehicle(500, "owner", 111, 1);
            var added = _vehicleService.AddVehicle(vehicle);
            Assert.Equal(0, added);

            //cannot add year from the future
            vehicle.Year = (short)(DateTime.Now.Year + 1);
            added = _vehicleService.AddVehicle(vehicle);
            Assert.Equal(0, added);

            //can add vehicle from current year
            vehicle.Year = (short)DateTime.Now.Year;
            added = _vehicleService.AddVehicle(vehicle);
            Assert.NotEqual(0, added);
        }

        [Fact]
        public void AddVehicle_WhenCalled_VahicleAdded_CanDelete()
        {
            var newVehicle = new Vehicle(500, "owner", (short)DateTime.Now.Year, 1);
            var added = _vehicleService.AddVehicle(newVehicle);
            Assert.NotEqual(0, added);

            var vehicle = _vehicleService.GetVehicleById(added);
            Assert.NotNull(vehicle);

            var removed = _vehicleService.DeleteVehicle(added);
            Assert.True(removed);

            vehicle = _vehicleService.GetVehicleById(added);
            Assert.Null(vehicle);
        }

    }
}
