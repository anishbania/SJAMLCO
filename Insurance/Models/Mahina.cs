using System.ComponentModel.DataAnnotations;

namespace Insurance.Models
{
    public class Mahina
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string NepaliName { get; set; }
    }
}
