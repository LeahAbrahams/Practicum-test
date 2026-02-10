namespace BL.DTOs
{
    public class ServiceCallDTO
    {
        public int CallID { get; set; }
        public int OfficeID { get; set; }
        public string OfficeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        // Calculated Field - סטטוס דחיפות
        public string UrgencyLevel
        {
            get
            {
                var daysSinceCreated = (DateTime.Now - CreatedAt).Days;
                if (Status == "פתוח" && daysSinceCreated > 7)
                    return "דחוף";
                if (Status == "פתוח" && daysSinceCreated > 3)
                    return "בינוני";
                if (Status == "פתוח")
                    return "רגיל";
                return "לא רלוונטי";
            }
        }
    }
}
