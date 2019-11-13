using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicRatingWebApp.Models
{
    public class Artist
    {
        public int Id { get; set; }
        [Required, MaxLength(30)] public string Name { get; set; }
        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
