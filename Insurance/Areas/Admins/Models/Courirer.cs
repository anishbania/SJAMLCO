using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insurance.Areas.Admins.Models
{
    public class LogisticCategory
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string? Category { get; set; }

        public virtual ICollection<LogisticItem> LogisticItems { get; set; }
    }

    public class CourierVendor
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string VendorName { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(10)]
        public string? Phone { get; set; }
        public virtual ICollection<LogisticDispatch> ?LogisticDispatches { get; set; }
    }

    public class LogisticDispatch
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(3)]
        public string? BranchCode { get; set; }

        [Required]
        public DateTime DispatchDate { get; set; }

        [Required]
        public int VendorID { get; set; }

        [StringLength(15)]
        public string? Status { get; set; }

        [StringLength(50)]
        public string? SendBy { get; set; }

        [StringLength(100)]
        public string? ReceivedBy { get; set; }

        public DateTime? ReceivedDate { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }

        [StringLength(40)]
        public string RefNumber { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public string? SupportingFilePath { get; set; }

        [ForeignKey("VendorID")]
        public virtual CourierVendor? Vendor { get; set; }
        public virtual ICollection<LogisticItem> LogisticItems { get; set; }
    }

    public class LogisticItem
    {
        [Key]
        public int ID { get; set; }
        public int? DispatchId { get; set; }

        [StringLength(100)]
        public string? ItemName { get; set; }

        public decimal? Qty { get; set; }

        [StringLength(20)]
        public string? UnitType { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual LogisticCategory? Category { get; set; }
        [ForeignKey("DispatchId")]
        public virtual LogisticDispatch? Dispatch { get; set; }
    }
    //public class CourierSupportingFile
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public int DispatchId { get; set; }
    //    public string? SupportingFilePath { get; set; }
    //    public string? SupportingFileName { get; set; }      

    //    [ForeignKey("DispatchId")]
    //    public LogisticDispatch? LogisticDispatch { get; set; }


    //}
}
