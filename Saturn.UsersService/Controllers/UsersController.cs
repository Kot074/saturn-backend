using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Saturn.UsersService.Repositories;
using Saturn.CommonLibrary.Models;
using Saturn.UsersService.Database.Models;
using Saturn.UsersService.Dto;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Saturn.UsersService.Services;

namespace Saturn.UsersService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUsersLogicService _usersLogicService;

        public UsersController(IUsersRepository usersRepository, IUsersLogicService usersLogicService)
        {
            _usersRepository = usersRepository;
            _usersLogicService = usersLogicService;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] UserCreateDto user)
        {
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("Возникла ошибка при сохраненнии пользователя!");
            }

            var bytePassword = Encoding.UTF8.GetBytes(user.Password);
            var sha256Key = SHA256.HashData(bytePassword);
            var keyString = Encoding.UTF8.GetString(sha256Key);

            var userDb = new UserDbModel
            {
                Name = user.Name,
                Lastname = user.Lastname,
                Patronymic = user.Patronymic,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Key = keyString
            };
            try
            {
                await _usersRepository.Create(userDb);

                return NoContent();
            }
            catch
            {
                return BadRequest("Возникла ошибка при сохраненнии пользователя!");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        [Route("Get")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromQuery] long id)
        {
            try
            {
                var user = new UserModel(await _usersRepository.Read(id));

                return Ok(user);
            }
            catch (InvalidOperationException)
            {
                return NotFound($"Пользователь с Id = {id} не найден.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        [Route("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _usersRepository.ReadAll();

                return Ok(users.Select(user => new UserModel(user)));
            }
            catch
            {
                return BadRequest($"При запросе пользователей произошла ошибка");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto user)
        {
            try
            {
                var userDb = await _usersRepository.Read(user.Id);
                userDb.Name = string.IsNullOrWhiteSpace(user.Name) ? userDb.Name : user.Name;
                userDb.Lastname = string.IsNullOrWhiteSpace(user.Lastname) ? userDb.Lastname : user.Lastname;
                userDb.Patronymic = string.IsNullOrWhiteSpace(user.Patronymic) ? userDb.Name : user.Patronymic;
                userDb.Email = string.IsNullOrWhiteSpace(user.Email) ? userDb.Email : user.Email;
                userDb.Phone = string.IsNullOrWhiteSpace(user.Phone) ? userDb.Phone : user.Phone;

                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    var bytePassword = Encoding.UTF8.GetBytes(user.Password);
                    var sha256Key = SHA256.HashData(bytePassword);
                    var keyString = Encoding.UTF8.GetString(sha256Key);

                    userDb.Key = keyString;
                }

                userDb.Role = user.Role;

                await _usersRepository.Update(userDb);

                return NoContent();
            }
            catch
            {
                return BadRequest("Возникла ошибка при изменении данных пользователя!");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromBody][Required] long id)
        {
            try
            {
                await _usersRepository.Delete(id);

                return NoContent();
            }
            catch
            {
                return BadRequest("Возникла ошибка при изменении данных пользователя!");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("LoginById")]
        [ProducesResponseType(typeof(UserLoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserLoginByIdDto loginData)
        {
            try
            {
                var identity = await _usersLogicService.GetClaimsIdentityAsync(loginData.Id, loginData.Password);

                if (identity is null)
                {
                    return Unauthorized("Неверный Id пользователя и/или пароль.");
                }

                var response = new UserLoginResponseDto
                {
                    Id = loginData.Id,
                    User = identity.Name ?? "",
                    Token = _usersLogicService.GetJwtToken(identity)
                };

                return Ok(response);
            }
            catch
            {
                return Unauthorized("Неверный Id пользователя и/или пароль.");
            }
        }
    }
}
