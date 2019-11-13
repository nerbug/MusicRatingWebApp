using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicRatingWebApp.Helpers;

namespace MusicRatingWebApp.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider provider)
        {
            using (var context = new MusicRatingWebAppDbContext(provider.GetRequiredService<DbContextOptions<MusicRatingWebAppDbContext>>()))
            {
                bool hasArtists = context.Artists.Any();
                bool hasSongs = context.Songs.Any();
                bool hasUsers = context.Users.Any();
                bool hasRatings = context.Ratings.Any();

                if (hasArtists || hasSongs || hasUsers || hasRatings)
                    return;

                var metallica = new Artist {Name = "Metallica"};
                var masterOfPuppets = new Song
                    {Artist = metallica, Title = "Master of Puppets", Genre = "Metal", Year = 1986};
                var blackened = new Song {Artist = metallica, Title = "Blackened", Genre = "Metal", Year = 1988};
                var spitOutTheBone = new Song
                    {Artist = metallica, Title = "Spit Out the Bone", Genre = "Metal", Year = 2016};

                var megadeth = new Artist {Name = "Megadeth"};
                var peaceSells = new Song {Artist = megadeth, Title = "Peace Sells", Genre = "Metal", Year = 1986};
                var holyWars = new Song
                    {Artist = megadeth, Title = "Holy Wars... The Punishment Due", Genre = "Metal", Year = 1990};

                var deadmau5 = new Artist {Name = "deadmau5"};
                var strobe = new Song {Artist = deadmau5, Title = "Strobe", Genre = "Electronic", Year = 2010};

                var taylorSwift = new Artist {Name = "Taylor Swift"};
                var someArtist = new Artist {Name = "Artist1"};
                var someOtherArtist = new Artist {Name = "Artist2"};

                var someSong = new Song
                    {Artist = someArtist, ArtistId = someArtist.Id, Genre = "Funk", Year = 2015, Title = "Some title"};
                var someOtherSong = new Song
                {
                    Artist = someArtist, ArtistId = someArtist.Id, Genre = "Funk", Year = 2014,
                    Title = "Some other song"
                };
                var thirdSong = new Song
                {
                    Artist = someOtherArtist, ArtistId = someOtherArtist.Id, Genre = "Classical", Year = 2010,
                    Title = "Some other title"
                };

                const string firstUsersPassword = "abc123";
                var firstUsersPasswordSalt = PasswordUtils.GenerateSalt();
                var firstUsersPasswordHash = PasswordUtils.HashPassword(firstUsersPassword, firstUsersPasswordSalt);
                var firstUser = new User
                {
                    Username = "regularUser1",
                    PasswordHash = firstUsersPasswordHash,
                    PasswordSalt = firstUsersPasswordSalt,
                    Type = User.UserType.RegularUser
                };

                const string secondUsersPassword = "def456";
                var secondUsersPasswordSalt = PasswordUtils.GenerateSalt();
                var secondUsersPasswordHash = PasswordUtils.HashPassword(secondUsersPassword, secondUsersPasswordSalt);
                var secondUser = new User
                {
                    Username = "regularUser2",
                    PasswordHash = secondUsersPasswordHash,
                    PasswordSalt = secondUsersPasswordSalt,
                    Type = User.UserType.RegularUser
                };

                const string adminPassword = "admin";
                var adminPasswordSalt = PasswordUtils.GenerateSalt();
                var adminPasswordHash = PasswordUtils.HashPassword(adminPassword, adminPasswordSalt);
                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = adminPasswordHash,
                    PasswordSalt = adminPasswordSalt,
                    Type = User.UserType.Admin
                };

                var firstUsersMasterOfPuppetsRating = new Rating {Song = masterOfPuppets, User = firstUser, NumberOfStars = 5};
                var firstUsersBlackenedRating = new Rating {Song = blackened, User = firstUser, NumberOfStars = 5};
                var firstUsersSpitOutTheBoneRating = new Rating {Song = spitOutTheBone, User = firstUser, NumberOfStars = 4};
                var firstUsersStrobeRating = new Rating {Song = strobe, User = firstUser, NumberOfStars = 4};

                var secondUsersMasterOfPuppetsRating = new Rating {Song = masterOfPuppets, User = secondUser, NumberOfStars = 4};
                var secondUsersPeaceSellsRating = new Rating {Song = peaceSells, User = secondUser, NumberOfStars = 4};
                var secondUsersStrobeRating = new Rating {Song = strobe, User = secondUser, NumberOfStars = 3};

                var firstUsersSomeSongRating = new Rating {Song = someSong, User = firstUser, NumberOfStars = 2};
                var firstUsersSomeOtherSongRating = new Rating {Song = someOtherSong, User = firstUser, NumberOfStars = 4};
                var firstUsersThirdSongRating = new Rating {Song = thirdSong, User = firstUser, NumberOfStars = 3};

                var secondUsersSomeSongRating = new Rating {Song = someSong, User = secondUser, NumberOfStars = 4};
                var secondUsersThirdSongRating = new Rating {Song = thirdSong, User = secondUser, NumberOfStars = 3};

                var adminsSomeOtherSongRating = new Rating {Song = someOtherSong, User = admin, NumberOfStars = 1};

                context.Artists.AddRange(metallica, megadeth, deadmau5, taylorSwift, someArtist, someOtherArtist);
                context.Songs.AddRange(masterOfPuppets, blackened, spitOutTheBone, peaceSells, holyWars, strobe,
                    someSong, someOtherSong, thirdSong);
                context.Users.AddRange(firstUser, secondUser, admin);
                context.Ratings.AddRange(firstUsersMasterOfPuppetsRating, firstUsersBlackenedRating,
                    firstUsersSpitOutTheBoneRating, firstUsersStrobeRating, secondUsersMasterOfPuppetsRating,
                    secondUsersPeaceSellsRating, secondUsersStrobeRating, firstUsersSomeSongRating,
                    firstUsersSomeOtherSongRating, firstUsersThirdSongRating, secondUsersSomeSongRating,
                    secondUsersThirdSongRating, adminsSomeOtherSongRating);

                context.SaveChanges();
            }
        }
    }
}
