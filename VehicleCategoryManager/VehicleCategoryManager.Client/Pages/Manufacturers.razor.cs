using System.Net.Http.Json;
using VehicleCategoryManager.Client.Components;
using VehicleCategoryManager.Client.DTOs;

namespace VehicleCategoryManager.Client.Pages
{
    public partial class Manufacturers : BasePage
    {
        private AddManufacturerModal? manufacturerModal { get; set; }

        List<ManufacturerDTO> manufacturers = new();

        protected override async Task OnInitializedAsync()
        {
            SortColumn = nameof(ManufacturerDTO.ManufacturerName);
            await LoadManufacturers();
        }

        private async Task LoadManufacturers()
        {
            try
            {
                manufacturers = await Http.GetFromJsonAsync<List<ManufacturerDTO>>("api/manufacturer") ?? new();
                errorMessage = null;
                loadErrorMessage = null;
            }
            catch (HttpRequestException)
            {
                loadErrorMessage = "Unable to load manufacturers.";
            }
        }

        protected override async Task PromptDelete(int recordId)
        {
            var record = manufacturers.Find(v => v.ManufacturerId == recordId);
            deleteModal.ConfirmationMessage = $"Are you sure you want to delete: {record.ManufacturerName}?";

            await base.PromptDelete(recordId);
        }

        private async Task DeleteManufacturer(int manufacturerId)
        {
            HttpResponseMessage response = await Http.DeleteAsync($"api/Manufacturer/{manufacturerId}");
            if (response.IsSuccessStatusCode)
            {
                await LoadManufacturers();
            }
            else
            {
                errorMessage = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(errorMessage))
                    errorMessage = "Unable to delete manufacturer.";
            }
        }

        private async Task EditManufacturer(int manufacturerId)
        {
            errorMessage = null;

            if (manufacturers == null) return;
            manufacturerModal?.Manufacturer = manufacturers.FirstOrDefault(m => m.ManufacturerId == manufacturerId, new());
            OpenModal();
        }

        private IEnumerable<ManufacturerDTO> SortedManufacturers =>
            SortColumn switch
            {
                nameof(ManufacturerDTO.ManufacturerName) =>
                    SortAscending
                        ? manufacturers.OrderBy(m => m.ManufacturerName)
                        : manufacturers.OrderByDescending(m => m.ManufacturerName),

                _ => manufacturers
            };

    }
}
