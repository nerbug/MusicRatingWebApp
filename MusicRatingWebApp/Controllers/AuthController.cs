using Microsoft.AspNetCore.Mvc;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository repository;

        public AuthController(IUserRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDto dto)
        {
            string jwtTokenString = repository.CreateJwtToken(dto);
            if (jwtTokenString == null)
                return Unauthorized(new {message = "Username or password is incorrect!"});

            return Ok(new {Token = jwtTokenString});
        }
    }
}
