using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Helpers
{
    public static class RatingMapper
    {
        public static RatingOutputDto MapToOutputDto(Rating rating, IRatingRepository repository)
        {
            return new RatingOutputDto
            {
                Id = rating.Id,
                Rating = rating.NumberOfStars,
                UserInfo = repository.GetUserInfoForRating(rating),
                SongInfo = repository.GetSongInfoForRating(rating)
            };
        }
    }
}
