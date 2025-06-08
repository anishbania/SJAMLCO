using System.ComponentModel.DataAnnotations;

namespace Insurance.ViewModels
{
    public class FiscalYearViewModel
    {
        public int Id { get; set; }
        [Required, Display(Name = "Name")]
        public string Name { get; set; }
        [Required, Display(Name = "Local Name")]
        public string LocalName { get; set; }
        [Required, Display(Name = "Starting Date(BS)")]
        public string BsStartDate { get; set; }
        [Required, Display(Name = "End Date(BS)")]
        public string BsEndDate { get; set; }
        [Display(Name = "Starting Date(AD)")]
        public Nullable<System.DateTime> AdStartDate { get; set; }
        [Display(Name = "End Date(AD)")]
        public Nullable<System.DateTime> AdEndDate { get; set; }
        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        [Display(Name = "Previous Fiscal Year")]
        public Nullable<int> PreviousFyId { get; set; }

    }
}
