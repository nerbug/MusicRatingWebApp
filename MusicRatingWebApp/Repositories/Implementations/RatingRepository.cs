using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicRatingWebApp.Controllers.API;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Repositories.Implementations
{
    public class RatingRepository : IRatingRepository
    {
        private readonly MusicRatingWebAppDbContext context;

        public RatingRepository(MusicRatingWebAppDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Rating> GetRatings() => context.Ratings;

        public Rating GetRating(int id) => context.Ratings.FirstOrDefault(r => r.Id == id);

        private bool RatingExists(int id) => context.Ratings.Any(r => r.Id == id);

        public bool UserAndSongExists(int userId, int songId)
        {
            bool userExists = context.Users.Any(u => u.Id == userId);
            bool songExists = context.Songs.Any(s => s.Id == songId);
            return userExists && songExists;
        }

        public IActionResult PutRating(int id, Rating rating, ControllerBase controller)
        {
            context.Entry(rating).State = EntityState.Modified;
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(id))
                    return controller.NotFound();
                throw;
            }

            return controller.NoContent();
        }

        public ActionResult<RatingOutputDto> PostRating(string action, Rating rating, ApiRatingsController controller)
        {
            context.Ratings.Add(rating);
            context.SaveChanges();

            return controller.CreatedAtAction(action, new {rating.Id}, RatingMapper.MapToOutputDto(rating, this));
        }

        public void DeleteRating(Rating rating)
        {
            context.Ratings.Remove(rating);
            context.SaveChanges();
        }

        public UserInfoDto GetUserInfoForRating(Rating rating)
        {
            var user = context.Users.First(u => u.Id == rating.UserId);
            int userId = user.Id;
            string username = user.Username;
            return new UserInfoDto
            {
                UserId = userId,
                Username = username
            };
        }

        public SongInfoDto GetSongInfoForRating(Rating rating)
        {
            var song = context.Songs.First(s => s.Id == rating.SongId);
            int songId = song.Id;
            string songTitle = song.Title;
            return new SongInfoDto
            {
                SongId = songId,
                SongTitle = songTitle
            };
        }

        public Rating GetRatingWithUserAndSongIds(int userId, int songId) =>
            context.Ratings.FirstOrDefault(r => r.UserId == userId && r.SongId == songId);
    }
}
