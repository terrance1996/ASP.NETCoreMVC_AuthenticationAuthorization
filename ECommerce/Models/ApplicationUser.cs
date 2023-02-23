using Microsoft.AspNetCore.Identity;

namespace ECommerce.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FName { get; set; }
        public string UName { get; set; }
    }
}
