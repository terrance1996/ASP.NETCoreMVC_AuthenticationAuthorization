using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
