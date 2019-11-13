namespace MusicRatingWebApp.Models.DTOs
{
    public class SimpleArtistOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RatingsCount { get; set; }
        public double AverageRating { get; set; }
    }
}
