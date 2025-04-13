using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Insurance.Models
{
    public class Palika
    {
        [Key]
        public int PalikaId { get; set; }
        public Nullable<int> DistrictId { get; set; }
        public string PalikaName { get; set; }
        public string PalikaName_Nep { get; set; }

        [ForeignKey("DistrictId")]
        public virtual District District { get; set; }
    }
}
