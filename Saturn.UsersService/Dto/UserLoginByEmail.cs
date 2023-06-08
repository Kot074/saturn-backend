using System.ComponentModel.DataAnnotations;

namespace Saturn.UsersService.Dto
{
    public class UserLoginByEmail
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public UserLoginByEmail()
        {
            Email = string.Empty;
            Password = string.Empty;
        }
    }
}
