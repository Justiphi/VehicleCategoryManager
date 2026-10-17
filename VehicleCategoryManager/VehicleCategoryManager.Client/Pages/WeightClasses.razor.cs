using System.Net.Http.Json;
using VehicleCategoryManager.Client.Components;
using VehicleCategoryManager.Client.DTOs;

namespace VehicleCategoryManager.Client.Pages
{
    public partial class WeightClasses : BasePage
    {
        private AddWeightClassModal? weightclassModal { get; set; }

        List<WeightClassDTO> weightClasses = new();

        protected override async Task OnInitializedAsync()
        {
            SortColumn = nameof(WeightClassDTO.MinWeightKg);
            await LoadWeightClasses();
        }

        private async Task LoadWeightClasses()
        {
            try
            {
                weightClasses = await Http.GetFromJsonAsync<List<WeightClassDTO>>("api/WeightClass") ?? new();
                loadErrorMessage = null;
                errorMessage = null;
            }
            catch (HttpRequestException)
            {
                loadErrorMessage = "Unable to load weight classes.";
            }
        }

        protected override async Task PromptDelete(int recordId)
        {
            var record = weightClasses.Find(v => v.VehicleWeightClassId == recordId);
            deleteModal.ConfirmationMessage = $"Are you sure you want to delete: {record.VehicleWeightClassName}?\nThe surrounding records will expand to fill the gap.";

            await base.PromptDelete(recordId);
        }

        private async Task DeleteWeightClass(int weightclassId)
        {
            HttpResponseMessage response = await Http.DeleteAsync($"api/WeightClass/{weightclassId}");
            if (response.IsSuccessStatusCode)
            {
                await LoadWeightClasses();
            }
            else
            {
                errorMessage = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(errorMessage))
                    errorMessage = "Unable to delete weight class.";
            }
        }

        private async Task EditWeightClass(int weightClassId)
        {
            errorMessage = null;

            if (weightClasses == null) return;
            weightclassModal?.WeightClass = weightClasses.FirstOrDefault(wc => wc.VehicleWeightClassId == weightClassId, new());
            OpenModal();
        }

        private IEnumerable<WeightClassDTO> SortedWeightClasses =>
            SortColumn switch
            {
                nameof(WeightClassDTO.VehicleWeightClassName) =>
                    SortAscending
                        ? weightClasses.OrderBy(wc => wc.VehicleWeightClassName)
                        : weightClasses.OrderByDescending(wc => wc.VehicleWeightClassName),

                nameof(WeightClassDTO.MinWeightKg) =>
                    SortAscending
                        ? weightClasses.OrderBy(wc => wc.MinWeightKg)
                        : weightClasses.OrderByDescending(wc => wc.MinWeightKg),

                nameof(WeightClassDTO.MaxWeightKg) =>
                    SortAscending
                        ? weightClasses.OrderBy(wc => wc.MaxWeightKg == null).ThenBy(wc => wc.MaxWeightKg)
                        : weightClasses.OrderByDescending(wc => wc.MaxWeightKg),

                _ => weightClasses
            };

    }
}
