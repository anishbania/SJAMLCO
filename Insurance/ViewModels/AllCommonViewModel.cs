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
            [Display(Name = "प्रोजेक्टको नाम")]
            public string Name { get; set; } 
            [Required]
            [DisplayName("कार्यालयको नाम")]
            public string Slogan { get; set; }
            [Required]
            [EmailAddress]
            [Display(Name = "इमेल")]
            public string Email { get; set; }
            [Display(Name = "सम्पर्क व्यक्ति")]
            public string ContactName { get; set; }
            [Display(Name = "सम्पर्क व्यक्तिको मोबाइल नं")]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number")]
            public string ContactNumber { get; set; }
            [Display(Name = "फोन नं")]
            public string MobileNumber { get; set; }
            [Display(Name = "फ्याक्स नं")]
            public string FaxNo { get; set; }
            [Display(Name = "वेबसाइट")]
            public string Website { get; set; }
            [Display(Name = "ठेगाना")]
            [Required]
            public string Address { get; set; }
            [Display(Name = "ठेगाना (Eng)")]
            [Required]
            public string Address2 { get; set; }
            [Display(Name = "Logo")]
            public string LogoPath { get; set; }
            [Display(Name = "Logo")]
            public IFormFile ProfileImage { get; set; }

            [Display(Name = "प्रदेश")]
            public Nullable<int> State { get; set; } /*= 1;*/
            [Display(Name = "जिल्ला")]
            public Nullable<int> District { get; set; } /*= 1;*/
            [Display(Name = "गा.पा./न.पा.")]
            public Nullable<int> Palika { get; set; } /*= 1;*/
            [Display(Name = "वडा नं")]
            public int Ward { get; set; }
        }

    }
}
