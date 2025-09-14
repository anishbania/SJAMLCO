using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Insurance.Areas.VMS.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [DisplayName("Full Name")]
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
        [RegularExpression(@"^[A-Za-z\u00C0-\u024F' .-]{2,100}$", ErrorMessage = "Full name may contain letters, spaces, apostrophes, hyphens and periods.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required.")]
        public string Department { get; set; } = string.Empty;

        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256, ErrorMessage = "Email can't be longer than 256 characters.")]
        public string? Email { get; set; }

        [DisplayName("Phone Number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must contain exactly 10 digits.")]
        public string? PhoneNumber { get; set; } 

    }
}
