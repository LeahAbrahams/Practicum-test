using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class ManagementOffice
    {
        [Key] // התגית הזו מגדירה מפתח ראשי באופן מפורש
        public int OfficeID { get; set; }

        [MaxLength(50)] // מגביל את אורך השם
        public string ProfessionalManager { get; set; } = string.Empty;

        public Office Office { get; set; } = null!;
    }
}
