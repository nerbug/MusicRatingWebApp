using Microsoft.AspNetCore.Mvc;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;

namespace MusicRatingWebApp.Repositories.Contracts
{
    public interface IUserRepository
    {
        IActionResult PostUser(User user, ControllerBase controller);
        void DeleteUser(User user);

        bool UserExists(string username);
        User GetUser(int id);

        string CreateJwtToken(UserLoginDto dto);
    }
}
