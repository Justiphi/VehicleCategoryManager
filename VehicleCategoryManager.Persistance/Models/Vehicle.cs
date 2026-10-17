using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VehicleCategoryManager.Persistance.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }

        [Required]
        [Precision(10, 2)]
        public decimal WeightKg { get; set; }

        [Required]
        [MaxLength(140)]
        public string Owner { get; set; }

        [Required]
        public short Year { get; set; }

        //Foreign key to Manufacturer
        [Required]
        [ForeignKey(nameof(Manufacturer.ManufacturerId))]
        public int ManufacturerId { get; set; }
        public Manufacturer? Manufacturer { get; set; }

        public Vehicle()
        {
            WeightKg = 0;
            Owner = string.Empty;
            Year = 0;
            ManufacturerId = 0;
        }
        public Vehicle(decimal weightKg, string owner, short year, int manufacturerId, int id = 0)
        {
            WeightKg = weightKg;
            Owner = owner;
            Year = year;
            ManufacturerId = manufacturerId;
            VehicleId = id;
        }
    }
}
