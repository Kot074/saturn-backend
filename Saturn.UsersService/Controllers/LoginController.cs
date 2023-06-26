using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saturn.UsersService.Dto;
using Saturn.UsersService.Services;

namespace Saturn.UsersService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUsersHelpersService _usersHelpersService;
        private readonly ILogger<UsersController> _logger;

        public LoginController(IUsersHelpersService usersHelpersService, ILogger<UsersController> logger)
        {
            _usersHelpersService = usersHelpersService;
            _logger = logger;
        }

        [HttpPost("by_email")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserLoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserLoginByEmail loginData)
        {
            try
            {
                var identity = await _usersHelpersService.GetClaimsIdentityAsync(loginData.Email, loginData.Password);

                if (identity is null)
                {
                    _logger.LogError($"При попытке авторизации неверно введены email пользователя и/или пароль.", loginData);
                    return Unauthorized("Неверный Email пользователя и/или пароль.");
                }

                var id = identity.Claims.First(_ => _.Type.Equals("id")).Value;
                var shortname = identity.Claims.First(_ => _.Type.Equals("shortName")).Value;
                var response = new UserLoginResponseDto
                {
                    Id = long.Parse(id),
                    ShortName = shortname,
                    Token = _usersHelpersService.GetJwtToken(identity),
                };

                _logger.LogInformation($"Авторизация прошла успешно (получен токен).");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При попытке авторизации по email произошла ошибка.");
                return Unauthorized("При попытке авторизации по email произошла ошибка.");
            }
        }
    }
}
