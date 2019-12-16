using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
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
        [ValidateAntiForgeryToken]
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

        // GET: Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Username,Password")] LoginAccountModel model)
        {
            if (ModelState.IsValid)
            {
                // Convert LoginAccountModel to UserLoginDto
                var loginDto = new UserLoginDto
                {
                    Username = model.Username,
                    Password = model.Password
                };

                // Attempt to create JWT token server-side. If this returns null, then there is no
                // user with the given credentials in the database and return a "username or password
                // is incorrect" message.
                var jwtToken = repository.CreateJwtToken(loginDto);
                if (jwtToken == null)
                {
                    // No such user with these credentials.
                    ModelState.AddModelError("UsernameOrPasswordIncorrect", "Username or password is incorrect!");
                    return View(model);
                }

                // There is a user with these credentials. Add JWT token string to session data and redirect to home.
                HttpContext.Session.SetString("JwtToken", jwtToken);
                return RedirectToAction("Index", "Home");
            }

            // User wrote info into form incorrectly.
            return View(model);
        }

        //GET: Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
