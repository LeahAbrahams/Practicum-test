using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Office
    {
        [Key] // התגית הזו מגדירה מפתח ראשי באופן מפורש
        public int OfficeID { get; set; }

        [MaxLength(50)] // מגביל את אורך השם
        public string OfficeName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        [MaxLength(20)] // גם אם בעתיד יהיו שמות יותר ארוכים יש גבול לאורך
        public string OfficeType { get; set; } = string.Empty;

        public FieldOffice? FieldOffice { get; set; }
        public ManagementOffice? ManagementOffice { get; set; }
        public ICollection<ServiceCall> ServiceCalls { get; set; } = new List<ServiceCall>();
    }
}
