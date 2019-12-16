using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.Other;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository repository;
        private readonly MusicRatingWebAppDbContext context;

        public AuthController(IUserRepository repository, MusicRatingWebAppDbContext context)
        {
            this.repository = repository;
            this.context = context;
        }

        // GET: Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register([Bind("Username,Password,ConfirmPassword")]
            RegisterAccountModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user already exists in database
                bool userAlreadyExists = context.Users.FirstOrDefault(u => u.Username == model.Username) != null;
                if (userAlreadyExists)
                {
                    ModelState.AddModelError("UserAlreadyExists", "User with this username already exists!");
                    return View(model);
                }

                var passwordSalt = PasswordUtils.GenerateSalt();
                var passwordHash = PasswordUtils.HashPassword(model.Password, passwordSalt);

                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Type = Models.User.UserType.RegularUser
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
