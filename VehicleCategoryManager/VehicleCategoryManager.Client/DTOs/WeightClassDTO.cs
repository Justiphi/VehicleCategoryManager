using System.ComponentModel.DataAnnotations;

namespace VehicleCategoryManager.Client.DTOs
{
    public class WeightClassDTO : IValidatableObject
    {
        public int VehicleWeightClassId { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        [StringLength(20, ErrorMessage = "Name cannot exeed 20 characters.")]
        public string VehicleWeightClassName { get; set; }

        [Required(ErrorMessage = "An icon is required to be selected.")]
        public string VehicleWeightClassIconPath { get; set; }

        [Required(ErrorMessage = "Minimum Weight is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum Weight cannot be negative.")]
        public decimal MinWeightKg { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Maximum Weight cannot be negative.")]
        public decimal? MaxWeightKg { get; set; }

        public WeightClassDTO()
        {
            VehicleWeightClassName = string.Empty;
            VehicleWeightClassIconPath = string.Empty;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MaxWeightKg.HasValue && MaxWeightKg.Value < MinWeightKg)
            {
                yield return new ValidationResult("Maximum Weight cannot be lower than Minimum Weight.", new[] { nameof(MaxWeightKg) });
            }
        }
    }
}
