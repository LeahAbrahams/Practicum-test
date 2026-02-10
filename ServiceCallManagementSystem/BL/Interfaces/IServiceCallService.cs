using BL.DTOs;

namespace BL.Interfaces
{
    public interface IServiceCallService
    {
        Task<List<ServiceCallDTO>> GetAllServiceCallsAsync();
        Task<List<ServiceCallDTO>> GetServiceCallsByOfficeAsync(int officeId);
        Task<ServiceCallDTO> CreateServiceCallAsync(CreateServiceCallDTO createDto);
        Task<ServiceCallDTO> UpdateStatusAsync(int callId, string newStatus);
    }
}
