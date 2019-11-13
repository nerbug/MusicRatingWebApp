namespace MusicRatingWebApp.Models.DTOs
{
    public class RatingInputDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SongId { get; set; }
        public int Rating { get; set; }
    }
}
