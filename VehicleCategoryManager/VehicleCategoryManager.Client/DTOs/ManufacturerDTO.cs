namespace VehicleCategoryManager.Client.DTOs
{
    public class ManufacturerDTO
    {
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }

        public ManufacturerDTO()
        {
            ManufacturerName = string.Empty;
        }
    }
}
