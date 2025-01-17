using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RoleBasedUserManagementApi.Models;
using RoleBasedUserManagementApi.Persistence;
using RoleBasedUserManagementApi.Services.Abstract;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RoleBasedUserManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AccountsController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, 
            IConfiguration configuration, ITokenService tokenService, ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));

        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if(!isEmailValid(model.Email))
            {
                return BadRequest(new { message = "Invalid email address!" });
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if(existingUser != null)
            {
                return BadRequest(new { message = "Email address already exist!" });
            }


            var user = new IdentityUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };

            var result = await _userManager.CreateAsync(user, model.Password);

            // Assign user to role upon registration.
            if(result.Succeeded)
            {
                if(!await _roleManager.RoleExistsAsync("User"))
                {
                    var role = await _roleManager.CreateAsync(new IdentityRole("User"));

                    if(!role.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);

                        return StatusCode(500, new { message = "Unable to create user role!", error = role.Errors });
                    }
                }

                await _userManager.AddToRoleAsync(user, "User");

                return Ok(new { message = "User registered successfully!" });
            }

            var errors = result.Errors.Select(err => err.Description);

            return BadRequest(new { message = "We are unable to process registration at this moment, please try again later!", errors });
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn([FromBody] Login model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);

            if(user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRole = await _userManager.GetRolesAsync(user);

                var userClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", user.Id)
                };

                userClaims.AddRange(userRole.Select(role =>
                    new Claim(ClaimTypes.Role, role)));

                var jwtToken = _tokenService.GetToken(userClaims);
                var refreshToken = _tokenService.GetRefreshToken();

                var tokenInfo = _context.TokenInfo.FirstOrDefault(x => x.Username == user.Email);

                if(tokenInfo is null)
                {
                    var info = new TokenInfo
                    {
                        Username = user.Email!,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiry = DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryMinutes"]!))
                    };
                    _context.TokenInfo.Add(info);
                }
                else
                {
                    tokenInfo.RefreshToken = refreshToken;
                    tokenInfo.RefreshTokenExpiry = DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryMinutes"]!));
                }

                try
                {
                    _context.SaveChanges();
                }
                catch(Exception ex)
                {
                    return BadRequest(new { message = "Unable to create a new token!" });
                }

                return Ok(new LoginResponse
                {
                    Name = user.UserName!,
                    Username = user.Email!,
                    Token = jwtToken.Token!,
                    RefreshToken = refreshToken,
                    Expiration = jwtToken.ValidTo,
                    StatusCode = 200,
                    StatusMessage = "Logged in Successfully!"
                });
                //var jwtToken = new JwtSecurityToken(
                //    issuer: _configuration["JwtSettings:Issuer"],
                //    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryMinutes"]!)),
                //    claims: userClaims,
                //    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!)),
                //    SecurityAlgorithms.HmacSha256));

                //return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(jwtToken) });
                //return Ok(jwtToken);
            }

            return Unauthorized();
        }


        private bool isEmailValid(string email)
        {
            try
            {
                var address = new MailAddress(email);

                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
