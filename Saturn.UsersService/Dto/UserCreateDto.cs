using Saturn.CommonLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace Saturn.UsersService.Dto
{
    public class UserCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Lastname { get; set; }
        public string Patronymic { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        public UserRoles Role { get; set; }

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
