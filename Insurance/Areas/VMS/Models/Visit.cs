using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insurance.Areas.VMS.Models
{
    public class Visit
    {
        public int Id { get; set; }
        public int VisitorId { get; set; } // Key
        public int EmployeeId { get; set; } // Key
        public string Purpose { get; set; }
        public string? Department { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeSpan? LengthOfMeeting { get; set; }
        public DateOnly? CheckInDate { get; set; }
        public TimeOnly? CheckOutTime { get; set; }

        public VisitStatus Status { get; set; }
        public VisitType VisitType { get; set; }
        public string? Notes { get; set; } // Receptionist notes
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? QrCodePath { get; set; } // relative path to saved QR image
        public string? PublicToken { get; set; } // random token for anonymous access (short GUID)

        [ForeignKey(nameof(VisitorId))]    //properties
        public Visitor Visitor { get; set; }
        [ForeignKey(nameof(EmployeeId))]    // Navigation properties
        public Employee EmployeeToMeet { get; set; }
    }
    public enum VisitStatus { Pending, Approved, CheckedIn, CheckedOut, Cancelled }
    public enum VisitType { InPerson, DocDelivery }
}
