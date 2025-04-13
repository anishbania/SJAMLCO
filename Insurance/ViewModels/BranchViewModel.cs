using System.ComponentModel.DataAnnotations;

namespace Insurance.ViewModels
{
    public class BranchViewModel
    {
        public int Id { get; set; }
        public int Code { get; set; }
        [Display(Name = "Branch Name")] 
        public string? BranchName { get; set; }
    }
}
