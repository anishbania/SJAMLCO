using System.ComponentModel.DataAnnotations;

namespace Insurance.Areas.VMS.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; } // For notifications
        public string ?PhoneNumber { get; set; } // For notifications
    }
}
