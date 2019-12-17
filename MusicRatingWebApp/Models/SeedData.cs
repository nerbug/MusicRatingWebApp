using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicRatingWebApp.Helpers;

namespace MusicRatingWebApp.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider provider)
        {
            using (var context = new MusicRatingWebAppDbContext(provider.GetRequiredService<DbContextOptions<MusicRatingWebAppDbContext>>()))
            {
                bool hasUsers = context.Users.Any();

                if (hasUsers)
                    return;

                var adminPassword = "admin";
                var adminPasswordSalt = PasswordUtils.GenerateSalt();
                var adminPasswordHash = PasswordUtils.HashPassword(adminPassword, adminPasswordSalt);

                // Add an admin user for testing
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = adminPasswordHash,
                    PasswordSalt = adminPasswordSalt,
                    Type = User.UserType.Admin
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
}
