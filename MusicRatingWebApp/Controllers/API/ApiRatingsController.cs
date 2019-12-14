using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Controllers.API
{
    [Route("api/ratings")]
    [ApiController]
    public class ApiRatingsController : ControllerBase
    {
        private readonly IRatingRepository repository;

        public ApiRatingsController(IRatingRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/ratings
        [HttpGet]
        public ActionResult<IEnumerable<RatingOutputDto>> GetRatings()
        {
            // Get ratings and map to DTOs
            var ratings = repository.GetRatings();
            var ratingDtos = ratings.Select(r => RatingMapper.MapToOutputDto(r, repository)).ToList();

            return Ok(ratingDtos);
        }

        // GET: api/ratings/{id}
        [HttpGet("{id}")]
        public ActionResult<RatingOutputDto> GetRating(int id)
        {
            var rating = repository.GetRating(id);
            // Check if rating exists
            if (rating == null)
                return NotFound();

            return Ok(RatingMapper.MapToOutputDto(rating, repository));
        }

        // PUT: api/ratings/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult PutRating(int id, RatingInputDto ratingInputDto)
        {
            // Check for mismatching IDs in query string and request body
            if (id != ratingInputDto.Id)
                return BadRequest(new {message = "IDs in query string and request body don't match!"});

            if (User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userIdString = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
                var userId = int.Parse(userIdString);
                var roles = claimsIdentity.FindAll(ClaimTypes.Role).Select(r => r.Value);
                if (ratingInputDto.Id != userId && !roles.Contains("Admin"))
                    // User is not admin and tried to modify a rating that isn't "owned" by the user
                {
                    const string errorMessage = "Tried to modify a rating the user doesn't own as a non-admin user!";
                    return BadRequest(new {message = errorMessage});
                }
            }

            // Check if given user and song exist
            int givenUserId = ratingInputDto.UserId;
            int givenSongId = ratingInputDto.SongId;
            if (repository.UserAndSongExists(givenUserId, givenSongId))
            {
                var ratingEntry = repository.GetRating(id);
                // Check if rating exists
                if (ratingEntry == null)
                    return NotFound();

                // If the ID of the song is being changed, that's a bad request too.
                if (ratingEntry.SongId != givenSongId)
                    return BadRequest(new {message = "Cannot change song ID of rating after the rating had already been created!"});

                ratingEntry.UserId = ratingInputDto.UserId;
                ratingEntry.SongId = ratingInputDto.SongId;
                ratingEntry.NumberOfStars = ratingInputDto.Rating;
                return repository.PutRating(id, ratingEntry, this);
            }
            else
            {
                return BadRequest();
            }
        }

        // POST: api/ratings
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public ActionResult<RatingOutputDto> PostRating(RatingInputDto ratingInputDto)
        {
            var userId = ratingInputDto.UserId;
            var songId = ratingInputDto.SongId;

            if (User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userIdString = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
                var userIdFromJwt = int.Parse(userIdString);
                if (userId != userIdFromJwt)
                    return BadRequest(new {message = "Tried to create rating for different user!"});
            }

            if (repository.UserAndSongExists(userId, songId))
            {
                var rating = new Rating
                {
                    UserId = userId,
                    SongId = songId,
                    NumberOfStars = ratingInputDto.Rating
                };

                return repository.PostRating("GetRating", rating, this);
            }
            else
            {
                return BadRequest();
            }
        }

        // DELETE: api/ratings/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteRating(int id)
        {
            var ratingEntry = repository.GetRating(id);
            if (ratingEntry == null)
                return NotFound();

            repository.DeleteRating(ratingEntry);
            return NoContent();
        }
    }
}