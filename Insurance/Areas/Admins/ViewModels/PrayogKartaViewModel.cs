using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Insurance.Areas.Admins.ViewModels
{
    public class PrayogKartaViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public string PrayogkartaName { get; set; }
        [Required]
        [Display(Name = "User Role")]
        public string Role { get; set; }

        [Phone]
        [Display(Name = "Mobile Number")]
        public string PhoneNumber { get; set; } = null!;

        [Display(Name = "Is Logged In?")]
        public bool? HasLogged { get; set; }
        public DateTime? LastLogged { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }


    }
}
