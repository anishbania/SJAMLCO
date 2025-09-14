using Insurance.Areas.VMS.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insurance.Areas.VMS.ViewModels
{
    public class VisitDocumentViewModel
    {
        public int Id { get; set; }
        public int VisitId { get; set; } // Foreign Key to Visit
        public string ? DartaChalani { get; set; } // Darta Chalani number or identifier
        [DisplayName("Document Name")]
        public string DocumentName { get; set; } // Name of the document
        public string DocumentPath { get; set; } // Path to the document file
        [DisplayName("Document Type")]
        public string DocumentType { get; set; }
        public string UploadedBy { get; set; }
        [DisplayName("Scan Type")]
        public string ScanType { get; set; }    
        public long FileSize { get; set; }
        public IFormFile File { get; set; } // For file upload handling
        public DateTime UploadedAt { get; set; } // Timestamp of when the document was uploaded
        [ForeignKey(nameof(VisitId))]
        public virtual Visit Visit { get; set; } // Navigation property to Visit    
    }
}
