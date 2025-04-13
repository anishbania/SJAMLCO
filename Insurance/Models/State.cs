using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Insurance.Models
{
    public class State
    {
        [Key]
        public int StateId { get; set; }
        public string StateName { get; set; }
        public string StateName_Nep { get; set; }
    }
}
