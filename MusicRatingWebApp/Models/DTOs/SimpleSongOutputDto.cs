namespace MusicRatingWebApp.Models.DTOs
{
    public class SimpleSongOutputDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public int RatingsCount { get; set; }
        public double AverageRating { get; set; }
    }
}
