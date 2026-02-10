using BL.DTOs;
using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ServiceCallManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficesController : ControllerBase
    {
        private readonly IOfficeService _officeService;

        // Constructor Injection
        public OfficesController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOffices()
        {
            var offices = await _officeService.GetAllOfficesAsync();
            return Ok(offices);
        }

        // API 2: כל המשרדים + כמות קריאות פתוחות (לפני {id}!)
        [HttpGet("with-open-calls-count")]
        public async Task<IActionResult> GetAllOfficesWithOpenCallsCount()
        {
            var offices = await _officeService.GetAllOfficesWithOpenCallsCountAsync();
            return Ok(offices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfficeById(int id)
        {
            var office = await _officeService.GetOfficeByIdAsync(id);
            if (office == null)
                return NotFound(new { message = $"משרד עם מזהה {id} לא נמצא" });

            return Ok(office);
        }

        // API 1: משרד ספציפי + כל קריאות השירות שלו
        [HttpGet("{id}/with-service-calls")]
        public async Task<IActionResult> GetOfficeWithServiceCalls(int id)
        {
            var office = await _officeService.GetOfficeWithServiceCallsAsync(id);
            if (office == null)
                return NotFound(new { message = $"משרד עם מזהה {id} לא נמצא" });

            return Ok(office);
        }
    }
}
