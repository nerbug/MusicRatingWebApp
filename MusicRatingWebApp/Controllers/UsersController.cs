using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository repository;

        public UsersController(IUserRepository repository)
        {
            this.repository = repository;
        }

        // POST: api/Users
        [HttpPost]
        public IActionResult PostUser(UserLoginDto input)
        {
            if (input.Username == null || input.Password == null)
                return BadRequest();

            if (repository.UserExists(input.Username))
                return BadRequest(new {message = "User already exists with this name!"});

            var salt = PasswordUtils.GenerateSalt();
            var hash = PasswordUtils.HashPassword(input.Password, salt);

            var createdUser = new User
            {
                Username = input.Username,
                PasswordSalt = salt,
                PasswordHash = hash,
                Type = Models.User.UserType.RegularUser
            };

            return repository.PostUser(createdUser, this);
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int id)
        {
            User user = repository.GetUser(id);
            if (user == null)
                return NotFound();

            repository.DeleteUser(user);
            return NoContent();
        }
    }
}
