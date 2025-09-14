using System.ComponentModel.DataAnnotations;

namespace Insurance.Areas.VMS.Models
{
    public class Visitor
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? Company { get; set; }
        public DateTimeOffset? CreatedDate { get; set; } 
        // Navigation property
        public ICollection<Visit> Visits { get; set; }
    }
}
