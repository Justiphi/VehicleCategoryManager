using Microsoft.AspNetCore.Mvc;
using VehicleCategoryManager.Client.DTOs;
using VehicleCategoryManager.Persistance.Models;
using VehicleCategoryManager.Persistance.Services;

namespace VehicleCategoryManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightClassController : ControllerBase
    {
        private readonly WeightClassService _weightClassService;

        public WeightClassController(WeightClassService weightClassService)
        {
            _weightClassService = weightClassService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<WeightClassDTO>> Index()
        {
            var weightClasss = _weightClassService.GetWeightClassList();
            var weightClassDtos = weightClasss.Select(MapToDTO).ToList();
            return Ok(weightClassDtos);
        }

        [HttpGet("{id:int}")]
        public ActionResult<WeightClassDTO> Details(int id)
        {
            var weightClass = _weightClassService.GetWeightClassById(id);
            if (weightClass == null)
            {
                return NotFound();
            }
            return MapToDTO(weightClass);
        }

        [HttpPost]
        public ActionResult Create(WeightClassDTO weightClass)
        {
            try
            {
                int recordId = _weightClassService.AddWeightClass(MapToEntity(weightClass));
                if (recordId != 0)
                {
                    return Ok();
                }
                //fallback
                return BadRequest("Unknown Error: Failed to create weightClass");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Edit(int id, WeightClassDTO weightClass)
        {
            try
            {
                if (id != weightClass.VehicleWeightClassId)
                {
                    return BadRequest("WeightClass ID mismatch");
                }
    
                if (_weightClassService.UpdateWeightClass(MapToEntity(weightClass)))
                {
                    return Ok();
                }
                //fallback
                return BadRequest("Unknown Error: Failed to update weightClass");
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
                _weightClassService.DeleteWeightClass(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private WeightClassDTO MapToDTO(VehicleWeightClass weightClass)
        {
            return new WeightClassDTO
            {
                VehicleWeightClassId = weightClass.VehicleWeightClassId,
                VehicleWeightClassName = weightClass.VehicleWeightClassName,
                MaxWeightKg = weightClass.MaxWeightKg,
                MinWeightKg = weightClass.MinWeightKg,
                VehicleWeightClassIconPath = weightClass.VehicleWeightClassIconPath
            };
        }

        private VehicleWeightClass MapToEntity(WeightClassDTO weightClassDTO)
        {
            return new VehicleWeightClass
            (
                id: weightClassDTO.VehicleWeightClassId,
                name: weightClassDTO.VehicleWeightClassName,
                minWeightKg: weightClassDTO.MinWeightKg,
                maxWeightKg: weightClassDTO.MaxWeightKg,
                iconPath: weightClassDTO.VehicleWeightClassIconPath
            );
        }
    }
}
