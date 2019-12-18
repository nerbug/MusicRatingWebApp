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
    public class ArtistRepository : IArtistRepository
    {
        private readonly MusicRatingWebAppDbContext context;

        public ArtistRepository(MusicRatingWebAppDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Artist> GetArtists() => context.Artists.ToList();
        public Artist GetArtist(int id) => context.Artists.FirstOrDefault(a => a.Id == id);

        public IActionResult PutArtist(int id, Artist artist, ControllerBase controller)
        {
            context.Entry(artist).State = EntityState.Modified;
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistExists(id))
                    return controller.NotFound();
                throw;
            }

            return controller.NoContent();
        }

        private bool ArtistExists(int id) => context.Artists.Any(a => a.Id == id);

        public ActionResult<DetailedArtistOutputDto> PostArtist(string action, Artist artist, ApiArtistsController controller)
        {
            context.Artists.Add(artist);
            context.SaveChanges();

            return controller.CreatedAtAction(action, new {artist.Id}, ArtistMapper.MapToDetailedOutputDto(artist, this));
        }

        public void DeleteArtist(Artist artist)
        {
            context.Artists.Remove(artist);
            context.SaveChanges();
        }

        public int GetRatingsCountForArtist(Artist artist)
        {
            var ratings = GetRatingsForArtist(artist.Id);
            return ratings.Count;
        }

        public double GetAverageRatingForArtist(Artist artist)
        {
            var ratings = GetRatingsForArtist(artist.Id);
            return ratings.Count != 0 ? ratings.Average(r => r.NumberOfStars) : 0.0;
        }

        public List<SimpleSongOutputDto> GetSongDtosForArtist(Artist artist)
        {
            // Get all songs by artist
            var songs = context.Songs.Where(s => s.ArtistId == artist.Id).ToList();
            var songDtos = new List<SimpleSongOutputDto>();
            foreach (var song in songs)
            {
                int ratingsCount = GetRatingsCountForSong(song);
                double averageRating = GetAverageRatingForSong(song);

                var songDto = new SimpleSongOutputDto
                {
                    Id = song.Id,
                    Title = song.Title,
                    Year = song.Year,
                    Genre = song.Genre,
                    RatingsCount = ratingsCount,
                    AverageRating = averageRating
                };
                songDtos.Add(songDto);
            }

            return songDtos;
        }

        private int GetRatingsCountForSong(Song song)
        {
            var ratings = GetRatingsForSong(song.Id);
            return ratings.Count;
        }

        private double GetAverageRatingForSong(Song song)
        {
            var ratings = GetRatingsForSong(song.Id);
            return ratings.Count != 0 ? ratings.Average(r => r.NumberOfStars) : 0.0;
        }

        private ICollection<Rating> GetRatingsForSong(int songId)
        {
            var ratings = context.Ratings.Where(r => r.SongId == songId).ToList();
            return ratings;
        }

        private List<Rating> GetRatingsForArtist(int artistId)
        {
            // Get all songs by artist
            var songs = context.Songs.Where(s => s.ArtistId == artistId).ToList();

            var ratings = new List<Rating>();
            foreach (var song in songs)
            {
                var songRatings = context.Ratings.Where(r => r.SongId == song.Id).ToList();
                ratings.AddRange(songRatings);
            }

            return ratings;
        }
    }
}
