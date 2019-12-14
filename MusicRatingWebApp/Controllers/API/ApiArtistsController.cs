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
    [Route("api/artists")]
    [ApiController]
    public class ApiArtistsController : ControllerBase
    {
        private readonly IArtistRepository repository;

        public ApiArtistsController(IArtistRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/artists
        [HttpGet]
        public ActionResult<IEnumerable<SimpleArtistOutputDto>> GetArtists()
        {
            // Get artists from database and map to DTOs
            var artists = repository.GetArtists();
            var artistDtos = artists.Select(artist => ArtistMapper.MapToSimpleOutputDto(artist, repository)).ToList();

            return Ok(artistDtos);
        }

        // GET: api/artists/{id}
        [HttpGet("{id}")]
        public ActionResult<DetailedArtistOutputDto> GetArtist(int id)
        {
            var artist = repository.GetArtist(id);
            // If artist with specified ID doesn't exist in the database, return a 404 Not Found error.
            if (artist == null)
                return NotFound();

            return Ok(ArtistMapper.MapToDetailedOutputDto(artist, repository));
        }

        // PUT: api/artists/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult PutArtist(int id, ArtistInputDto artistInputDto)
        {
            // If IDs in query string and request body don't match, return 400 Bad Request.
            if (id != artistInputDto.Id)
                return BadRequest();

            var artistEntry = repository.GetArtist(id);
            // Check if artist with specified ID was found in database
            if (artistEntry == null)
                return NotFound();
            
            // Check if the user gave a new name for the artist they want to modify
            if (artistInputDto.Name == null)
                return BadRequest();

            artistEntry.Name = artistInputDto.Name;
            return repository.PutArtist(id, artistEntry, this);
        }

        // POST: api/artists
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<DetailedArtistOutputDto> PostArtist(ArtistInputDto artistInputDto)
        {
            // Check if user gave a name first
            if (artistInputDto.Name == null)
                return BadRequest();

            var artist = new Artist {Name = artistInputDto.Name};
            return repository.PostArtist("GetArtist", artist, this);
        }

        // DELETE: api/artists/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteArtist(int id)
        {
            var artist = repository.GetArtist(id);
            // Check if artist exists
            if (artist == null)
                return NotFound();

            repository.DeleteArtist(artist);
            return NoContent();
        }

    }
}