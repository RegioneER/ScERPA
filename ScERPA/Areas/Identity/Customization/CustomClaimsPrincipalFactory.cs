using ScERPA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ScERPA.Areas.Identity.Customization
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public CustomClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
          
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user) 
        {
            ClaimsIdentity claims = await base.GenerateClaimsAsync(user);
            claims.AddClaim(new Claim(ClaimTypes.Surname, user.Cognome));
            claims.AddClaim(new Claim(ClaimTypes.GivenName, user.Nome));

            return claims;
        }
    }
}
