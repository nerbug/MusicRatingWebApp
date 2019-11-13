using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRatingWebApp.Models
{
    public class Rating
    {
        [Required, ForeignKey("UserId")] public User User { get; set; }
        public int UserId { get; set; }
        [Required, ForeignKey("SongId")] public Song Song { get; set; }
        public int SongId { get; set; }

        public int Id { get; set; }
        public int NumberOfStars { get; set; }
    }
}
