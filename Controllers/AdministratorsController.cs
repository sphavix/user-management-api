using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoleBasedUserManagementApi.Models;
using System.Net.Mail;

namespace RoleBasedUserManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdministratorsController(UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager) : ControllerBase
    {
        [HttpGet("users")]
        public async Task<IActionResult> UserList()
        {
            var users = await _userManager.Users.Select(u =>
                new 
                {
                    u.Id,
                    u.Email,
                    u.PhoneNumber
                }).ToListAsync();

            return Ok(User);
        }

        [HttpPost("users")]
        public async Task<IActionResult> AddUser([FromBody] Register model)
        {
            if (!isEmailValid(model.Email))
            {
                return BadRequest(new { message = "Invalid email address!" });
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                return BadRequest(new { message = "Email address already exist!" });
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };

            var results = await _userManager.CreateAsync(user, model.Password);

            if(results.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    var role = await _roleManager.CreateAsync(new IdentityRole("User"));

                    if (!role.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);

                        return StatusCode(500, new { message = "Unable to create user role!", error = role.Errors });
                    }
                }

                await _userManager.AddToRoleAsync(user, "User");

                return Ok(new { message = "User added successfully!" });
            }

            return BadRequest(results.Errors);
        }

        [HttpDelete("users")]
        public async Task<IActionResult> DeleteUser([FromBody] string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if(user is null)
            {
                return NotFound();
            }

            var results = await _userManager.DeleteAsync(user);

            if(results.Succeeded)
            {
                return Ok(new { message = "User has been deleted!" });
            }

            return BadRequest(results.Errors);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> RolesList()
        {
            var roles = await _roleManager.Roles
                                .Select(r => new
                                {
                                    r.Id,
                                    r.Name
                                }).ToListAsync();

            return Ok(roles);
        }

        [HttpPost("roles")]
        public async Task<IActionResult> AddRole([FromBody] string roleName)
        {
            if(await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest("This role already exist");
            }

            var results = await _roleManager.CreateAsync(new IdentityRole { Name = roleName });

            if(results.Succeeded)
            {
                return Ok(new { message = "Role created!" });
            }

            return BadRequest(results.Errors);
        }

        [HttpDelete("roles")]
        public async Task<IActionResult> DeleteRole([FromBody] string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if(role is null)
            {
                return NotFound();
            }

            if(role.Name == "Admin")
            {
                return BadRequest("Admin role cannot be deleted!");
            }

            var results = await _roleManager.DeleteAsync(role);

            if(results.Succeeded)
            {
                return Ok(new { message = "Role has been deleted!" });
            }

            return BadRequest(results.Errors);
        }

        [HttpPost("change-user-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeRole model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserEmail);

            if(user is null)
            {
                return NotFound($"User with email {model.UserEmail} does not exist");
            }

            if(!await _roleManager.RoleExistsAsync(model.NewRole))
            {
                return BadRequest($"Role {model.NewRole} does not exist");
            }

            var currentRole = await _userManager.GetRolesAsync(user);

            var removeRole = await _userManager.RemoveFromRolesAsync(user, currentRole);

            if(!removeRole.Succeeded)
            {
                return BadRequest("Cannot change role, please try again!");
            }

            var newRole = await _userManager.AddToRoleAsync(user, model.NewRole);

            if(newRole.Succeeded)
            {
                return Ok($"User {model.UserEmail} role has been changed to {model.NewRole}!");
            }

            return BadRequest("Failed to change role, please try again!");
        }

        [HttpPut("update-user-info")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfo model)
        {
            var admin = await _userManager.FindByEmailAsync(model.Email);

            if(admin is null)
            {
                return NotFound();
            }


            admin.Email = model.Email;
            admin.UserName = model.Email;
            admin.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(admin);

            if(result.Succeeded)
            {
                return Ok(new { message = "Admin info had been updated" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user is null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if(result.Succeeded)
            {
                return Ok(new { message = "Password has been updated!" });
            }

            return BadRequest(result.Errors);
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
