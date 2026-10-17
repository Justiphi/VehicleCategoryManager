using System.ComponentModel.DataAnnotations;

namespace VehicleCategoryManager.Client.DTOs
{
    public class ManufacturerDTO
    {
        public int ManufacturerId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(20, ErrorMessage = "Name cannot exceed 20 characters.")]
        public string ManufacturerName { get; set; }

        public ManufacturerDTO()
        {
            ManufacturerName = string.Empty;
        }
    }
}
