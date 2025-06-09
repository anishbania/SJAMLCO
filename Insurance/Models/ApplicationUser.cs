using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insurance.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string PrayogkartaName { get; set; }
        public string Department  { get; set; }
        public bool? HasLoggedIn { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
