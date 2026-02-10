using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class FieldOffice
    {
        [Key] // התגית הזו מגדירה מפתח ראשי באופן מפורש
        public int OfficeID { get; set; }
        public bool HasPublicReception { get; set; }
        public string? ReceptionAddress { get; set; } // הנחה: אין 2 משרדים באותו אזור כך שחלוקה של הכתובת לא תהיה יעילה

        public Office Office { get; set; } = null!;
    }
}
