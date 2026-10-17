using System.ComponentModel.DataAnnotations;

namespace VehicleCategoryManager.Client.DTOs
{
    public class WeightClassDTO
    {
        public int VehicleWeightClassId { get; set; }
        public string VehicleWeightClassName { get; set; }
        public string VehicleWeightClassIconPath { get; set; }
        public decimal MinWeightKg { get; set; }
        public decimal? MaxWeightKg { get; set; }

        public WeightClassDTO()
        {
            VehicleWeightClassName = string.Empty;
            VehicleWeightClassIconPath = string.Empty;
        }
    }
}
