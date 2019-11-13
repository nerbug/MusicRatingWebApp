using System.Collections.Generic;

namespace MusicRatingWebApp.Models.DTOs
{
    public class DetailedArtistOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RatingsCount { get; set; }
        public double AverageRating { get; set; }
        public List<SimpleSongOutputDto> Songs { get; set; }
    }
}
