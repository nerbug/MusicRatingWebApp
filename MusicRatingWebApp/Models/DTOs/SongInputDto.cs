namespace MusicRatingWebApp.Models.DTOs
{
    public class SongInputDto
    {
        public int Id { get; set; }
        public int ArtistId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
    }
}
