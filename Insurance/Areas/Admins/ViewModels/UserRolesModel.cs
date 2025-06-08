using System.ComponentModel.DataAnnotations;

namespace Insurance.Areas.Admins.ViewModels
{
    public class UserRolesModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
