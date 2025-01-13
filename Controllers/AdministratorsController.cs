using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoleBasedUserManagementApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorsController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("This is the administrators area.");
        }
    }
}
