using System.ComponentModel.DataAnnotations;

namespace Insurance.Models
{
    public class Gender
    {
        [Key]
        public int GenderId { get; set; }
        public string GenderNepali { get; set; }
        public string GenderEnglish { get; set; }

    }
}
