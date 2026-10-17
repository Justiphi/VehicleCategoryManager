using System.ComponentModel.DataAnnotations;

namespace VehicleCategoryManager.Client.DTOs
{
    public class VehicleDTO : IValidatableObject
    {
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Owner is required.")]
        [MaxLength(140, ErrorMessage = "Owner cannot exceed 140 characters.")]
        public string Owner { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        public short Year { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a manufacturer.")]
        public int ManufacturerId { get; set; }
        public string? ManufacturerName { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Weight must not be 0 or negative")]
        public decimal WeightKg { get; set; }
        public string? WeightCategoryIconPath { get; set; }

        public VehicleDTO()
        {
            Owner = string.Empty;
            Year = (short)DateTime.Now.Year;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Year.ToString().Length != 4)
            {
                yield return new ValidationResult("Year must be 4 characters.", new[] { nameof(Year) });
            }

            if (Year < 1800 || Year > DateTime.Now.Year)
            {
                yield return new ValidationResult($"Year must be between 1800 and {DateTime.Now.Year}.", new[] { nameof(Year) });
            }
        }
    }
}
