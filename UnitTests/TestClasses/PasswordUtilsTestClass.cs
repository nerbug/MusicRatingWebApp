using System.ComponentModel;
using MusicRatingWebApp.Helpers;
using Xunit;

namespace UnitTests.TestClasses
{
    public class PasswordUtilsTestClass
    {
        [Fact, DisplayName("Two generated salts are not the same")]
        public void TwoGeneratedSaltsAreNotTheSame()
        {
            byte[] firstSalt = PasswordUtils.GenerateSalt();
            byte[] secondSalt = PasswordUtils.GenerateSalt();

            Assert.NotEqual(firstSalt, secondSalt);
        }

        [Fact, DisplayName("Password hashes are not the same if salts are different")]
        public void PasswordHashesAreNotTheSameIfSaltsAreDifferent()
        {
            byte[] firstSalt = PasswordUtils.GenerateSalt();
            byte[] secondSalt = PasswordUtils.GenerateSalt();

            const string password = "123ABC";
            var firstHash = PasswordUtils.HashPassword(password, firstSalt);
            var secondHash = PasswordUtils.HashPassword(password, secondSalt);

            Assert.NotEqual(firstHash, secondHash);
        }

        [Fact, DisplayName("Password hashes are not the same if passwords are different")]
        public void PasswordHashesAreNotTheSameIfPasswordsAreDifferent()
        {
            byte[] salt = PasswordUtils.GenerateSalt();
            const string firstPassword = "123";
            const string secondPassword = "456";

            var firstHash = PasswordUtils.HashPassword(firstPassword, salt);
            var secondHash = PasswordUtils.HashPassword(secondPassword, salt);

            Assert.NotEqual(firstHash, secondHash);
        }

        [Fact, DisplayName("Password hashes are not the same if passwords and salts are different")]
        public void PasswordHashesAreNotTheSameIfPasswordsAndSaltsAreDifferent()
        {
            byte[] firstSalt = PasswordUtils.GenerateSalt();
            byte[] secondSalt = PasswordUtils.GenerateSalt();
            const string firstPassword = "123";
            const string secondPassword = "456";

            var firstHash = PasswordUtils.HashPassword(firstPassword, firstSalt);
            var secondHash = PasswordUtils.HashPassword(secondPassword, secondSalt);

            Assert.NotEqual(firstHash, secondHash);
        }

        [Fact, DisplayName("Password hashes are the same if passwords and salts are the same")]
        public void PasswordHashesAreTheSameIfPasswordsAndSaltsAreTheSame()
        {
            byte[] salt = PasswordUtils.GenerateSalt();
            const string password = "123456789";

            var firstHash = PasswordUtils.HashPassword(password, salt);
            var secondHash = PasswordUtils.HashPassword(password, salt);

            Assert.Equal(firstHash, secondHash);
        }
    }
}
