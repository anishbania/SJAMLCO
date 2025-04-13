using System.ComponentModel.DataAnnotations;

namespace Insurance.Models
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public Nullable<DateTime> UpdatedAt { get; set; }
    }
}
