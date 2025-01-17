using RoleBasedUserManagementApi.Models;
using System.Security.Claims;

namespace RoleBasedUserManagementApi.Services.Abstract
{
    public interface ITokenService
    {
        TokenResponse GetToken(IEnumerable<Claim> claims);

        string GetRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
