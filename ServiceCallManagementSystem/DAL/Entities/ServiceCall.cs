using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class ServiceCall
    {
        [Key] // התגית הזו מגדירה מפתח ראשי באופן מפורש
        public int CallID { get; set; }
        public int OfficeID { get; set; }
        public string Description { get; set; } = string.Empty;

        [MaxLength(20)] // מגביל את אורך הסטטוס
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Office Office { get; set; } = null!;
    }
}
