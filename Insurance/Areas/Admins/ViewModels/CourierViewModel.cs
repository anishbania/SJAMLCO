using Insurance.Areas.Admins.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insurance.Areas.Admins.ViewModels
{
    public class LogisticDispatchViewModel
    {
        public int Id { get; set; }
        public int SequenceNumber { get; set; }
        public string ChalaniNo { get; set; }
        public string DartaNo { get; set; }
        [DisplayName("Vendor Name")]
        public int VendorID { get; set; }
        [DisplayName("Branch Code")]
        public string? BranchCode { get; set; }
        [DisplayName("Branch Name")]
        public string? BranchName { get; set; }
        [DisplayName("Vendor Name")]
        public string? VendorName { get; set; }
        [DisplayName("Dispatch Date")]
        public DateTime DispatchDate { get; set; }
        [DisplayName("Mode Of Courier")]
        public string? ModeOfCourier { get; set; }
        public string? Status { get; set; }
        [DisplayName("Dispatched By")]
        public string? SendBy { get; set; }
        [DisplayName("Received By")]
        public string? ReceivedBy { get; set; }
        [DisplayName("Received Date")]
        public DateTime? ReceivedDate { get; set; }
        public string? Remarks { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [DisplayName("Supporting File")]
        public string? SupportingFilePath { get; set; }
        public List<IFormFile>? SupportingFile { get; set; } = new List<IFormFile>();

        public CourierVendorViewModel Vendor { get; set; } = new CourierVendorViewModel();
        public List<LogisticItemViewModel> Items { get; set; } = new List<LogisticItemViewModel>();
    }

    public class CourierVendorViewModel
    {
        public int Id { get; set; }
        [DisplayName("Vendor Name")]
        public string VendorName { get; set; }
        public string ?Email { get; set; }
        public string? Phone { get; set; }
    }
    public class LogisticCategoryViewModel
    {
        public int Id { get; set; }
        public string ?Category { get; set; }
    }

    public class LogisticItemViewModel
    {
        public int ID { get; set; }
        [DisplayName("Item Name")]
        public string? ItemName { get; set; }
        [DisplayName("Quantity")]
        public int? Qty { get; set; }
        [DisplayName("Unit Type")]
        public string ?UnitType { get; set; }
        public int CategoryID { get; set; }
        [DisplayName("Category Name")]
        public string ?CategoryName { get; set; }
    }
}
