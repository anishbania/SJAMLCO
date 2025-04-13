using System.ComponentModel.DataAnnotations;

namespace Insurance.ViewModels
{
    public class FiscalYearViewModel
    {
        public int Id { get; set; }
        [Required, Display(Name = "नाम")]
        public string Name { get; set; }
        [Required, Display(Name = "स्थानिय नाम")]
        public string LocalName { get; set; }
        [Required, Display(Name = "सुरूको मिति")]
        public string BsStartDate { get; set; }
        [Required, Display(Name = "अन्तिम मिति")]
        public string BsEndDate { get; set; }
        [Display(Name = "सुरूको मिति(English)")]
        public Nullable<System.DateTime> AdStartDate { get; set; }
        [Display(Name = "अन्तिम मिति(English)")]
        public Nullable<System.DateTime> AdEndDate { get; set; }
        [Display(Name = "स्थिति")]
        public bool IsActive { get; set; }
        [Display(Name = "अघिल्लो वित्तीय वर्ष")]
        public Nullable<int> PreviousFyId { get; set; }

    }
}
