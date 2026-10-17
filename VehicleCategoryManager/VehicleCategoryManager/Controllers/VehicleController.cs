using Microsoft.AspNetCore.Mvc;
using VehicleCategoryManager.Client.DTOs;
using VehicleCategoryManager.Persistance.Models;
using VehicleCategoryManager.Persistance.Services;

namespace VehicleCategoryManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly VehicleService _vehicleService;
        private readonly WeightClassService _weightClassService;

        public VehicleController(VehicleService vehicleService, WeightClassService weightClassService)
        {
            _vehicleService = vehicleService;
            _weightClassService = weightClassService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VehicleDTO>> Index()
        {
            var vehicles = _vehicleService.GetVehicleList();
            var vehicleDTOs = vehicles.Select(MapToDTO).ToList();

            return Ok(vehicleDTOs);
        }

        [HttpGet("{id:int}")]
        public ActionResult<VehicleDTO> Details(int id)
        {
            var vehicle = _vehicleService.GetVehicleById(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return MapToDTO(vehicle);
        }

        [HttpPost]
        public ActionResult<int> Create(VehicleDTO vehicle)
        {
            try
            {
                int vehicleId = _vehicleService.AddVehicle(MapToEntity(vehicle));
                if (vehicleId != 0)
                {
                    return NoContent();
                }
                //fallback
                return BadRequest("Unknown Error: Failed to create vehicle");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Edit(int id, VehicleDTO vehicle)
        {
            try
            {
                if (id != vehicle.VehicleId)
                {
                    return BadRequest("Vehicle ID mismatch");
                }

                if (_vehicleService.UpdateVehicle(MapToEntity(vehicle)))
                {
                    return NoContent();
                }
                return BadRequest("Failed to update vehicle");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            try
            {
                _vehicleService.DeleteVehicle(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private VehicleDTO MapToDTO(Vehicle vehicle)
        {
            return new VehicleDTO
            {
                VehicleId = vehicle.VehicleId,
                WeightKg = vehicle.WeightKg,
                Owner = vehicle.Owner,
                Year = vehicle.Year,
                ManufacturerName = vehicle.Manufacturer?.ManufacturerName ?? string.Empty,
                ManufacturerId = vehicle.ManufacturerId,
                WeightCategoryIconPath = _weightClassService.GetWeightClassByWeight(vehicle.WeightKg)?.VehicleWeightClassIconPath ?? string.Empty
            };
        }

        private Vehicle MapToEntity(VehicleDTO vehicleDTO)
        {
            return new Vehicle
                (
                    id: vehicleDTO.VehicleId,
                    weightKg: vehicleDTO.WeightKg,
                    owner: vehicleDTO.Owner,
                    year: vehicleDTO.Year,
                    manufacturerId: vehicleDTO.ManufacturerId
                );
        }

    }
}
