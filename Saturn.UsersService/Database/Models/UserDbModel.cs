using System;
using System.ComponentModel.DataAnnotations;
using Saturn.CommonLibrary.Models;

namespace Saturn.UsersService.Database.Models
{
    public class UserDbModel : UserModel
    {
        [Required]
        public string Key { get; set; }

        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public UserDbModel() : base()
        {
            TimeStamp = Array.Empty<byte>();
            Key = string.Empty;
        }
    }
}
