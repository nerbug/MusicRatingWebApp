using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly MusicRatingWebAppDbContext context;
        private readonly IConfiguration configuration;

        public UserRepository(MusicRatingWebAppDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }


        public IActionResult PostUser(User user, ControllerBase controller)
        {
            context.Users.Add(user);
            context.SaveChanges();

            return controller.Ok();
        }

        public void DeleteUser(User user)
        {
            context.Users.Remove(user);
            context.SaveChanges();
        }

        public bool UserExists(string username) => context.Users.Any(u => u.Username == username);
        public User GetUser(int id) => context.Users.FirstOrDefault(u => u.Id == id);

        public string CreateJwtToken(UserLoginDto loginInfo)
        {
            if (loginInfo.Username == null || loginInfo.Password == null)
                return null;

            var user = context.Users.SingleOrDefault(u => u.Username == loginInfo.Username);
            if (user == null)
                return null;

            var calculatedPasswordHash = PasswordUtils.HashPassword(loginInfo.Password, user.PasswordSalt);
            if (calculatedPasswordHash != user.PasswordHash)
                return null;

            // User typed in username and password correctly, build and return JWT token
            IEnumerable<Claim> claims = GetClaims(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtToken:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(configuration["JwtToken:ExpiryDuration"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JwtToken:Issuer"],
                audience: configuration["JwtToken:Issuer"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private IEnumerable<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>();

            // For any user, add username, user ID and user role claims
            var usernameClaim = new Claim(ClaimTypes.Name, user.Username);
            var userIdClaim = new Claim("userId", user.Id.ToString());
            var userRoleClaim = new Claim(ClaimTypes.Role, "User");

            claims.Add(usernameClaim);
            claims.Add(userIdClaim);
            claims.Add(userRoleClaim);

            // If the user is an admin, we also need to additionally add an admin role claim
            if (user.Type == User.UserType.Admin)
            {
                var adminRoleClaim = new Claim(ClaimTypes.Role, "Admin");
                claims.Add(adminRoleClaim);
            }

            return claims;
        }
    }
}
