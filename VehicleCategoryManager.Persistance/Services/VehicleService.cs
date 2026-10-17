using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleCategoryManager.Persistance.Models;

namespace VehicleCategoryManager.Persistance.Services
{
    public class VehicleService : ServiceBase
    {
        public VehicleService(DataContextFactory contextFactory) : base(contextFactory) { }

        public bool ValidateRecord(DataContext context, Vehicle vehicle)
        {
            List<string> errorList = new();

            if(vehicle.WeightKg <= 0)
            {
                errorList.Add("Weight must be a positive value.");
            }

            if(vehicle.Year.ToString().Length != 4)
            {
                errorList.Add("Invalid Year: Year must be 4 characters");
            }

            if (vehicle.Year < 1800 || vehicle.Year > DateTime.Now.Year)
            {
                errorList.Add($"Invalid Year: Year must be between 1800 and {DateTime.Now.Year}");
            }

            if (errorList.Any())
            {
                throw new InvalidDataException(string.Join(", \n", errorList));
            }

            return true;
        }

        public int AddVehicle(Vehicle vehicle)
        {
            using (var context = _contextFactory.CreateContext())
            {
                if (!ValidateRecord(context, vehicle))
                {
                    return 0;
                }

                context.Vehicles.Add(vehicle);
                context.SaveChanges();
            }

            return vehicle.VehicleId;
        }

        public bool UpdateVehicle(Vehicle vehicle)
        {
            Vehicle? vehicleInstanced = null;

            using (var context = _contextFactory.CreateContext())
            {
                //ensure new record is valid before updating
                if (!ValidateRecord(context, vehicle))
                {
                    return false;
                }

                vehicleInstanced = context.Vehicles.Find(vehicle.VehicleId);

                //ensure record exists before updating
                if (vehicleInstanced == null)
                {
                    throw new InvalidDataException("Cannot Update: Vehicle not found.");
                }

                vehicleInstanced.WeightKg = vehicle.WeightKg;
                vehicleInstanced.Owner = vehicle.Owner;
                vehicleInstanced.Year = vehicle.Year;
                vehicleInstanced.ManufacturerId = vehicle.ManufacturerId;

                context.Vehicles.Update(vehicleInstanced);
                context.SaveChanges();
            }

            return true;
        }

        public bool DeleteVehicle(int vehicleId)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var vehicle = context.Vehicles.Find(vehicleId);
                if (vehicle == null)
                {
                    throw new InvalidDataException("Cannot Delete: Vehicle not found.");
                }

                context.Vehicles.Remove(vehicle);
                context.SaveChanges();
            }

            return true;
        }

        public Vehicle? GetVehicleById(int vehicleId)
        {
            Vehicle? vehicle = null;

            using (var context = _contextFactory.CreateContext())
            {
                vehicle = context.Vehicles.Find(vehicleId);
            }

            //if not found, will return null
            return vehicle;
        }

        public IEnumerable<Vehicle> GetVehicleList()
        {
            List<Vehicle> vehicles = new List<Vehicle>();

            using (var context = _contextFactory.CreateContext())
            {
                vehicles = context.Vehicles.Include(v => v.Manufacturer).ToList();
            }

            //if not found, will return null
            return vehicles;
        }
    }
}
