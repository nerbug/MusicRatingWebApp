using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicRatingWebApp.Models.DTOs
{
    public class DetailedArtistOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RatingsCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public double AverageRating { get; set; }

        public List<SimpleSongOutputDto> Songs { get; set; }
    }
}
