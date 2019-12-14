using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MusicRatingWebApp.Controllers.API;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;

namespace MusicRatingWebApp.Repositories.Contracts
{
    public interface IRatingRepository
    {
        IEnumerable<Rating> GetRatings();
        Rating GetRating(int id);
        bool UserAndSongExists(int userId, int songId);
        IActionResult PutRating(int id, Rating rating, ControllerBase controller);
        ActionResult<RatingOutputDto> PostRating(string action, Rating rating, ApiRatingsController controller);
        void DeleteRating(Rating rating);
        UserInfoDto GetUserInfoForRating(Rating rating);
        SongInfoDto GetSongInfoForRating(Rating rating);
    }
}
