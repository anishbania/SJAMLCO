using System.ComponentModel.DataAnnotations;

namespace Insurance.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }
        public int Code { get; set; }
        public string? Name { get; set; }

    }
}
