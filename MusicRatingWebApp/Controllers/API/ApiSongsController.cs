using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Controllers.API
{
    [Route("api/songs")]
    [ApiController]
    public class ApiSongsController : ControllerBase
    {
        private readonly ISongRepository repository;

        public ApiSongsController(ISongRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/songs?minYear=1985&genre=Metal
        [HttpGet]
        public ActionResult<IEnumerable<SimpleSongOutputDto>> GetSongs(int? minYear, int? maxYear, string title,
            string genre)
        {
            // Retrieve from database and map to output DTOs
            var songs = repository.GetFilteredSongs(minYear, maxYear, title, genre);
            var songDtos = songs.Select(song => SongMapper.MapToSimpleOutputDto(song, repository));

            return Ok(songDtos);
        }

        // GET: api/songs/{id}
        [HttpGet("{id}")]
        public ActionResult<DetailedSongOutputDto> GetSong(int id)
        {
            var song = repository.GetSong(id);
            if (song == null)
                return NotFound();

            return Ok(SongMapper.MapToDetailedOutputDto(song, repository));
        }

        // PUT: api/songs/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult PutSong(int id, SongInputDto songInputDto)
        {
            // Check for mismatching IDs
            if (id != songInputDto.Id)
                return BadRequest();

            // Check if a title and genre were provided by the user
            if (songInputDto.Title == null || songInputDto.Genre == null)
                return BadRequest();

            if (repository.ArtistExists(songInputDto.ArtistId))
            {
                var songEntry = repository.GetSong(id);
                if (songEntry == null)
                    return NotFound();

                songEntry.ArtistId = songInputDto.ArtistId;
                songEntry.Title = songInputDto.Title;
                songEntry.Genre = songInputDto.Genre;
                songEntry.Year = songInputDto.Year;
                return repository.PutSong(id, songEntry, this);
            }
            else
            {
                return BadRequest();
            }
        }

        // POST: api/songs
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<DetailedSongOutputDto> PostSong(SongInputDto songInputDto)
        {
            // Check if title and genre were provided
            if (songInputDto.Title == null || songInputDto.Genre == null)
                return BadRequest();

            var artistId = songInputDto.ArtistId;
            if (repository.ArtistExists(artistId))
            {
                var song = new Song
                {
                    ArtistId = artistId,
                    Title = songInputDto.Title,
                    Year = songInputDto.Year,
                    Genre = songInputDto.Genre
                };

                return repository.PostSong("GetSong", song, this);
            }
            else
            {
                return BadRequest();
            }
        }

        // DELETE: api/songs/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteSong(int id)
        {
            var songEntry = repository.GetSong(id);
            if (songEntry == null)
                return NotFound();

            repository.DeleteSong(songEntry);
            return NoContent();
        }
    }
}