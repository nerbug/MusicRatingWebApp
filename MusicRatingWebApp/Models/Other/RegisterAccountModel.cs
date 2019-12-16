using System.ComponentModel.DataAnnotations;

namespace MusicRatingWebApp.Models.Other
{
    public class RegisterAccountModel
    {
        [Required, MaxLength(30)]
        [Display(Name = "username")]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "password")]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "confirm password")]
        public string ConfirmPassword { get; set; }
    }
}
