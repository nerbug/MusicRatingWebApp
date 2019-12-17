using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicRatingWebApp.Helpers;
using MusicRatingWebApp.Models;
using MusicRatingWebApp.Repositories.Contracts;

namespace MusicRatingWebApp.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly MusicRatingWebAppDbContext _context;
        private readonly IArtistRepository repository;

        public ArtistsController(MusicRatingWebAppDbContext context, IArtistRepository repository)
        {
            _context = context;
            this.repository = repository;
        }

        // GET: Artists
        public IActionResult Index()
        {
            var artists = repository.GetArtists();
            var viewModel = artists.Select(artist => ArtistMapper.MapToSimpleOutputDto(artist, repository)).ToList();
            return View(viewModel);
        }

        // GET: Artists/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("NotFound");
            }

            var artist = repository.GetArtist(id.Value);
            if (artist == null)
            {
                return new NotFoundViewResult("NotFound");
            }

            var viewModel = ArtistMapper.MapToDetailedOutputDto(artist, repository);

            return View(viewModel);
        }

        // GET: Artists/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        // GET: Artists/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("NotFound");
            }

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return new NotFoundViewResult("NotFound");
            }
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Artist artist)
        {
            if (id != artist.Id)
            {
                return new NotFoundViewResult("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.Id))
                    {
                        return new NotFoundViewResult("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        // GET: Artists/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("NotFound");
            }

            var artist = await _context.Artists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return new NotFoundViewResult("NotFound");
            }

            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }
    }
}
