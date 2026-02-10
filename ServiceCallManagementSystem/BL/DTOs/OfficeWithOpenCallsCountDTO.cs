namespace BL.DTOs
{
    public class OfficeWithOpenCallsCountDTO
    {
        public int OfficeID { get; set; }
        public string OfficeName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string OfficeType { get; set; } = string.Empty;
        public int OpenCallsCount { get; set; }
    }
}
