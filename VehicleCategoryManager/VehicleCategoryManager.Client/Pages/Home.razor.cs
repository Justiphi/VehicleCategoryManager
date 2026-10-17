using System.Net.Http.Json;
using VehicleCategoryManager.Client.Components;
using VehicleCategoryManager.Client.DTOs;

namespace VehicleCategoryManager.Client.Pages
{
    public partial class Home : BasePage
    {
        private AddVehicleModal? vehicleModal { get; set; }

        List<VehicleDTO> vehicles = new();

        protected override async Task OnInitializedAsync()
        {
            SortColumn = nameof(VehicleDTO.Owner);
            await LoadVehicles();
        }

        private async Task LoadVehicles()
        {
            try
            {
                vehicles = await Http.GetFromJsonAsync<List<VehicleDTO>>("api/Vehicle") ?? new();
                errorMessage = null;
                loadErrorMessage = null;
            }
            catch (HttpRequestException)
            {
                loadErrorMessage = "Unable to load vehicles.";
            }
        }

        private async Task DeleteVehicle(int vehicleId)
        {
            HttpResponseMessage response = await Http.DeleteAsync($"api/Vehicle/{vehicleId}");
            if (response.IsSuccessStatusCode)
            {
                await LoadVehicles();
            }
            else
            {
                errorMessage = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(errorMessage))
                    errorMessage = "Unable to delete vehicle.";
            }
        }

        private async Task EditVehicle(int vehicleId)
        {
            errorMessage = null;

            if (vehicles == null) return;
            vehicleModal?.Vehicle = vehicles.FirstOrDefault(v => v.VehicleId == vehicleId, new());
            OpenModal();
        }

        private IEnumerable<VehicleDTO> SortedVehicles =>
            SortColumn switch
            {
                nameof(VehicleDTO.Owner) =>
                    SortAscending
                        ? vehicles.OrderBy(v => v.Owner)
                        : vehicles.OrderByDescending(v => v.Owner),

                nameof(VehicleDTO.Year) =>
                    SortAscending
                        ? vehicles.OrderBy(v => v.Year)
                        : vehicles.OrderByDescending(v => v.Year),

                nameof(VehicleDTO.ManufacturerName) =>
                    SortAscending
                        ? vehicles.OrderBy(v => v.ManufacturerName)
                        : vehicles.OrderByDescending(v => v.ManufacturerName),

                nameof(VehicleDTO.WeightCategoryIconPath) =>
                    SortAscending
                        ? vehicles.OrderBy(v => v.WeightKg)
                        : vehicles.OrderByDescending(v => v.WeightKg),

                nameof(VehicleDTO.WeightKg) =>
                    SortAscending
                        ? vehicles.OrderBy(v => v.WeightKg)
                        : vehicles.OrderByDescending(v => v.WeightKg),

                _ => vehicles
            };

    }
}
