using System.ComponentModel.DataAnnotations;

namespace Insurance.Models
{
    public class FiscalYear
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string LocalName { get; set; }
        public string BsStartDate { get; set; }
        public string BsEndDate { get; set; }
        public Nullable<System.DateTime> AdStartDate { get; set; }
        public Nullable<System.DateTime> AdEndDate { get; set; }
        public Nullable<bool> Status { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> PreviousFyId { get; set; }
        public string? CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string? DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
    }
}
