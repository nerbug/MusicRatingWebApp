namespace MusicRatingWebApp.Models.DTOs
{
    public class RatingOutputDto
    {
        public int Id { get; set; }
        public UserInfoDto UserInfo { get; set; }
        public SongInfoDto SongInfo { get; set; }
        public int Rating { get; set; }
    }
}
