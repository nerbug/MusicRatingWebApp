﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly ISongRepository repository;

        public SongsController(ISongRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/songs
        //[HttpGet]
        //public ActionResult<IEnumerable<SimpleSongOutputDto>> GetSongs()
        //{
        //    var songs = repository.GetSongs();
        //    var songDtos = songs.Select(song => SongMapper.MapToSimpleOutputDto(song, repository)).ToList();

        //    return Ok(songDtos);
        //}

        // GET: api/songs?minYear=1985&genre=Metal
        [HttpGet]
        public ActionResult<IEnumerable<SimpleSongOutputDto>> GetSongs(int? minYear, int? maxYear, string title,
            string genre)
        {
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
            if (id != songInputDto.Id)
                return BadRequest();

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