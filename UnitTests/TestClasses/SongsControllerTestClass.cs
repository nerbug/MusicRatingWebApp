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
    public class SongsControllerTestClass
    {
        private readonly List<Artist> artists = new List<Artist>()
        {
            new Artist {Id = 1, Name = "FirstArtist"},
            new Artist {Id = 2, Name = "SecondArtist"}
        };

        private readonly List<Song> songs = new List<Song>()
        {
            new Song {Id = 1, ArtistId = 1, Genre = "Pop", Year = 2019, Title = "FirstSong"},
            new Song {Id = 2, ArtistId = 1, Genre = "Classical", Year = 2009, Title = "SecondSong"},
            new Song {Id = 3, ArtistId = 2, Genre = "Pop", Year = 2005, Title = "ThirdSong"}
        };

        private readonly SongsController controller;

        public SongsControllerTestClass()
        {
            controller = SetupController();
        }

        [Fact, DisplayName("Returns 200 OK when getting all songs")]
        public void Returns200OkWhenGettingAllSongs()
        {
            var serverCode = controller.GetSongs(null, null, null, null);
            Assert.IsType<OkObjectResult>(serverCode.Result);
        }

        [Theory, DisplayName("Returns 200 OK when getting a song that exists")]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Returns200OkWhenGettingASongThatExists(int id)
        {
            var serverCode = controller.GetSong(id);
            Assert.IsType<OkObjectResult>(serverCode.Result);
        }

        [Fact, DisplayName("Returns 404 Not Found when trying to get a song that doesn't exist")]
        public void Returns404NotFoundWhenTryingToGetASongThatDoesNotExist()
        {
            var serverCode = controller.GetSong(5);
            Assert.IsType<NotFoundResult>(serverCode.Result);
        }

        [Theory, DisplayName("Returns 204 No Content when a song has been successfully modified")]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Returns204NoContentWhenASongHasBeenSuccessfullyModified(int id)
        {
            var serverCode = controller.PutSong(id,
                new SongInputDto {Id = id, ArtistId = 1, Genre = "SomeGenre", Year = 2000, Title = "SomeTitle"});
            Assert.IsType<NoContentResult>(serverCode);
        }

        [Fact, DisplayName("Returns 404 Not Found when trying to modify a song that doesn't exist")]
        public void Returns404NotFoundWhenTryingToModifyASongThatDoesNotExist()
        {
            var serverCode = controller.PutSong(4,
                new SongInputDto {Id = 4, ArtistId = 1, Genre = "SomeGenre", Year = 2000, Title = "SomeTitle"});
            Assert.IsType<NotFoundResult>(serverCode);
;       }

        [Fact, DisplayName("Returns 400 Bad Request when trying to modify a song and giving an artist ID that doesn't exist")]
        public void Returns400BadRequestWhenGivingANonExistentPutArtistId()
        {
            var serverCode = controller.PutSong(1,
                new SongInputDto {ArtistId = 10, Genre = "SomeGenre", Id = 1, Year = 2000, Title = "SomeTitle"});
            Assert.IsType<BadRequestResult>(serverCode);
        }

        [Fact, DisplayName("Returns 400 Bad Request when trying to modify a song with a malformed request body")]
        public void Returns400BadRequestWhenGivingAMalformedPutRequestBody()
        {
            var serverCode = controller.PutSong(1, new SongInputDto {ArtistId = 1, Id = 1, Genre = "SomeGenre"});
            Assert.IsType<BadRequestResult>(serverCode);
        }

        [Fact, DisplayName("Returns 400 Bad Request when giving mismatched query string and request body IDs")]
        public void Returns400BadRequestWhenGivingMismatchedQueryAndRequestBodyIds()
        {
            var serverCode = controller.PutSong(1,
                new SongInputDto {ArtistId = 1, Genre = "Genre", Id = 2, Year = 2000, Title = "Title"});
            Assert.IsType<BadRequestResult>(serverCode);
        }

        [Fact, DisplayName("Returns 201 Created when a song has been successfully created")]
        public void Returns201CreatedWhenASongHasBeenSuccessfullyCreated()
        {
            var serverCode = controller.PostSong(new SongInputDto
                {ArtistId = 1, Genre = "Genre", Year = 2000, Title = "Title"});
            Assert.IsType<CreatedAtActionResult>(serverCode.Result);
        }

        [Theory, DisplayName("Returns 400 Bad Request when giving a malformed post request body")]
        [InlineData(null, "Genre")]
        [InlineData("Title", null)]
        [InlineData(null, null)]
        public void Returns400BadRequestWhenGivingAMalformedPostRequestBody(string title, string genre)
        {
            var input = new SongInputDto {ArtistId = 1, Genre = genre, Year = 2000, Title = title};
            var serverCode = controller.PostSong(input);
            Assert.IsType<BadRequestResult>(serverCode.Result);
        }

        [Fact, DisplayName("Returns 400 Bad Request when giving an artist ID that doesn't exist during POST")]
        public void Returns400BadRequestWhenGivingAnArtistIdThatDoesNotExist()
        {
            var serverCode = controller.PostSong(new SongInputDto
                {ArtistId = 4, Genre = "Genre", Year = 2000, Title = "Title"});
            Assert.IsType<BadRequestResult>(serverCode.Result);
        }

        [Theory, DisplayName("Returns 204 No Content when a song has been successfully deleted")]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Returns204NoContentWhenASongHasBeenSuccessfullyDeleted(int id)
        {
            var serverCode = controller.DeleteSong(id);
            Assert.IsType<NoContentResult>(serverCode);
        }

        [Theory, DisplayName("Returns 404 Not Found when trying to delete a song that doesn't exist")]
        [InlineData(4)]
        [InlineData(5)]
        public void Returns404NotFoundWhenTryingToDeleteASongThatDoesNotExist(int id)
        {
            var serverCode = controller.DeleteSong(id);
            Assert.IsType<NotFoundResult>(serverCode);
        }

        private SongsController SetupController()
        {
            var options = new DbContextOptionsBuilder<MusicRatingWebAppDbContext>()
                .UseInMemoryDatabase("SongControllerTest").Options;

            var context = new MusicRatingWebAppDbContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Artists.AddRange(artists);
            context.Songs.AddRange(songs);
            context.SaveChanges();

            var repository = new SongRepository(context);
            var controllerObj = new SongsController(repository);
            return controllerObj;
        }
    }
}
