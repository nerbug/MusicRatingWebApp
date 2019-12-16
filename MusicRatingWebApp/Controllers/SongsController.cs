using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Models.Other;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Controllers
{
    public class SongsController : Controller
    {
        private readonly ISongRepository songRepository;
        private readonly IRatingRepository ratingRepository;
        private readonly MusicRatingWebAppDbContext _context;

        public SongsController(ISongRepository songRepository, IRatingRepository ratingRepository, MusicRatingWebAppDbContext context)
        {
            this.songRepository = songRepository;
            this.ratingRepository = ratingRepository;
            _context = context;
        }

        // GET: Songs
        public async Task<IActionResult> Index()
        {
            var musicRatingWebAppDbContext = _context.Songs.Include(s => s.Artist);
            return View(await musicRatingWebAppDbContext.ToListAsync());
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            var songDto = SongMapper.MapToDetailedOutputDto(song, songRepository);
            SongDetailViewModel viewModel;

            // Get current user's ID, if it exists
            Claim currentUserIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (currentUserIdClaim == null)
            {
                // User is not logged in
                viewModel = new SongDetailViewModel
                {
                    Song = songDto,
                    UsersCurrentRating = 0
                };
            }
            else
            {
                // User is logged in, check if the user has rated the song
                int userId = int.Parse(currentUserIdClaim.Value);
                Rating rating = ratingRepository.GetRatingWithUserAndSongIds(userId, song.Id);
                if (rating == null)
                    // User hasn't rated the song
                    viewModel = new SongDetailViewModel
                    {
                        Song = songDto,
                        UsersCurrentRating = 0
                    };
                else
                {
                    // User has rated the song, get current rating
                    viewModel = new SongDetailViewModel
                    {
                        Song = songDto,
                        UsersCurrentRating = rating.NumberOfStars
                    };
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public IActionResult Details(int? id, int CurrentUsersRating)
        {
            if (!id.HasValue)
                return BadRequest();

            int userId = int.Parse(User.Claims.First(c => c.Type == "userId").Value);
            int songId = id.Value;
            int numberOfStars = CurrentUsersRating;

            // Check if rating already exists in the database. If so, we update it.
            Rating rating = ratingRepository.GetRatingWithUserAndSongIds(userId, songId);
            if (rating != null)
            {
                rating.NumberOfStars = numberOfStars;
                _context.Update(rating);
                _context.SaveChanges();

                return RedirectToAction(nameof(Details), new {id = songId});
            }

            // Rating doesn't exist, so we create one
            var newRating = new Rating
            {
                UserId = userId,
                SongId = songId,
                NumberOfStars = numberOfStars
            };

            _context.Ratings.Add(newRating);
            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new {id = songId});
        }

        // GET: Songs/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ArtistId,Id,Title,Year,Genre")] Song song)
        {
            var byArtist = _context.Artists.FirstOrDefault(a => a.Id == song.ArtistId);
            var modelValid = byArtist != null && !string.IsNullOrWhiteSpace(song.Title) &&
                             !string.IsNullOrWhiteSpace(song.Genre) && song.Year >= 0;
            if (modelValid)
            {
                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
            return View(song);
        }

        // GET: Songs/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ArtistId,Id,Title,Year,Genre")] Song song)
        {
            if (id != song.Id)
            {
                return NotFound();
            }

            var byArtist = _context.Artists.FirstOrDefault(a => a.Id == song.ArtistId);
            var modelValid = byArtist != null && !string.IsNullOrWhiteSpace(song.Title) &&
                             !string.IsNullOrWhiteSpace(song.Genre) && song.Year >= 0;

            if (modelValid)
            {
                try
                {
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
            return View(song);
        }

        // GET: Songs/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }
    }
}
