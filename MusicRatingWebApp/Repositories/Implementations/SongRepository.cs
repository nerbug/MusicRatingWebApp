using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicRatingWebApp.Controllers;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.DTOs;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Repositories.Implementations
{
    public class SongRepository : ISongRepository
    {
        private readonly MusicRatingWebAppDbContext context;

        public SongRepository(MusicRatingWebAppDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Song> GetSongs() => context.Songs;
        public IEnumerable<Song> GetFilteredSongs(int? minYear, int? maxYear, string title, string genre)
        {
            int min = minYear ?? GetOldestSongsYear();
            int max = maxYear ?? GetNewestSongsYear();

            // Get all songs in year range
            var songsInYearRange = context.Songs.Where(s => s.Year >= min && s.Year <= max);

            // Continue filtering
            var songsWithTitle = title != null
                ? songsInYearRange.AsEnumerable().Where(s => s.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase))
                : songsInYearRange;
            var finalSongsList = genre != null
                ? songsWithTitle.AsEnumerable().Where(s => s.Genre.Contains(genre, StringComparison.CurrentCultureIgnoreCase))
                : songsWithTitle;
            return finalSongsList;
        }

        public Song GetSong(int id) => context.Songs.FirstOrDefault(s => s.Id == id);

        public void DeleteSong(Song song)
        {
            context.Songs.Remove(song);
            context.SaveChanges();
        }

        private bool SongExists(int id) => context.Songs.Any(s => s.Id == id);
        public bool ArtistExists(int artistId) => context.Artists.Any(a => a.Id == artistId);
        public IActionResult PutSong(int id, Song song, ControllerBase controller)
        {
            context.Entry(song).State = EntityState.Modified;
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
                    return controller.NotFound();
                throw;
            }

            return controller.NoContent();
        }

        public ActionResult<DetailedSongOutputDto> PostSong(string action, Song song, SongsController controller)
        {
            context.Songs.Add(song);
            context.SaveChanges();

            return controller.CreatedAtAction(action, new {song.Id}, SongMapper.MapToDetailedOutputDto(song, this));
        }

        public int GetRatingsCountForSong(Song song)
        {
            var ratings = GetRatingsForSong(song.Id);
            return ratings.Count;
        }

        public double GetAverageRatingForSong(Song song)
        {
            var ratings = GetRatingsForSong(song.Id);
            return ratings.Count != 0 ? ratings.Average(r => r.NumberOfStars) : 0.0;
        }

        public RatingDistributionDto GetRatingDistributionForSong(Song song)
        {
            var ratings = GetRatingsForSong(song.Id);
            var ratingDistribution = new List<int> {0, 0, 0, 0, 0};
            foreach (var rating in ratings)
            {
                int numberOfStars = rating.NumberOfStars;
                ratingDistribution[numberOfStars - 1]++;
            }

            return new RatingDistributionDto
            {
                One = ratingDistribution[0],
                Two = ratingDistribution[1],
                Three = ratingDistribution[2],
                Four = ratingDistribution[3],
                Five = ratingDistribution[4]
            };
        }

        public ArtistInfoDto GetArtistInfoForSong(Song song)
        {
            var artist = context.Artists.First(a => a.Id == song.ArtistId);
            return new ArtistInfoDto
            {
                ArtistId = song.ArtistId,
                ArtistName = artist.Name
            };
        }

        public List<RatingOutputDto> GetRatingOutputDtosForSong(Song song)
        {
            var ratings = GetRatingsForSong(song.Id);
            var ratingDtos = new List<RatingOutputDto>();
            foreach (var rating in ratings)
            {
                User user = context.Users.First(u => u.Id == rating.UserId);
                UserInfoDto userInfo = new UserInfoDto {UserId = user.Id, Username = user.Username};
                SongInfoDto songInfo = new SongInfoDto {SongId = song.Id, SongTitle = song.Title};
                RatingOutputDto ratingOutputDto = new RatingOutputDto
                {
                    Id = rating.Id,
                    Rating = rating.NumberOfStars,
                    SongInfo = songInfo,
                    UserInfo = userInfo
                };
                ratingDtos.Add(ratingOutputDto);
            }

            return ratingDtos.ToList();
        }

        private ICollection<Rating> GetRatingsForSong(int songId)
        {
            var ratings = context.Ratings.Where(r => r.SongId == songId);
            return ratings.ToList();
        }

        private int GetOldestSongsYear()
        {
            var year = context.Songs.Min(s => s.Year);
            return year;
        }

        private int GetNewestSongsYear()
        {
            var year = context.Songs.Max(s => s.Year);
            return year;
        }
    }
}
