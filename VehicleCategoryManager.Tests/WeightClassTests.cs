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
            var mediumWeightClass = _weightClassService.GetWeightClassById(2);
            Assert.NotNull(mediumWeightClass);

            var deleted = _weightClassService.DeleteWeightClass(mediumWeightClass.VehicleWeightClassId);
            Assert.True(deleted);
            mediumWeightClass = null;

            // ensure weight class is deleted
            mediumWeightClass = _weightClassService.GetWeightClassById(2);
            Assert.Null(mediumWeightClass);

            //small weight class should now have a max weight of 1500
            var smallWeightClass = _weightClassService.GetWeightClassById(1);
            Assert.NotNull(smallWeightClass);
            Assert.Equal(1500, smallWeightClass.MaxWeightKg);

            // large weight class should now have a min weight of 1500
            var largeWeightClass = _weightClassService.GetWeightClassById(3);
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
            Assert.Equal("Extra Heavy", addedWeightClass.VehicleWeightClassName);
            Assert.Equal(5000, addedWeightClass.MinWeightKg);
            Assert.Null(addedWeightClass.MaxWeightKg);

            //heavy class should limit to new weight class
            var heavyWeightClass = _weightClassService.GetWeightClassById(3);
            Assert.NotNull(heavyWeightClass);
            Assert.Equal(5000, heavyWeightClass.MaxWeightKg);
        }

        [Fact]
        public void UpdateWeightClass_WhenExpanding_AdjustsAdjacentWeightClasses()
        {
            var medWeightClass = _weightClassService.GetWeightClassById(2);
            Assert.NotNull(medWeightClass);
            medWeightClass.MinWeightKg = 250;
            medWeightClass.MaxWeightKg = 3000;
            var updated = _weightClassService.UpdateWeightClass(medWeightClass);
            Assert.True(updated);

            //heavy class min weight should be adjusted to med weight class max weight
            var heavyWeightClass = _weightClassService.GetWeightClassById(3);
            Assert.NotNull(heavyWeightClass);
            Assert.Equal(3000, heavyWeightClass.MinWeightKg);
            Assert.Null(heavyWeightClass.MaxWeightKg);

            //light class max weight should be adjusted to med weight class min weight
            var lightWeightClass = _weightClassService.GetWeightClassById(1);
            Assert.NotNull(lightWeightClass);
            Assert.Equal(0, lightWeightClass.MinWeightKg);
            Assert.Equal(250, lightWeightClass.MaxWeightKg);
        }

        [Fact]
        public void UpdateWeightClass_WhenShrinking_AdjustsAdjacentWeightClasses()
        {
            var medWeightClass = _weightClassService.GetWeightClassById(2);
            Assert.NotNull(medWeightClass);
            medWeightClass.MinWeightKg = 750;
            medWeightClass.MaxWeightKg = 2000;
            var updated = _weightClassService.UpdateWeightClass(medWeightClass);
            Assert.True(updated);

            //heavy class min weight should be adjusted to med weight class max weight
            var heavyWeightClass = _weightClassService.GetWeightClassById(3);
            Assert.NotNull(heavyWeightClass);
            Assert.Equal(2000, heavyWeightClass.MinWeightKg);
            Assert.Null(heavyWeightClass.MaxWeightKg);

            //light class max weight should be adjusted to med weight class min weight
            var lightWeightClass = _weightClassService.GetWeightClassById(1);
            Assert.NotNull(lightWeightClass);
            Assert.Equal(0, lightWeightClass.MinWeightKg);
            Assert.Equal(750, lightWeightClass.MaxWeightKg);
        }

        [Fact]
        public void UpdateWeightClass_ShiftForward_AdjustsAdjacentWeightClasses()
        {
            var medWeightClass = _weightClassService.GetWeightClassById(2);
            Assert.NotNull(medWeightClass);
            medWeightClass.MinWeightKg = 750;
            medWeightClass.MaxWeightKg = 3000;
            var updated = _weightClassService.UpdateWeightClass(medWeightClass);
            Assert.True(updated);

            //heavy class min weight should be adjusted to med weight class max weight
            var heavyWeightClass = _weightClassService.GetWeightClassById(3);
            Assert.NotNull(heavyWeightClass);
            Assert.Equal(3000, heavyWeightClass.MinWeightKg);
            Assert.Null(heavyWeightClass.MaxWeightKg);

            //light class max weight should be adjusted to med weight class min weight
            var lightWeightClass = _weightClassService.GetWeightClassById(1);
            Assert.NotNull(lightWeightClass);
            Assert.Equal(0, lightWeightClass.MinWeightKg);
            Assert.Equal(750, lightWeightClass.MaxWeightKg);
        }

        [Fact]
        public void UpdateWeightClass_ShiftBack_AdjustsAdjacentWeightClasses()
        {
            var medWeightClass = _weightClassService.GetWeightClassById(2);
            Assert.NotNull(medWeightClass);
            medWeightClass.MinWeightKg = 250;
            medWeightClass.MaxWeightKg = 2000;
            var updated = _weightClassService.UpdateWeightClass(medWeightClass);
            Assert.True(updated);

            //heavy class min weight should be adjusted to med weight class max weight
            var heavyWeightClass = _weightClassService.GetWeightClassById(3);
            Assert.NotNull(heavyWeightClass);
            Assert.Equal(2000, heavyWeightClass.MinWeightKg);
            Assert.Null(heavyWeightClass.MaxWeightKg);

            //light class max weight should be adjusted to med weight class min weight
            var lightWeightClass = _weightClassService.GetWeightClassById(1);
            Assert.NotNull(lightWeightClass);
            Assert.Equal(0, lightWeightClass.MinWeightKg);
            Assert.Equal(250, lightWeightClass.MaxWeightKg);
        }

        [Fact]
        public void DeleteWeightClass_WhenOnlyOneClassLeft_DoesNotDelete()
        {
            // Delete medium and heavy classes first
            _weightClassService.DeleteWeightClass(2);
            _weightClassService.DeleteWeightClass(3);
            // Attempt to delete the last remaining class (light)
            Assert.ThrowsAny<InvalidOperationException>(() =>
                _weightClassService.DeleteWeightClass(1)
            );

            // Ensure the light class still exists
            var lightWeightClass = _weightClassService.GetWeightClassById(1);
            Assert.NotNull(lightWeightClass);
        }

        [Fact]
        public void DeleteWeightClass_WhenOnlyOneClassLeft_HasFullRange()
        {
            // Delete medium and heavy classes first
            _weightClassService.DeleteWeightClass(2);
            _weightClassService.DeleteWeightClass(3);
            var lightWeightClass = _weightClassService.GetWeightClassById(1);
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
            Assert.ThrowsAny<InvalidDataException>(() =>
                _weightClassService.AddWeightClass(newWeightClass)
            );
        }

        [Fact]
        public void AddWeightClass_WhenNameAlreadyUsed_CannotAdd()
        {
            var newWeightClass = new VehicleWeightClass("Heavy", "icons/weight.svg", 3000, null, 2);
            Assert.ThrowsAny<InvalidDataException>(() =>
                _weightClassService.AddWeightClass(newWeightClass)
            );
        }

        [Fact]
        public void AddWeightClass_WhenIconDoesNotExist_CannotAdd()
        {
            var newWeightClass = new VehicleWeightClass("Test", "no-icon", 3000, null, 2);
            Assert.ThrowsAny<InvalidDataException>(() =>
                _weightClassService.AddWeightClass(newWeightClass)
            );
        }

        [Fact]
        public void GetWeightClassByWeight_WhenCalled_ReturnsCorrectClass()
        {
            decimal[] weightsToTest = [ 500m, 2500m, 7000m, 600m, 650.22m ];

            foreach(decimal weight in weightsToTest)
            {
                var weightClass = _weightClassService.GetWeightClassByWeight(weight);
                Assert.NotNull(weightClass);
                Assert.True(weight < weightClass.MaxWeightKg || weightClass.MaxWeightKg == null);
                Assert.True(weight >= weightClass.MinWeightKg);
            }
        }

        [Fact]
        public void GetWeightClassByWeight_WhenCalledWithNegative_ReturnsNothing()
        {
            var weightClass = _weightClassService.GetWeightClassByWeight(-200);
            Assert.Null(weightClass);
        }

        [Fact]
        public void EditWeightClass_WhenAlterHeaviestMin_AdjustPreviousClass()
        {
            var weightClass = _weightClassService.GetWeightClassById(3);
            Assert.NotNull(weightClass);

            //check increase
            weightClass.MinWeightKg = 3000m;
            _weightClassService.UpdateWeightClass(weightClass);
            var medWeightClass = _weightClassService.GetWeightClassById(2);
            Assert.NotNull(medWeightClass);
            Assert.Equal(3000, medWeightClass.MaxWeightKg);



            //check decrease
            weightClass.MinWeightKg = 2000m;
            _weightClassService.UpdateWeightClass(weightClass);
            medWeightClass = _weightClassService.GetWeightClassById(2);
            Assert.NotNull(medWeightClass);
            Assert.Equal(2000, medWeightClass.MaxWeightKg);
        }

        [Fact]
        public void EditWeightClass_WhenAlterLightestMax_AdjustNextClass()
        {
            var weightClass = _weightClassService.GetWeightClassById(1);
            Assert.NotNull(weightClass);

            //check decrease
            weightClass.MaxWeightKg = 250m;
            _weightClassService.UpdateWeightClass(weightClass);
            var medWeightClass = _weightClassService.GetWeightClassById(2);
            Assert.NotNull(medWeightClass);
            Assert.Equal(250, medWeightClass.MinWeightKg);

            //check increase
            weightClass.MaxWeightKg = 750m;
            _weightClassService.UpdateWeightClass(weightClass);
            medWeightClass = _weightClassService.GetWeightClassById(2);
            Assert.NotNull(medWeightClass);
            Assert.Equal(750, medWeightClass.MinWeightKg);
        }

        [Fact]
        public void EditWeightClass_WhenAlterHeaviestMax_CannotAlter()
        {
            var weightClass = _weightClassService.GetWeightClassById(3);
            Assert.NotNull(weightClass);

            weightClass.MaxWeightKg = 3000m;
            Assert.ThrowsAny<InvalidDataException>(() =>
                _weightClassService.UpdateWeightClass(weightClass)
            );
        }

        [Fact]
        public void EditWeightClass_WhenAlterLightestMin_CannotAlter()
        {
            var weightClass = _weightClassService.GetWeightClassById(1);
            Assert.NotNull(weightClass);

            weightClass.MinWeightKg = 250m;
            Assert.ThrowsAny<InvalidDataException>(() =>
                _weightClassService.UpdateWeightClass(weightClass)
            );
        }
    }
}
