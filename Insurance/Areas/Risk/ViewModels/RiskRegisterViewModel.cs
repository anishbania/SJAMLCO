using Insurance.Areas.Risk.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insurance.Areas.Risk.ViewModels
{
    public class RiskRegisterViewModel
    {
        public int ID { get; set; }
        [DisplayName("Risk ID")]
        public string RiskID { get; set; }

        [DisplayName("Register Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RegisterDate { get; set; } = DateTime.Now;

        [DisplayName("Risk Description")]
        [Required]
        public string RiskDescription { get; set; }
        [Required]
        [DisplayName("Department")]
        public string Department { get; set; }

        [DisplayName("Primary Risk")]
        [Required]
        public string PrimaryRisk { get; set; }

        [DisplayName("Secondary Risk")]
        [Required]
        public string SecondaryRisk { get; set; }

        [DisplayName("LikeHood")]
        [Required]
        public string LikeHood { get; set; }

        [DisplayName("Impact")]
        [Required]
        public string Impact { get; set; }

        [DisplayName("Risk Owner")]
        [Required]
        public string RiskOwner { get; set; }

        [DisplayName("Mitigation Action")]
        [Required]
        public string MitigationAction { get; set; }

        [DisplayName("Risk Status")]
        [Required]
        public string RiskStatus { get; set; }

        [DisplayName("Closed Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ClosedDate { get; set; } 

        [DisplayName("Quantification")]
        public int Quantification { get; set; }

        [DisplayName("Updated Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedDate { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        [DisplayName("Risk Response")]
        public string RiskResponse { get; set; }
        [DisplayName("Matrix")]
        public string Matrix { get; set; }
        [DisplayName("Likehood Score")]
        public int LikeHoodScore { get; set; }
        [DisplayName("Impact Score")]
        public int ImpactScore { get; set; }
        public string RiskLevel
        {
            get
            {
                int score = LikeHoodScore * ImpactScore;

                if (score >= 9) return "HIGH";
                if (score >= 4) return "MEDIUM";
                return "LOW";
            }
        }
        public int LikehoodId { get; set; }
        public int ImpactId { get; set; }        

    }
}
