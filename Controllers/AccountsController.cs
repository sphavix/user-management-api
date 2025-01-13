using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RoleBasedUserManagementApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;

namespace RoleBasedUserManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountsController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var user = new IdentityUser { UserName = model.UserName };

            var result = await _userManager.CreateAsync(user, model.Password);

            if(result.Succeeded)
            {
                return Ok(new { message = "User registered successfully!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn([FromBody] Login model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if(user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRole = await _userManager.GetRolesAsync(user);

                var userClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                userClaims.AddRange(userRole.Select(role =>
                    new Claim(ClaimTypes.Role, role)));

                var jwtToken = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryMinutes"]!)),
                    claims: userClaims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!)),
                    SecurityAlgorithms.HmacSha256));

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(jwtToken) });
            }

            return Unauthorized();
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] string role)
        {
            if(!await _roleManager.RoleExistsAsync(role))
            {
                var results = await _roleManager.CreateAsync(new IdentityRole(role));
                if(results.Succeeded)
                {
                    return Ok(new { message = "Role Added Successfully" });
                }
                return BadRequest();
            }

            return BadRequest("Role already exist");
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRole model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if(user is null)
            {
                return BadRequest("User does not exist");
            }

            var results = await _userManager.AddToRoleAsync(user, model.RoleName);

            if(results.Succeeded)
            {
                return Ok(new { message = "Role has been assigned to the user" });
            }

            return BadRequest("There was an issue assigning role to the user.");
        }


    }
}
