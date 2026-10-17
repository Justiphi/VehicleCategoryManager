using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleCategoryManager.Persistance.Models
{
    public class VehicleWeightClass
    {
        public int VehicleWeightClassId { get; set; }

        [Required]
        [MaxLength(20)]
        public string VehicleWeightClassName { get; set; }
        [Required]
        public string VehicleWeightClassIconPath { get; set; }

        [Precision(10, 2)]
        public decimal MinWeightKg { get; set; }
        [Precision(10, 2)]
        public decimal? MaxWeightKg { get; set; }

        public VehicleWeightClass()
        {
            VehicleWeightClassName = string.Empty;
            VehicleWeightClassIconPath = string.Empty;
            MinWeightKg = 0;
            MaxWeightKg = null;
        }
        public VehicleWeightClass(string name, string iconPath, decimal minWeightKg, decimal? maxWeightKg, int id = 0)
        {
            VehicleWeightClassId = id;
            VehicleWeightClassName = name;
            VehicleWeightClassIconPath = iconPath;
            MinWeightKg = minWeightKg;
            MaxWeightKg = maxWeightKg;
        }
    }
}
