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
            }
            catch (HttpRequestException)
            {
                errorMessage = "Unable to load manufacturers.";
            }
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
                errorMessage = "Unable to delete manufacturer.";
            }
        }

        private async Task EditManufacturer(int manufacturerId)
        {
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
