using Saturn.CommonLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace Saturn.UsersService.Dto
{
    public class UserUpdateDto
    {
        [Required]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
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
