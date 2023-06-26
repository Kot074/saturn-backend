namespace Saturn.UsersService.Dto
{
    public class UserLoginResponseDto
    {
        public long Id { get; set; }
        public string User { get; set; }
        public string ShortName { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
    }
}
