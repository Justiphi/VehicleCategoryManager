using VehicleCategoryManager.Persistance.Models;
using VehicleCategoryManager.Persistance.Services;

namespace VehicleCategoryManager.Tests
{
    public class WeightClassTests
    {
        private readonly TestDatabase _database;
        private readonly WeightClassService _weightClassService;

        public WeightClassTests()
        {
            _database = new TestDatabase();
            _weightClassService = new WeightClassService(_database.Factory);
        }

        [Fact]
        public void DeleteWeightClass_WhenCalled_AdjustsAdjacentWeightClasses()
        {
            // Get medium weight class (seeded Id 2)
            var mediumWeightClass = _database.Context.VehicleWeightClasses.FirstOrDefault(wc => wc.VehicleWeightClassId == 2);
            Assert.NotNull(mediumWeightClass);

            var deleted = _weightClassService.DeleteWeightClass(mediumWeightClass.VehicleWeightClassId);
            Assert.True(deleted);
            mediumWeightClass = null;

            // ensure weight class is deleted
            mediumWeightClass = _database.Context.VehicleWeightClasses.FirstOrDefault(wc => wc.VehicleWeightClassId == 2);
            Assert.Null(mediumWeightClass);

            //small weight class should now have a max weight of 1500
            var smallWeightClass = _database.Context.VehicleWeightClasses.FirstOrDefault(wc => wc.VehicleWeightClassId == 1);
            Assert.NotNull(smallWeightClass);
            Assert.Equal(1500, smallWeightClass.MaxWeightKg);

            // large weight class should now have a min weight of 1500
            var largeWeightClass = _database.Context.VehicleWeightClasses.FirstOrDefault(wc => wc.VehicleWeightClassId == 3);
            Assert.NotNull(largeWeightClass);
            Assert.Equal(1500, largeWeightClass.MinWeightKg);
        }

        [Fact]
        public void AddWeightClass_WhenCalled_AddsNewWeightClass()
        {
            var newWeightClass = new VehicleWeightClass("Extra Heavy", "icons/weight.svg", 5000, null, 4);
            var added = _weightClassService.AddWeightClass(newWeightClass);
            Assert.NotEqual(0, added);
            var addedWeightClass = _weightClassService.GetWeightClassById(added);
            Assert.NotNull(addedWeightClass);
            Assert.Equal(addedWeightClass.VehicleWeightClassName, "Extra Heavy");
            Assert.Equal(5000, addedWeightClass.MinWeightKg);
            Assert.Null(addedWeightClass.MaxWeightKg);

            //heavy class should limit to new weight class
            var heavyWeightClass = _database.Context.VehicleWeightClasses.FirstOrDefault(wc => wc.VehicleWeightClassId == 3);
            Assert.NotNull(heavyWeightClass);
            Assert.Equal(5000, heavyWeightClass.MaxWeightKg);
        }

        [Fact]
        public void UpdateWeightClass_WhenCalled_AdjustsAdjacentWeightClasses()
        {
            var medWeightClass = _database.Context.VehicleWeightClasses.FirstOrDefault(wc => wc.VehicleWeightClassId == 2);
            Assert.NotNull(medWeightClass);
            medWeightClass.MinWeightKg = 250;
            medWeightClass.MaxWeightKg = 3000;
            var updated = _weightClassService.UpdateWeightClass(medWeightClass);
            Assert.True(updated);


            //heavy class min weight should be adjusted to med weight class max weight
            var heavyWeightClass = _database.Context.VehicleWeightClasses.FirstOrDefault(wc => wc.VehicleWeightClassId == 3);
            Assert.NotNull(heavyWeightClass);
            Assert.Equal(3000, heavyWeightClass.MinWeightKg);

            //light class max weight should be adjusted to med weight class min weight
            var lightWeightClass = _database.Context.VehicleWeightClasses.FirstOrDefault(wc => wc.VehicleWeightClassId == 1);
            Assert.NotNull(lightWeightClass);
            Assert.Equal(250, lightWeightClass.MaxWeightKg);
        }

        [Fact]
        public void DeleteWeightClass_WhenOnlyOneClassLeft_DoesNotDelete()
        {
            // Delete medium and heavy classes first
            _weightClassService.DeleteWeightClass(2);
            _weightClassService.DeleteWeightClass(3);
            // Attempt to delete the last remaining class (light)
            var deleted = _weightClassService.DeleteWeightClass(1);
            Assert.False(deleted);
            // Ensure the light class still exists
            var lightWeightClass = _database.Context.VehicleWeightClasses.FirstOrDefault(wc => wc.VehicleWeightClassId == 1);
            Assert.NotNull(lightWeightClass);
        }

        [Fact]
        public void DeleteWeightClass_WhenOnlyOneClassLeft_HasFullRange()
        {
            // Delete medium and heavy classes first
            _weightClassService.DeleteWeightClass(2);
            _weightClassService.DeleteWeightClass(3);
            var lightWeightClass = _database.Context.VehicleWeightClasses.FirstOrDefault(wc => wc.VehicleWeightClassId == 1);
            Assert.NotNull(lightWeightClass);
            Assert.Equal(0, lightWeightClass.MinWeightKg);
            Assert.Null(lightWeightClass.MaxWeightKg);
        }

        [Fact]
        public void AddWeightClass_WhenOnlyOneClassLeft_HasMinAndMax()
        {
            // Delete medium and heavy classes first
            _weightClassService.DeleteWeightClass(2);
            _weightClassService.DeleteWeightClass(3);
            //Add new heavy class
            var newWeightClass = new VehicleWeightClass("Test", "icons/weight.svg", 3000, null);
            var added = _weightClassService.AddWeightClass(newWeightClass);
            Assert.NotEqual(0, added);

            var lightWeightClass = _weightClassService.GetWeightClassById(1);
            Assert.NotNull(lightWeightClass);
            Assert.Equal(0, lightWeightClass.MinWeightKg);
            Assert.Equal(3000, lightWeightClass.MaxWeightKg);

            var heavyWeightClass = _weightClassService.GetWeightClassById(added);
            Assert.NotNull(heavyWeightClass);
            Assert.Equal(3000, heavyWeightClass.MinWeightKg);
            Assert.Null(heavyWeightClass.MaxWeightKg);
        }

        [Fact]
        public void AddWeightClass_WhenIconAlreadyUsed_CannotAdd()
        {
            var newWeightClass = new VehicleWeightClass("Test", "icons/light-weight.svg", 3000, null, 2);
            var added = _weightClassService.AddWeightClass(newWeightClass);
            Assert.Equal(0, added);
        }

        [Fact]
        public void AddWeightClass_WhenNameAlreadyUsed_CannotAdd()
        {
            var newWeightClass = new VehicleWeightClass("Heavy", "icons/weight.svg", 3000, null, 2);
            var added = _weightClassService.AddWeightClass(newWeightClass);
            Assert.Equal(0, added);
        }

        [Fact]
        public void AddWeightClass_WhenIconDoesNotExist_CannotAdd()
        {
            var newWeightClass = new VehicleWeightClass("Test", "no-icon", 3000, null, 2);
            var added = _weightClassService.AddWeightClass(newWeightClass);
            Assert.Equal(0, added);
        }
    }
}
