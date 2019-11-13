using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Helpers
{
    public static class ArtistMapper
    {
        public static SimpleArtistOutputDto MapToSimpleOutputDto(Artist artist, IArtistRepository repository)
        {
            return new SimpleArtistOutputDto
            {
                Id = artist.Id,
                Name = artist.Name,
                RatingsCount = repository.GetRatingsCountForArtist(artist),
                AverageRating = repository.GetAverageRatingForArtist(artist)
            };
        }

        public static DetailedArtistOutputDto MapToDetailedOutputDto(Artist artist, IArtistRepository repository)
        {
            return new DetailedArtistOutputDto
            {
                Id = artist.Id,
                Name = artist.Name,
                RatingsCount = repository.GetRatingsCountForArtist(artist),
                AverageRating = repository.GetAverageRatingForArtist(artist),
                Songs = repository.GetSongDtosForArtist(artist)
            };
        }
    }
}
