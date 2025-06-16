using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Insurance.ViewModels
{
    public class AllCommonViewModel
    {
        public class SiteSettingViewModel
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; } 
            [Required]
            public string Slogan { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Display(Name = "Contact Name")]
            public string ContactName { get; set; }
            [Display(Name = "Contact Number")]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number")]
            public string ContactNumber { get; set; }
            [Display(Name = "Mobile Number")]
            public string MobileNumber { get; set; }
            [Display(Name = "Fax No")]
            public string FaxNo { get; set; }
            [Display(Name = "Website")]
            public string Website { get; set; }
            [Display(Name = "ठेगाना")]
            [Required]
            public string Address { get; set; }
            [Display(Name = "Address (Eng)")]
            [Required]
            public string Address2 { get; set; }
            [Display(Name = "Logo")]
            public string LogoPath { get; set; }
            [Display(Name = "Logo")]
            public IFormFile ProfileImage { get; set; }

            [Display(Name = "State")]
            public Nullable<int> State { get; set; } /*= 1;*/
            [Display(Name = "District")]
            public Nullable<int> District { get; set; } /*= 1;*/
            [Display(Name = "गा.पा./न.पा.")]
            public Nullable<int> Palika { get; set; } /*= 1;*/
            [Display(Name = "Ward No")]
            public int Ward { get; set; }
        }

    }
}
