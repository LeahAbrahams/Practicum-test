using BL.DTOs;

namespace BL.Interfaces
{
    public interface IOfficeService
    {
        Task<List<OfficeDTO>> GetAllOfficesAsync();
        Task<OfficeDTO?> GetOfficeByIdAsync(int officeId);
        Task<OfficeDTO> CreateOfficeAsync(OfficeDTO officeDto);
        Task<OfficeWithServiceCallsDTO?> GetOfficeWithServiceCallsAsync(int officeId);
        Task<List<OfficeWithOpenCallsCountDTO>> GetAllOfficesWithOpenCallsCountAsync();
    }
}
