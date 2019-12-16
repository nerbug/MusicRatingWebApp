﻿using MusicRatingWebApp.Models.DTOs;

namespace MusicRatingWebApp.Models
{
    public class SongDetailViewModel
    {
        public DetailedSongOutputDto Song { get; set; }
        public int UsersCurrentRating { get; set; }
    }
}
