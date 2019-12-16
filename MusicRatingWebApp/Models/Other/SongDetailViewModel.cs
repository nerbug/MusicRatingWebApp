using MusicRatingWebApp.Models.DTOs;

namespace MusicRatingWebApp.Models.Other
{
    public class SongDetailViewModel
    {
        public DetailedSongOutputDto Song { get; set; }

        //  0, if user is not logged in or didn't rate the song
        // Otherwise, the current rating of the song by the logged in user in stars
        public int UsersCurrentRating { get; set; }
    }
}
