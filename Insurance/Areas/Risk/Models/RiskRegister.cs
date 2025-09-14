using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insurance.Areas.Risk.Models
{
    public class RiskRegister
    {
        [Key]
        public int ID { get; set; }
        public  string? RiskID { get; set; }
        public  string? Username { get; set; }
        public DateTime? RegisterDate { get; set; }
        public string? RiskDescription { get; set; }
        public string? Department { get; set; }
        public string? PrimaryRisk { get; set; }
        public string? SecondaryRisk { get; set; }
        public string? LikeHood { get; set; }
        public string? Impact { get; set; }
        public string? RiskOwner { get; set; }
        public string? MitigationAction { get; set; }
        public string? RiskStatus { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? Quantification { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ? CreatedBy { get; set; }
        public string ? UpdatedBy { get; set; }
        public string? Remarks { get; set; }
        public string? RiskResponse { get; set; }
        public int? LikehoodId { get; set; }
        public int? ImpactId { get; set; }

        [ForeignKey("LikehoodId")]
        public Likehood? Likehood { get; set; }

        [ForeignKey("ImpactId")]
        public Impact? Impacts { get; set; }

    }
}
