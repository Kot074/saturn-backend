using System.Security.Claims;

namespace Saturn.UsersService.Services
{
    public interface IUsersLogicService
    {
        public string GetJwtToken(ClaimsIdentity identity);
        public Task<ClaimsIdentity> GetClaimsIdentityAsync(long userId, string password);
    }
}
