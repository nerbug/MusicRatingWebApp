using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRatingWebApp.Models
{
    public class Song
    {
        [Required, ForeignKey("ArtistId")] public Artist Artist { get; set; }
        public int ArtistId { get; set; }

        public int Id { get; set; }
        [Required, MaxLength(50)] public string Title { get; set; }
        [Required] public int Year { get; set; }
        [Required, MaxLength(30)] public string Genre { get; set; }

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
