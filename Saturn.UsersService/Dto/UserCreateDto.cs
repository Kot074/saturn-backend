using Saturn.CommonLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace Saturn.UsersService.Dto
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Имя обязательно для заполнения.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Фамилия обязательна для заполнения.")]
        public string Lastname { get; set; }
        public string Patronymic { get; set; }
        [Required(ErrorMessage = "Адрес электронной почты обязателен для заполнения.")]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessage = "Пароль обязателен для заполнения.")]
        public string Password { get; set; }
        public UserRoles Role { get; set; }
        public string? Avatar { get; set; }

        public UserCreateDto()
        {
            Name = string.Empty;
            Lastname = string.Empty;
            Patronymic = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Password = string.Empty;
        }
    }
}
