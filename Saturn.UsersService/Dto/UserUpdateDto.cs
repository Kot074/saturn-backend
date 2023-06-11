using Saturn.CommonLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace Saturn.UsersService.Dto
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Обновляемый пользователь должен иметь идентификатор.")]
        public long Id { get; set; }
        [Required(ErrorMessage = "Обновляемый пользователь должен иметь имя.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Обновляемый пользователь должен иметь фамилию.")]
        public string Lastname { get; set; }
        public string Patronymic { get; set; }
        [Required(ErrorMessage = "Обновляемый пользователь должен иметь адрес электронной почты.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Обновляемый пользователь должен иметь номер телефона.")]
        public string Phone { get; set; }
        public string? Password { get; set; }
        [Required(ErrorMessage = "Обновляемый пользователь должен иметь роль в системе.")]
        public UserRoles Role { get; set; }

        public UserUpdateDto()
        {
            Name = string.Empty;
            Lastname = string.Empty;
            Patronymic = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
        }
    }
}
