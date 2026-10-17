namespace VehicleCategoryManager.Client.DTOs
{
    public class VehicleDTO
    {
        public int VehicleId { get; set; }
        public string Owner { get; set; }
        public short Year { get; set; }
        public int ManufacturerId { get; set; }
        public string? ManufacturerName { get; set; }
        public decimal WeightKg { get; set; }
        public string? WeightCategoryIconPath { get; set; }

        public VehicleDTO()
        {
            Owner = string.Empty;
            Year = (short)DateTime.Now.Year;
        }
    }
}
