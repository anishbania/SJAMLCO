using Insurance.Areas.VMS.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insurance.Areas.VMS.ViewModels
{
    public class VisitViewModel
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Visitor Name")]
        public int VisitorId { get; set; } // Foreign Key
        [DisplayName("Person To Meet")]
        public int  EmployeeId { get; set; } // Foreign Key
        public string Purpose { get; set; }
        public string? Department { get; set; }
        [DisplayName("CheckIn Time")]
        public TimeOnly? CheckInTime { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
        [DisplayName("CheckIn Date")]
        public DateOnly? CheckInDate { get; set; }
        [DisplayName("Length of Meeting")]
        public TimeSpan? LengthOfMeeting { get; set; }
        [DisplayName("CheckOut Time")]
        public TimeOnly? CheckOutTime { get; set; }
        public VisitStatus Status { get; set; }
        [DisplayName("Visit Type")]
        public VisitType VisitType { get; set; }
        public string? Notes { get; set; } // Receptionist notes
        public string? CreatedBy { get; set; }
        [DisplayName("Created At")]
        public DateTime? CreatedAt { get; set; }

        [ForeignKey(nameof(VisitorId))]    // Navigation properties
        public Visitor Visitor { get; set; }
        [ForeignKey(nameof(EmployeeId))]    // Navigation properties
        public Employee EmployeeToMeet { get; set; }
        public virtual VisitorViewModel ?  VisitorDetails { get; set; }
        public virtual VisitDocumentViewModel? VisitDocument { get; set; }
        public virtual ICollection<VisitDocument> Documents { get; set; } = new List<VisitDocument>();
    }
}
