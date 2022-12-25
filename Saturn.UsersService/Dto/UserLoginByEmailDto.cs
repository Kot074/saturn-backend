using System.ComponentModel.DataAnnotations;

namespace Saturn.UsersService.Dto
{
    public class UserLoginByEmailDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public UserLoginByEmailDto()
        {
            Email = string.Empty;
            Password = string.Empty;
        }
    }
}
