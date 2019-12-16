using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicRatingWebApp.Models.DTOs
{
    public class DetailedSongOutputDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public int RatingsCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public double AverageRating { get; set; }

        public RatingDistributionDto RatingDistribution { get; set; }
        public ArtistInfoDto ArtistInfo { get; set; }
        public List<RatingOutputDto> Ratings { get; set; }
    }
}
