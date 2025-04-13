using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Insurance.Models
{
    public class District
    {
        [Key]
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int? StateId { get; set; }
        public string DistrictName_Nep { get; set; }


        [ForeignKey("StateId")]
        public virtual State State { get; set; }
    }
}

