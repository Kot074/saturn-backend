using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Saturn.UsersService.Auth
{
    public class AuthOptions
    {
        public const string ISSUER = "Saturn App";
        public const string AUDIENCE = "UsersService";
        const string Key = "mysupersecret_secretkey!123";
        public const int LIFETIME = 1;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}
