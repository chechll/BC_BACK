using System.Security.Claims;

namespace BC_BACK.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string userId);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
