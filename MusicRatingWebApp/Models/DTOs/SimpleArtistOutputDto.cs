using System.ComponentModel.DataAnnotations;

namespace MusicRatingWebApp.Models.DTOs
{
    public class SimpleArtistOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RatingsCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public double AverageRating { get; set; }
    }
}
