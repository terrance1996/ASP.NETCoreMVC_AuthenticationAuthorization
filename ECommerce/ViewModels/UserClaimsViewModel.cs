using System.Collections.Generic;

namespace ECommerce.ViewModels
{
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            this.Claims = new List<UserClaims>();
        }

        public string UserId { get; set; }
        public List<UserClaims> Claims { get; set; }
    }
}
