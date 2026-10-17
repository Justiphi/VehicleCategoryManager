using Microsoft.AspNetCore.Mvc;
using VehicleCategoryManager.Client.DTOs;
using VehicleCategoryManager.Persistance.Models;
using VehicleCategoryManager.Persistance.Services;

namespace VehicleCategoryManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManufacturerController : ControllerBase
    {
        private readonly ManufacturerService _manufacturerService;

        public ManufacturerController(ManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ManufacturerDTO>> Index()
        {
            var manufacturers = _manufacturerService.GetManufacturerList();
            var manufacturerDtos = manufacturers.Select(MapToDTO).ToList();

            return Ok(manufacturerDtos);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ManufacturerDTO> Details(int id)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(id);
            if (manufacturer == null)
            {
                return NotFound();
            }
            return MapToDTO(manufacturer);
        }

        [HttpPost]
        public ActionResult Create(ManufacturerDTO manufacturer)
        {
            try
            {
                var manufacturerid = _manufacturerService.AddManufacturer(MapToEntity(manufacturer));
                if (manufacturerid != 0)
                {
                    return RedirectToAction("Index");
                }
                return BadRequest("Failed to create manufacturer");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in Create GET: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Edit(int id, ManufacturerDTO manufacturer)
        {
            if(id != manufacturer.ManufacturerId)
            {
                return BadRequest("Manufacturer ID mismatch");
            }

            if (_manufacturerService.UpdateManufacturer(MapToEntity(manufacturer)))
            {
                return Ok();
            }
            return BadRequest("Failed to update manufacturer");
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            if (_manufacturerService.DeleteManufacturer(id))
            {
                return Ok();
            }
            return BadRequest("Failed to delete manufacturer");
        }

        private ManufacturerDTO MapToDTO(Manufacturer manufacturer)
        {
            return new ManufacturerDTO
            {
                ManufacturerId = manufacturer.ManufacturerId,
                ManufacturerName = manufacturer.ManufacturerName
            };
        }

        private Manufacturer MapToEntity(ManufacturerDTO manufacturerDTO)
        {
            return new Manufacturer(manufacturerDTO.ManufacturerName, manufacturerDTO.ManufacturerId);
        }
    }
}
