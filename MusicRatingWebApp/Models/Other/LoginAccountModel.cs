using System.ComponentModel.DataAnnotations;

namespace MusicRatingWebApp.Models.Other
{
    public class LoginAccountModel
    {
        [Required, MaxLength(30)]
        [Display(Name = "username")]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "password")]
        public string Password { get; set; }
    }
}
