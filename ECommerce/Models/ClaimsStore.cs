using System.Collections.Generic;
using System.Security.Claims;

namespace ECommerce.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> Allclaims = new List<Claim>()
        {
            new Claim("Get Claim","Get Claim"),
            new Claim("Create Claim","Create Claim"),
            new Claim("Edit Claim","Edit Role"),
            new Claim("Delete Claim","Delete Claim")
        };
    }
}
