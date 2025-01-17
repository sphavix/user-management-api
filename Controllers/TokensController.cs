using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoleBasedUserManagementApi.Models;
using RoleBasedUserManagementApi.Persistence;
using RoleBasedUserManagementApi.Services.Abstract;

namespace RoleBasedUserManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public TokensController(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToke(RefreshTokenRequest request)
        {
            if(request is null)
            {
                return BadRequest("Invalid client request");
            }

            string accessToken = request.AccessToken!;
            string refreshToken = request.RefreshToken!;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

            var username = principal.Identity!.Name;

            var user = _context.TokenInfo.SingleOrDefault(x => x.Username == username);

            if(user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.Now)
            {
                return BadRequest("Invalid client request");
            }

            var newAccessToke = _tokenService.GetToken(principal.Claims);
            var newRefreshToken = _tokenService.GetRefreshToken();

            user.RefreshToken = newRefreshToken;
            _context.SaveChanges();

            return Ok(new RefreshTokenRequest()
            {
                AccessToken = newAccessToke.Token,
                RefreshToken = newRefreshToken,
            });
        }

        [HttpPost("revoke"), Authorize]
        public IActionResult RevokeToken()
        {
            try
            {
                var username = User.Identity!.Name;

                var user = _context.TokenInfo.SingleOrDefault(x => x.Username == username);

                if(user is null)
                {
                    return BadRequest();
                }

                user.RefreshToken = null;
                _context.SaveChanges();

                return Ok(true);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
