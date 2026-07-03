using Entities.Users;
using System.Security.Claims;

namespace Services.Services.JwtServices
{
    public interface IJwtService
    {
        Task<AccessToken> GenerateAsync(User user);
        ClaimsPrincipal ValidateToken(string token);
        ClaimsPrincipal ReadTokenWithoutValidation(string token);
        DateTime? GetTokenExpiration(string token);
        bool IsTokenExpired(string token);
        Task<AccessToken> RefreshTokenAsync(User user);
    }
}