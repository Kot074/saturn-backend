using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Saturn.UsersService.Repositories;
using Saturn.CommonLibrary.Models;
using Saturn.UsersService.Database.Models;
using Saturn.UsersService.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("Create")]
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

                _logger.LogInformation($"Найден пользователь {user.Lastname} {user.Name} {user.Patronymic}.");
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, $"При попытке получения пользователя (Id = {id}) произошла ошибка.");
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

                _logger.LogInformation($"Список пользователей успешно получен.");
                return Ok(users.Select(user => new UserModel(user)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При попытке получения списка пользователей произошла ошибка.");
                return BadRequest($"При попытке получения списка пользователей произошла ошибка.");
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
                    userDb.Key = _usersHelpersService.EncodingString(user.Password);
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

                _logger.LogInformation($"Данные пользователя (Id = {id}) были успешно удалены.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При попытке удаления данных пользователя произошла ошибка.");
                return BadRequest("При попытке удаления данных пользователя произошла ошибка.");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("LoginById")]
        [ProducesResponseType(typeof(UserLoginResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] UserLoginByIdDto loginData)
        {
            try
            {
                var identity = await _usersHelpersService.GetClaimsIdentityAsync(loginData.Id, loginData.Password);

                if (identity is null)
                {
                    _logger.LogError($"При попытке авторизации неверно введены id пользователя и/или пароль.", loginData);
                    return Unauthorized("Неверный Id пользователя и/или пароль.");
                }

                var response = new UserLoginResponseDto
                {
                    User = identity.Name ?? "",
                    Token = _usersHelpersService.GetJwtToken(identity)
                };

                _logger.LogInformation($"Авторизация прошла успешно (получен токен).");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При попытке авторизации по id произошла ошибка.");
                return Unauthorized("При попытке авторизации по id произошла ошибка.");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("LoginByEmail")]
        [ProducesResponseType(typeof(UserLoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserLoginByEmailDto loginData)
        {
            try
            {
                var identity = await _usersHelpersService.GetClaimsIdentityAsync(loginData.Email, loginData.Password);

                if (identity is null)
                {
                    _logger.LogError($"При попытке авторизации неверно введены email пользователя и/или пароль.", loginData);
                    return Unauthorized("Неверный Email пользователя и/или пароль.");
                }

                var response = new UserLoginResponseDto
                {
                    User = identity.Name ?? "",
                    Token = _usersHelpersService.GetJwtToken(identity)
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
