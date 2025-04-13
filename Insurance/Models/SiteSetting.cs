using System.ComponentModel.DataAnnotations;

namespace Insurance.Models
{
    public class SiteSetting
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string LogoPath { get; set; } = "/img/logo.png";
        public string FaxNo { get; set; }
        public string Website { get; set; }
        public string Slogan { get; set; }
        public DateTime? ExpireDate { get; set; }

        public Nullable<int> State { get; set; }
        public Nullable<int> District { get; set; }
        public Nullable<int> Palika { get; set; }
        public int Ward { get; set; }
         
    }
}
