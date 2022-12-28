using System.Security.Claims;

namespace Saturn.UsersService.Services
{
    public interface IUsersHelpersService
    {
        public string GetJwtToken(ClaimsIdentity identity);
        public Task<ClaimsIdentity?> GetClaimsIdentityAsync(long userId, string password);
        public Task<ClaimsIdentity?> GetClaimsIdentityAsync(string userEmail, string password);
        public string EncodingString(string str);
    }
}
