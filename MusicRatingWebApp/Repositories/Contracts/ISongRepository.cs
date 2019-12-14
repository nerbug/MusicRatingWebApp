using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MusicRatingWebApp.Controllers.API;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;

namespace MusicRatingWebApp.Repositories.Contracts
{
    public interface ISongRepository
    {
        IEnumerable<Song> GetSongs();
        IEnumerable<Song> GetFilteredSongs(int? minYear, int? maxYear, string title, string genre);
        Song GetSong(int id);
        void DeleteSong(Song song);

        bool ArtistExists(int artistId);

        IActionResult PutSong(int id, Song song, ControllerBase controller);

        ActionResult<DetailedSongOutputDto> PostSong(string action, Song song, ApiSongsController controller);
        int GetRatingsCountForSong(Song song);
        double GetAverageRatingForSong(Song song);
        RatingDistributionDto GetRatingDistributionForSong(Song song);
        ArtistInfoDto GetArtistInfoForSong(Song song);
        List<RatingOutputDto> GetRatingOutputDtosForSong(Song song);
    }
}
