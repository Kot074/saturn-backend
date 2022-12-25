using System.ComponentModel.DataAnnotations;

namespace Saturn.UsersService.Dto
{
    public class UserLoginByIdDto
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string Password { get; set; }

        public UserLoginByIdDto()
        {
            Password = string.Empty;
        }
    }
}
