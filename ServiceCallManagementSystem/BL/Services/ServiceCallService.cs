using BL.DTOs;
using BL.Exceptions;
using BL.Interfaces;
using DAL.DB;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class ServiceCallService : IServiceCallService
    {
        private readonly ServiceCallDbContext _context;

        public ServiceCallService(ServiceCallDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceCallDTO>> GetAllServiceCallsAsync()
        {
            var calls = await _context.ServiceCalls
                .Include(s => s.Office)
                .ToListAsync();
            
            return calls.Select(MapToDTO).ToList();
        }

        public async Task<List<ServiceCallDTO>> GetServiceCallsByOfficeAsync(int officeId)
        {
            var calls = await _context.ServiceCalls
                .Include(s => s.Office)
                .Where(s => s.OfficeID == officeId)
                .ToListAsync();
            
            return calls.Select(MapToDTO).ToList();
        }

        public async Task<ServiceCallDTO> CreateServiceCallAsync(CreateServiceCallDTO createDto)
        {
            // Validation - בדיקת קיום משרד
            var officeExists = await _context.Offices.AnyAsync(o => o.OfficeID == createDto.OfficeID);
            if (!officeExists)
                throw new ValidationException($"משרד עם מזהה {createDto.OfficeID} לא קיים במערכת");

            // Validation - תיאור לא ריק
            if (string.IsNullOrWhiteSpace(createDto.Description))
                throw new ValidationException("תיאור הקריאה הוא שדה חובה");

            var serviceCall = new ServiceCall
            {
                OfficeID = createDto.OfficeID,
                Description = createDto.Description,
                Status = "פתוח",
                CreatedAt = DateTime.Now
            };

            _context.ServiceCalls.Add(serviceCall);
            await _context.SaveChangesAsync();

            // טעינת Office למיפוי
            await _context.Entry(serviceCall).Reference(s => s.Office).LoadAsync();

            return MapToDTO(serviceCall);
        }

        public async Task<ServiceCallDTO> UpdateStatusAsync(int callId, string newStatus)
        {
            var call = await _context.ServiceCalls
                .Include(s => s.Office)
                .FirstOrDefaultAsync(s => s.CallID == callId);

            if (call == null)
                throw new ValidationException($"קריאת שירות עם מזהה {callId} לא נמצאה");

            // Validation - סטטוס תקין
            var validStatuses = new[] { "פתוח", "בטיפול", "טופל" };
            if (!validStatuses.Contains(newStatus))
                throw new ValidationException($"סטטוס לא תקין. ערכים מותרים: {string.Join(", ", validStatuses)}");

            call.Status = newStatus;
            await _context.SaveChangesAsync();

            return MapToDTO(call);
        }

        // Manual Mapping: Entity -> DTO (כולל Calculated Field)
        private ServiceCallDTO MapToDTO(ServiceCall call)
        {
            return new ServiceCallDTO
            {
                CallID = call.CallID,
                OfficeID = call.OfficeID,
                OfficeName = call.Office?.OfficeName ?? string.Empty,
                Description = call.Description,
                Status = call.Status,
                CreatedAt = call.CreatedAt
                // UrgencyLevel מחושב אוטומטית ב-DTO
            };
        }
    }
}
