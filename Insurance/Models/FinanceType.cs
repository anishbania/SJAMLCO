using System.ComponentModel.DataAnnotations;

namespace Insurance.Models
{
    public class FinanceType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
