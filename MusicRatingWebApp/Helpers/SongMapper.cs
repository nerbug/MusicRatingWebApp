using System.Collections.Generic;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Helpers
{
    public static class SongMapper
    {

        public static SimpleSongOutputDto MapToSimpleOutputDto(Song song, ISongRepository repository)
        {
            int ratingsCount = repository.GetRatingsCountForSong(song);
            double averageRating = repository.GetAverageRatingForSong(song);
            return new SimpleSongOutputDto
            {
                Id = song.Id,
                Title = song.Title,
                Year = song.Year,
                Genre = song.Genre,
                RatingsCount = ratingsCount,
                AverageRating = averageRating
            };
        }

        public static DetailedSongOutputDto MapToDetailedOutputDto(Song song, ISongRepository repository)
        {
            int ratingsCount = repository.GetRatingsCountForSong(song);
            double averageRating = repository.GetAverageRatingForSong(song);
            ArtistInfoDto artistInfo = repository.GetArtistInfoForSong(song);
            RatingDistributionDto ratingDistribution = repository.GetRatingDistributionForSong(song);
            List<RatingOutputDto> ratingOutputs = repository.GetRatingOutputDtosForSong(song);
            return new DetailedSongOutputDto
            {
                Id = song.Id,
                Title = song.Title,
                Year = song.Year,
                Genre = song.Genre,
                RatingsCount = ratingsCount,
                AverageRating = averageRating,
                RatingDistribution = ratingDistribution,
                ArtistInfo = artistInfo,
                Ratings = ratingOutputs
            };
        }
    }
}
