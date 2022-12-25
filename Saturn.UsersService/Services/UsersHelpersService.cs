using Microsoft.IdentityModel.Tokens;
using Saturn.UsersService.Auth;
using Saturn.UsersService.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Saturn.UsersService.Services
{
    public class UsersHelpersService : IUsersHelpersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersHelpersService(IUsersRepository repository)
        {
            _usersRepository = repository;
        }

        public string GetJwtToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public async Task<ClaimsIdentity> GetClaimsIdentityAsync(long userId, string password)
        {
            var userDb = await _usersRepository.Read(userId);
            if (userDb == null)
            {
                throw new InvalidOperationException("Пользователь с таким id не найден.");
            }

            var bytePassword = Encoding.UTF8.GetBytes(password);
            var sha256Key = SHA256.HashData(bytePassword);
            var keyString = Encoding.UTF8.GetString(sha256Key);

            var shortName = string.IsNullOrWhiteSpace(userDb.Patronymic)
                ? $"{userDb.Lastname} {userDb.Name[0]}."
                : $"{userDb.Lastname} {userDb.Name[0]}. {userDb.Patronymic[0]}.";

            if (string.Equals(userDb.Key, keyString))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userDb.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, userDb.Role.ToString()),
                    new Claim("id", userDb.Id.ToString()),
                    new Claim("shortName", shortName),
                    new Claim("email", userDb.Email),
                    new Claim("phone", userDb.Phone)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }
            return null;
        }
        public async Task<ClaimsIdentity> GetClaimsIdentityAsync(string userEmail, string password)
        {
            var userDb = (await _usersRepository.ReadAll()).SingleOrDefault(_ => _.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
            if (userDb == null)
            {
                throw new InvalidOperationException("Пользователь с таким email не зарегистрирован.");
            }

            var bytePassword = Encoding.UTF8.GetBytes(password);
            var sha256Key = SHA256.HashData(bytePassword);
            var keyString = Encoding.UTF8.GetString(sha256Key);

            var shortName = string.IsNullOrWhiteSpace(userDb.Patronymic)
                ? $"{userDb.Lastname} {userDb.Name[0]}."
                : $"{userDb.Lastname} {userDb.Name[0]}. {userDb.Patronymic[0]}.";

            if (string.Equals(userDb.Key, keyString))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userDb.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, userDb.Role.ToString()),
                    new Claim("id", userDb.Id.ToString()),
                    new Claim("shortName", shortName),
                    new Claim("email", userDb.Email),
                    new Claim("phone", userDb.Phone)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }
            return null;
        }
    }
}
