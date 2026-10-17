using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleCategoryManager.Persistance.Models
{
    public class Manufacturer
    {
        [Key]
        public int ManufacturerId { get; set; }
        [Required]
        [MaxLength(20)]
        public string ManufacturerName { get; set; }

        public List<Vehicle> Vehicles { get; set; }

        public Manufacturer() 
        {
            ManufacturerName = string.Empty;
            Vehicles = new List<Vehicle>();
        }
        public Manufacturer(string name, int id = 0)
        {
            ManufacturerId = id;
            ManufacturerName = name;
            Vehicles = new List<Vehicle>();
        }
    }
}
