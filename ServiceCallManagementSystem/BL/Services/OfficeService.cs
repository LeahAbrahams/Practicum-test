using BL.DTOs;
using BL.Exceptions;
using BL.Interfaces;
using DAL.DB;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BL.Services
{
    public class OfficeService : IOfficeService
    {
        private readonly ServiceCallDbContext _context;

        public OfficeService(ServiceCallDbContext context)
        {
            _context = context;
        }

        public async Task<List<OfficeDTO>> GetAllOfficesAsync()
        {
            var offices = await _context.Offices.ToListAsync();
            return offices.Select(MapToDTO).ToList();
        }

        public async Task<OfficeDTO?> GetOfficeByIdAsync(int officeId)
        {
            var office = await _context.Offices.FindAsync(officeId);
            return office == null ? null : MapToDTO(office);
        }

        public async Task<OfficeDTO> CreateOfficeAsync(OfficeDTO officeDto)
        {
            // Validation - מספר טלפון מכיל רק ספרות ומקפים
            ValidatePhone(officeDto.Phone);

            var office = MapToEntity(officeDto);
            _context.Offices.Add(office);
            await _context.SaveChangesAsync();

            return MapToDTO(office);
        }

        // API 1: שליפת משרד + כל קריאות השירות שלו
        public async Task<OfficeWithServiceCallsDTO?> GetOfficeWithServiceCallsAsync(int officeId)
        {
            var office = await _context.Offices
                .Include(o => o.ServiceCalls)
                .FirstOrDefaultAsync(o => o.OfficeID == officeId);

            if (office == null)
                return null;

            return new OfficeWithServiceCallsDTO
            {
                OfficeID = office.OfficeID,
                OfficeName = office.OfficeName,
                Phone = office.Phone,
                OfficeType = office.OfficeType,
                ServiceCalls = office.ServiceCalls.Select(sc => new ServiceCallDTO
                {
                    CallID = sc.CallID,
                    OfficeID = sc.OfficeID,
                    OfficeName = office.OfficeName,
                    Description = sc.Description,
                    Status = sc.Status,
                    CreatedAt = sc.CreatedAt
                }).ToList()
            };
        }

        // API 2: רשימת משרדים + כמות קריאות פתוחות
        public async Task<List<OfficeWithOpenCallsCountDTO>> GetAllOfficesWithOpenCallsCountAsync()
        {
            var offices = await _context.Offices
                .Include(o => o.ServiceCalls)
                .ToListAsync();

            return offices.Select(o => new OfficeWithOpenCallsCountDTO
            {
                OfficeID = o.OfficeID,
                OfficeName = o.OfficeName,
                Phone = o.Phone,
                OfficeType = o.OfficeType,
                OpenCallsCount = o.ServiceCalls.Count(sc => sc.Status == "פתוח")
            }).ToList();
        }

        // Validation Method
        private void ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ValidationException("מספר טלפון הוא שדה חובה");

            // בדיקה שמכיל רק ספרות ומקפים
            if (!Regex.IsMatch(phone, @"^[0-9\-]+$"))
                throw new ValidationException("מספר טלפון חייב להכיל רק ספרות ומקפים");
        }

        // Manual Mapping: Entity -> DTO
        private OfficeDTO MapToDTO(Office office)
        {
            return new OfficeDTO
            {
                OfficeID = office.OfficeID,
                OfficeName = office.OfficeName,
                Phone = office.Phone,
                OfficeType = office.OfficeType
            };
        }

        // Manual Mapping: DTO -> Entity
        private Office MapToEntity(OfficeDTO dto)
        {
            return new Office
            {
                OfficeID = dto.OfficeID,
                OfficeName = dto.OfficeName,
                Phone = dto.Phone,
                OfficeType = dto.OfficeType
            };
        }
    }
}
