using Microsoft.AspNetCore.Identity;

namespace PadyLife.Api.Models.Users.Validator
{
    public class CustomUserValidator<TUser> : UserValidator<TUser>
     where TUser : IdentityUser
    {
        public override async Task<IdentityResult> ValidateAsync(
            UserManager<TUser> manager,
            TUser user
        )
        {
            // Skip all email checks
            return IdentityResult.Success;
        }
    }
}
