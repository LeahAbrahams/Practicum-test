namespace BL.DTOs
{
    public class OfficeWithServiceCallsDTO
    {
        public int OfficeID { get; set; }
        public string OfficeName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string OfficeType { get; set; } = string.Empty;
        public List<ServiceCallDTO> ServiceCalls { get; set; } = new List<ServiceCallDTO>();
    }
}
