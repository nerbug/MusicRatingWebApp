using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MusicRatingWebApp.Helpers
{
    public static class PasswordUtils
    {
        public static byte[] GenerateSalt()
        {
            var salt = new byte[128];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        public static string HashPassword(string password, byte[] salt)
        {
            string result = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 20000,
                numBytesRequested: 32
            ));
            return result;
        }
    }
}
