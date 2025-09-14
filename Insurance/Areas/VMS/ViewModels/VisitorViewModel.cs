using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using Insurance.Areas.VMS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Insurance.Areas.VMS.ViewModels
{
    public class VisitorViewModel
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Visitor name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [DisplayName("Phone Number")]
        [StringLength(10, ErrorMessage = "Phone number must be 10 digits.", MinimumLength = 10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits (numbers only).")]
        public string PhoneNumber { get; set; } = string.Empty;


        [DisplayName("Email Address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256, ErrorMessage = "Email is too long.")]
        // Make email optional; when present it must be valid format
        public string? EmailAddress { get; set; }

        [DisplayName("Company")]
        [StringLength(150, ErrorMessage = "Company name is too long.")]
        public string? Company { get; set; }

        [DisplayName("Registered At")]
        [DataType(DataType.DateTime)]
        [ScaffoldColumn(false)] // hide from scaffolds/edit forms by default
        public DateTimeOffset ? CreatedDate { get; set; } = DateTimeOffset.UtcNow;

        // Navigation property — do not attempt to validate/bind children here for simple forms
        [ValidateNever]
        public ICollection<Visit>? Visits { get; set; } = new List<Visit>();
    }
}
