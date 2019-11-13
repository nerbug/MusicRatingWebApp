using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicRatingWebApp.Controllers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Implementations;
using Xunit;

namespace UnitTests.TestClasses
{
    public class ArtistsControllerTestClass
    {
        private readonly List<Artist> artists = new List<Artist>()
        {
            new Artist {Id = 1, Name = "FirstArtist"},
            new Artist {Id = 2, Name = "SecondArtist"}
        };

        private readonly ArtistsController controller;

        public ArtistsControllerTestClass()
        {
            controller = SetupController();
        }

        [Fact, DisplayName("Returns 200 OK when getting all artists")]
        public void Returns200OkWhenGettingAllArtists()
        {
            var serverCode = controller.GetArtists();
            Assert.IsType<OkObjectResult>(serverCode.Result);
        }

        [Fact, DisplayName("Returns 200 OK when getting an artist that exists")]
        public void Returns200OkWhenGettingAnArtistThatExists()
        {
            var serverCode = controller.GetArtist(1);
            Assert.IsType<OkObjectResult>(serverCode.Result);
        }

        [Fact, DisplayName("Returns 404 Not Found when trying to get an artist that doesn't exist")]
        public void Returns404NotFoundWhenTryingToGetAnArtistThatDoesNotExist()
        {
            var serverCode = controller.GetArtist(4);
            Assert.IsType<NotFoundResult>(serverCode.Result);
;       }

        [Fact,
         DisplayName(
             "Returns 400 Bad Request when trying to modify an artist with different IDs in request body and query string")]
        public void Returns400BadRequestWithMismatchingIds()
        {
            var serverCode = controller.PutArtist(1, new ArtistInputDto {Id = 2, Name = "Artist"});
            Assert.IsType<BadRequestResult>(serverCode);
        }

        [Fact, DisplayName("Returns 404 Not Found when attempting to put an artist that doesn't exist")]
        public void Returns404NotFoundWhenAttemptingToPutAnArtistThatDoesNotExist()
        {
            var serverCode = controller.PutArtist(5, new ArtistInputDto {Id = 5, Name = "Artist"});
            Assert.IsType<NotFoundResult>(serverCode);
        }

        [Fact, DisplayName("Returns 400 Bad Request when attempting to put an artist with no name")]
        public void Returns400BadRequestWhenAttemptingToPutAnArtistWithNoName()
        {
            var serverCode = controller.PutArtist(1, new ArtistInputDto {Id = 1});
            Assert.IsType<BadRequestResult>(serverCode);
        }

        [Fact, DisplayName("Returns 404 Not Found when trying to modify an artist that doesn't exist")]
        public void Returns404NotFoundWhenTryingToModifyAnArtistThatDoesNotExist()
        {
            var serverCode = controller.PutArtist(3, new ArtistInputDto {Id = 3, Name = "Artist"});
            Assert.IsType<NotFoundResult>(serverCode);
        }

        [Fact, DisplayName("Returns 204 No Content when an artist has been successfully modified")]
        public void Returns204NoContentWhenAnArtistHasBeenSuccessfullyModified()
        {
            var serverCode = controller.PutArtist(2, new ArtistInputDto {Id = 2, Name = "Artist"});
            Assert.IsType<NoContentResult>(serverCode);
        }

        [Fact, DisplayName("Returns 201 Created when an artist has been created")]
        public void Returns201CreatedWhenAnArtistHasBeenCreated()
        {
            var serverCode = controller.PostArtist(new ArtistInputDto {Name = "SomeArtist"});
            Assert.IsType<CreatedAtActionResult>(serverCode.Result);
        }

        [Fact, DisplayName("Returns 400 Bad Request when trying to create an artist with a malformed request body")]
        public void Returns400BadRequestWhenTryingToCreateAnArtistWithMalformedRequestBody()
        {
            var serverCode = controller.PostArtist(new ArtistInputDto());
            Assert.IsType<BadRequestResult>(serverCode.Result);
        }

        [Fact, DisplayName("Returns 204 No Content when an artist has been successfully deleted")]
        public void Returns204NoContentWhenAnArtistHasBeenSuccessfullyDeleted()
        {
            var serverCode = controller.DeleteArtist(1);
            Assert.IsType<NoContentResult>(serverCode);
        }

        [Fact, DisplayName("Returns 404 Not Found when trying to delete an artist that doesn't exist")]
        public void Returns404NotFoundWhenTryingToDeleteAnArtistThatDoesNotExist()
        {
            var serverCode = controller.DeleteArtist(10);
            Assert.IsType<NotFoundResult>(serverCode);
        }

        private ArtistsController SetupController()
        {
            var options = new DbContextOptionsBuilder<MusicRatingWebAppDbContext>()
                .UseInMemoryDatabase("ArtistControllerTest").Options;

            var context = new MusicRatingWebAppDbContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Artists.AddRange(artists);
            context.SaveChanges();

            var repository = new ArtistRepository(context);
            var controllerObj = new ArtistsController(repository);

            return controllerObj;
        }
    }
}
