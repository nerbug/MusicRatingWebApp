using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicRatingWebApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(30)] public string Username { get; set; }
        [Required, MaxLength(50)] public string PasswordHash { get; set; }
        [Required, MaxLength(128)] public byte[] PasswordSalt { get; set; }

        public enum UserType
        {
            Admin,
            RegularUser
        }
        public UserType Type { get; set; }

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
