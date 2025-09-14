using System.ComponentModel.DataAnnotations.Schema;

namespace Insurance.Areas.VMS.Models
{
    public class VisitDocument
    {
        public int Id { get; set; }
        public int VisitId { get; set; } // Foreign Key to Visit
        public string ? DartaChalani { get; set; } 
        public string DocumentName { get; set; } // Name of the document
        public string DocumentPath { get; set; } // Path to the document file
        public string DocumentType { get; set; } 
        public string UploadedBy { get; set; } 
        public string ScanType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } // Timestamp of when the document was uploaded
        [ForeignKey(nameof(VisitId))]
        public virtual Visit Visit { get; set; } 
    }
}
