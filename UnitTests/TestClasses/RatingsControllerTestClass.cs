using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicRatingWebApp.Controllers.API;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Repositories.Implementations;
using Xunit;

namespace UnitTests.TestClasses
{
    public class RatingsControllerTestClass
    {
        private readonly List<Artist> artists = new List<Artist>()
        {
            new Artist {Id = 1, Name = "FirstArtist"},
            new Artist {Id = 2, Name = "SecondArtist"}
        };

        private readonly List<Song> songs = new List<Song>()
        {
            new Song {ArtistId = 1, Genre = "Metal", Title = "FirstSong", Year = 1986, Id = 1},
            new Song {ArtistId = 1, Genre = "Rock", Title = "SecondSong", Year = 1999, Id = 2}
        };

        private readonly List<User> users;

        private readonly List<Rating> ratings = new List<Rating>()
        {
            new Rating {Id = 1, NumberOfStars = 5, SongId = 1, UserId = 1},
            new Rating {Id = 2, NumberOfStars = 4, SongId = 2, UserId = 2}
        };

        private readonly ApiRatingsController controller;

        public RatingsControllerTestClass()
        {
            users = SetupUsers();
            controller = SetupController();
        }

        private static List<User> SetupUsers()
        {
            const string password = "password";
            var firstSalt = PasswordUtils.GenerateSalt();
            var secondSalt = PasswordUtils.GenerateSalt();
            var firstHash = PasswordUtils.HashPassword(password, firstSalt);
            var secondHash = PasswordUtils.HashPassword(password, secondSalt);
            var users = new List<User>
            {
                new User
                {
                    Username = "FirstUser",
                    PasswordHash = firstHash,
                    PasswordSalt = firstSalt,
                    Type = User.UserType.RegularUser
                },
                new User
                {
                    Username = "SecondUser",
                    PasswordHash = secondHash,
                    PasswordSalt = secondSalt,
                    Type = User.UserType.RegularUser
                }
            };
            return users;
        }

        [Fact, DisplayName("Returns 200 OK when getting all ratings")]
        public void Returns200OkWhenGettingAllRatings()
        {
            var serverCode = controller.GetRatings();
            Assert.IsType<OkObjectResult>(serverCode.Result);
        }

        [Fact, DisplayName("Returns 200 OK when getting a rating that exists")]
        public void Returns200OkWhenGettingARatingThatExists()
        {
            var serverCode = controller.GetRating(2);
            Assert.IsType<OkObjectResult>(serverCode.Result);
        }

        [Fact, DisplayName("Returns 404 Not Found when getting a rating that doesn't exist")]
        public void Returns404NotFoundWhenGettingARatingThatDoesNotExist()
        {
            var serverCode = controller.GetRating(15);
            Assert.IsType<NotFoundResult>(serverCode.Result);
        }

        [Fact, DisplayName("Returns 204 No Content when deleting a rating that exists")]
        public void Returns204NoContentWhenDeletingARatingThatExists()
        {
            var serverCode = controller.DeleteRating(1);
            Assert.IsType<NoContentResult>(serverCode);
        }

        [Fact, DisplayName("Returns 404 Not Found when trying to delete a rating that doesn't exist")]
        public void Returns404NotFoundWhenTryingToDeleteARatingThatDoesNotExist()
        {
            var serverCode = controller.DeleteRating(50);
            Assert.IsType<NotFoundResult>(serverCode);
        }

        private ApiRatingsController SetupController()
        {
            var options = new DbContextOptionsBuilder<MusicRatingWebAppDbContext>()
                .UseInMemoryDatabase("RatingControllerTest").Options;

            var context = new MusicRatingWebAppDbContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Artists.AddRange(artists);
            context.Ratings.AddRange(ratings);
            context.Songs.AddRange(songs);
            context.Users.AddRange(users);
            context.SaveChanges();

            var repository = new RatingRepository(context);
            var controllerObj = new ApiRatingsController(repository);

            return controllerObj;
        }
    }
}
