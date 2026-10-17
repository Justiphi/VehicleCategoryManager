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
            if(vehicle.WeightKg <= 0)
            {
                return false;
            }

            if(vehicle.Year.ToString().Length != 4)
            {
                return false;
            }

            if (vehicle.Year < 1886 || vehicle.Year > DateTime.Now.Year)
            {
                return false;
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
                    return false;
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
                    return false;
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
