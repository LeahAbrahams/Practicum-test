using BL.DTOs;
using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ServiceCallManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCallsController : ControllerBase
    {
        private readonly IServiceCallService _serviceCallService;

        // Constructor Injection
        public ServiceCallsController(IServiceCallService serviceCallService)
        {
            _serviceCallService = serviceCallService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServiceCalls()
        {
            var calls = await _serviceCallService.GetAllServiceCallsAsync();
            return Ok(calls);
        }

        [HttpGet("office/{officeId}")]
        public async Task<IActionResult> GetServiceCallsByOffice(int officeId)
        {
            var calls = await _serviceCallService.GetServiceCallsByOfficeAsync(officeId);
            return Ok(calls);
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceCall([FromBody] CreateServiceCallDTO createDto)
        {
            var createdCall = await _serviceCallService.CreateServiceCallAsync(createDto);
            return CreatedAtAction(nameof(GetAllServiceCalls), new { id = createdCall.CallID }, createdCall);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var updatedCall = await _serviceCallService.UpdateStatusAsync(id, request.NewStatus);
            return Ok(updatedCall);
        }
    }

    // DTO לעדכון סטטוס
    public class UpdateStatusRequest
    {
        public string NewStatus { get; set; } = string.Empty;
    }
}
