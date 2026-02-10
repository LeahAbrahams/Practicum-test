using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.DB
{
    public class ServiceCallDbContext : DbContext
    {
        public ServiceCallDbContext(DbContextOptions<ServiceCallDbContext> options) : base(options) { }

        public DbSet<Office> Offices { get; set; }
        public DbSet<FieldOffice> FieldOffices { get; set; }
        public DbSet<ManagementOffice> ManagementOffices { get; set; }
        public DbSet<ServiceCall> ServiceCalls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // הגדרת קשר One-to-Zero-or-One בין Office ל-FieldOffice
            modelBuilder.Entity<FieldOffice>()
                .HasKey(f => f.OfficeID);

            modelBuilder.Entity<FieldOffice>()
                .HasOne(f => f.Office)
                .WithOne(o => o.FieldOffice)
                .HasForeignKey<FieldOffice>(f => f.OfficeID)
                .OnDelete(DeleteBehavior.Cascade);

            // הגדרת קשר One-to-Zero-or-One בין Office ל-ManagementOffice
            modelBuilder.Entity<ManagementOffice>()
                .HasKey(m => m.OfficeID);

            modelBuilder.Entity<ManagementOffice>()
                .HasOne(m => m.Office)
                .WithOne(o => o.ManagementOffice)
                .HasForeignKey<ManagementOffice>(m => m.OfficeID)
                .OnDelete(DeleteBehavior.Cascade);

            // הגדרת קשר One-to-Many בין Office ל-ServiceCalls
            modelBuilder.Entity<ServiceCall>()
                .HasOne(s => s.Office)
                .WithMany(o => o.ServiceCalls)
                .HasForeignKey(s => s.OfficeID)
                .OnDelete(DeleteBehavior.Cascade);

            //// Data Seeding - משרדים
            //modelBuilder.Entity<Office>().HasData(
            //    new Office { OfficeID = 1, OfficeName = "משרד מיסוי ירושלים", Phone = "02-6543210", OfficeType = "Field" },
            //    new Office { OfficeID = 2, OfficeName = "משרד מיסוי תל אביב", Phone = "03-7654321", OfficeType = "Field" },
            //    new Office { OfficeID = 3, OfficeName = "משרד ניהול מרכז", Phone = "03-9876543", OfficeType = "Management" }
            //);

            //// Data Seeding - משרדי שטח
            //modelBuilder.Entity<FieldOffice>().HasData(
            //    new FieldOffice { OfficeID = 1, HasPublicReception = true, ReceptionAddress = "רח' יפו 97, ירושלים" },
            //    new FieldOffice { OfficeID = 2, HasPublicReception = true, ReceptionAddress = "דרך מנחם בגין 125, תל אביב" }
            //);

            //// Data Seeding - משרדי ניהול
            //modelBuilder.Entity<ManagementOffice>().HasData(
            //    new ManagementOffice { OfficeID = 3, ProfessionalManager = "דוד כהן" }
            //);

            //// Data Seeding - קריאות שירות
            //modelBuilder.Entity<ServiceCall>().HasData(
            //    new ServiceCall { CallID = 1, OfficeID = 1, Description = "מקלדת לא עובדת", Status = "פתוח", CreatedAt = new DateTime(2024, 1, 15) },
            //    new ServiceCall { CallID = 2, OfficeID = 1, Description = "מדפסת תקועה", Status = "בטיפול", CreatedAt = new DateTime(2024, 1, 20) },
            //    new ServiceCall { CallID = 3, OfficeID = 2, Description = "בעיה במסך", Status = "טופל", CreatedAt = new DateTime(2024, 1, 10) }
            //);
        }
    }
}
