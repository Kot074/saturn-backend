using System.Text.Json.Serialization;

namespace Saturn.CommonLibrary.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public UserRoles Role { get; set; }
        public string? Avatar { get; set; }

        public UserModel()
        {
            Name = string.Empty;
            Lastname = string.Empty;
            Patronymic = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
        }

        public UserModel(UserModel user)
        {
            Id = user.Id;
            Name = user.Name;
            Lastname = user.Lastname;
            Patronymic = user.Patronymic;
            Email = user.Email;
            Phone = user.Phone;
            Role = user.Role;
            Avatar = user.Avatar;
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRoles
    {
        Guest = 0,
        User = 1,
        Administrator = 2,
    }
}
