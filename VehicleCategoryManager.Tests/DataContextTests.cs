using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace VehicleCategoryManager.Tests
{
    public class DataContextTests : IDisposable
    {
        private readonly TestDatabase _database;

        public DataContextTests()
        {
            _database = new TestDatabase();
        }

        [Fact]
        public void Manufacturers_AreSeeded()
        {
            // Arrange
            var context = _database.Context;

            // Act
            var manufacturers = context.Manufacturers.ToList();

            // Assert
            Assert.Equal(5, manufacturers.Count);
            Assert.Contains(
                manufacturers,
                m => m.ManufacturerName == "Mazda");
            Assert.Contains(
                manufacturers,
                m => m.ManufacturerName == "Toyota");
        }

        [Fact]
        public void WeightClasses_AreSeeded()
        {
            var context = _database.Context;

            var classes = context.VehicleWeightClasses
                .OrderBy(c => c.VehicleWeightClassId)
                .ToList();

            Assert.Equal(3, classes.Count);
            Assert.Equal("Light", classes[0].VehicleWeightClassName);
            Assert.Equal("Medium", classes[1].VehicleWeightClassName);
            Assert.Equal("Heavy", classes[2].VehicleWeightClassName);
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}
