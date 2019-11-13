
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MusicRatingWebApp.Controllers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;

namespace MusicRatingWebApp.Repositories.Contracts
{
    public interface IArtistRepository
    {
        IEnumerable<Artist> GetArtists();
        Artist GetArtist(int id);
        IActionResult PutArtist(int id, Artist artist, ControllerBase controller);

        ActionResult<DetailedArtistOutputDto> PostArtist(string action, Artist artist, ArtistsController controller);

        void DeleteArtist(Artist artist);
        int GetRatingsCountForArtist(Artist artist);
        double GetAverageRatingForArtist(Artist artist);
        List<SimpleSongOutputDto> GetSongDtosForArtist(Artist artist);
    }
}
