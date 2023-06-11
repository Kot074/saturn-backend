using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Saturn.UsersService.Repositories;
using Saturn.CommonLibrary.Models;
using Saturn.UsersService.Database.Models;
using Saturn.UsersService.Dto;
using Microsoft.AspNetCore.Authorization;
using Saturn.UsersService.Services;

namespace Saturn.UsersService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUsersHelpersService _usersHelpersService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUsersRepository usersRepository, IUsersHelpersService usersHelpersService, ILogger<UsersController> logger)
        {
            _usersRepository = usersRepository;
            _usersHelpersService = usersHelpersService;
            _logger = logger;
        }

        [HttpGet("get_roles")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetRoles()
        {
            return Ok(Enum.GetNames(typeof(UserRoles)));
        }

        [HttpPost("create_user")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] UserCreateDto user)
        {
            var userDb = new UserDbModel
            {
                Name = user.Name,
                Lastname = user.Lastname,
                Patronymic = user.Patronymic,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Key = _usersHelpersService.EncodingString(user.Password)
            };
            try
            {
                await _usersRepository.Create(userDb);

                _logger.LogInformation($"Пользователь {user.Lastname} {user.Name} {user.Patronymic} успешно зарегистрирован.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При регистрации пользователя {user.Lastname} {user.Name} {user.Patronymic} произошла ошибка.");
                return BadRequest($"При регистрации пользователя {user.Lastname} {user.Name} {user.Patronymic} произошла ошибка.");
            }
        }

        [HttpGet("get_users")]
        [Authorize(Roles = "Administrator, User")]
        [ProducesResponseType(typeof(IEnumerable<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _usersRepository.ReadAll();

                _logger.LogInformation($"Список пользователей успешно получен.");

                return Ok(users.Select(user => new UserModel(user)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При попытке получения списка пользователей произошла ошибка.");
                return BadRequest($"При попытке получения списка пользователей произошла ошибка.");
            }
        }

        [HttpGet("get_user")]
        [Authorize(Roles = "Administrator, User")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([Required][FromQuery] long id)
        {
            {
                try
                {
                    var user = await _usersRepository.Read(id);

                    _logger.LogInformation($"Пользователь успешно получен.");

                    return Ok(new UserModel(user));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"При попытке получения пользователя произошла ошибка.");
                    return BadRequest($"При попытке получения пользователя произошла ошибка.");
                }
            }
        }

        [HttpPut("update_user")]
        [Authorize(Roles = "Administrator, User")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto user)
        {
            try
            {
                var userDb = await _usersRepository.Read(user.Id);
                userDb.Name = user.Name;
                userDb.Lastname = user.Lastname;
                userDb.Patronymic = user.Patronymic;
                userDb.Email = user.Email;
                userDb.Phone = user.Phone;

                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    userDb.Key = _usersHelpersService.EncodingString(user.Password);
                }

                var currentUserId = _usersHelpersService.GetCurrentUserId(Request);
                if (currentUserId == user.Id && userDb.Role != user.Role)
                {
                    return BadRequest("Невозможно изменить роль самому себе.");
                }
                userDb.Role = user.Role;

                await _usersRepository.Update(userDb);

                _logger.LogInformation($"Данные пользователя {user.Email} были успешно обновлены.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При попытке обновления данных пользователя произошла ошибка.");
                return BadRequest("Возникла ошибка при изменении данных пользователя!");
            }
        }

        [HttpDelete("delete_user")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromQuery][Required] long id)
        {
            try
            {
                var currentUserId = _usersHelpersService.GetCurrentUserId(Request);
                if (currentUserId == id)
                {
                    return BadRequest("Невозможно удалить свою учетную запись.");
                }

                await _usersRepository.Delete(id);

                _logger.LogInformation($"Данные пользователя (Id = {id}) были успешно удалены.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При попытке удаления данных пользователя произошла ошибка.");
                return BadRequest("При попытке удаления данных пользователя произошла ошибка.");
            }
        }


    }
}
