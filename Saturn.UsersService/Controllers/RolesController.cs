using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saturn.CommonLibrary.Models;

namespace Saturn.UsersService.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetRoles()
        {
            return Ok(Enum.GetNames(typeof(UserRoles)));
        }
    }
}
