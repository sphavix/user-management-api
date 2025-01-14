using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleBasedUserManagementApi.Models;

namespace RoleBasedUserManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class UsersController(UserManager<IdentityUser> _userManager) : ControllerBase
    {
        [HttpGet("user-info")]
        public async Task<IActionResult> GetUserInfo([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user is null)
            {
                return NotFound("User not found");
            }

            return Ok(new { user.Email, user.PhoneNumber });
        }

        [HttpPut("update-user-info")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfo model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user is null)
            {
                return NotFound("User not found");
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if(result.Succeeded)
            {
                return Ok(new { message = "User info has been updated!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.DeleteAsync(user);

            if(result.Succeeded)
            {
                return Ok(new { message = "Account has been deleted" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { message = "Password has been updated!" });
            }

            return BadRequest(result.Errors);
        }
    }
}
