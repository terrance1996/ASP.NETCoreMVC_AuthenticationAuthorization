using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Userame { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        public List<string> Claims { get; set; }
        public IList<string> Roles { get; set; }
    }
}
