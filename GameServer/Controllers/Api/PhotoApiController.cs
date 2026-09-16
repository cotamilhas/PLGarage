using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using GameServer.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GameServer.Controllers.Api
{
    [ApiController]
    public class PhotoApiController(Database database) : Controller
    {
        [HttpGet]
        [Route("/api/photos/{id}")]
        public IActionResult GetPhotoById(int id)
        {
            var photo = database.PlayerCreations
                .AsNoTracking()
                .Include(c => c.Author)
                .FirstOrDefault(c => c.Type == PlayerCreationType.PHOTO 
                && c.PlayerCreationId == id);

            if (photo == null)
                return NotFound(new { error = "error_photo_not_found" });

            return Json(new
            {
                photo.PlayerCreationId,
                photo.AssociatedUsernames,
                photo.TrackId,
                AuthorUsername = photo.Author.Username,
                photo.ModerationStatus,
                photo.CreatedAt
            });
        }

        [HttpGet]
        [Route("/api/photos")]
        public IActionResult GetPhotos(
            string username = null,
            int? trackId = null,
            int page = 1,
            int perPage = 10,
            SortOrder? sortOrder = null)
        {
            if (page < 1) page = 1;
            if (perPage < 1 || perPage > 10) perPage = 10;

            var q = database.PlayerCreations
                .AsNoTracking()
                .Where(c => c.Type == PlayerCreationType.PHOTO
                && c.ModerationStatus != ModerationStatus.BANNED
                && c.ModerationStatus != ModerationStatus.ILLEGAL);

            if (!string.IsNullOrWhiteSpace(username))
            {
                if (!database.Users.AsNoTracking().Any(u => u.Username == username))
                    return NotFound(new { error = "error_photos_not_found_for_user" });

                q = q.Where(c => c.Author.Username == username);
            }

            if (trackId.HasValue)
                q = q.Where(c => c.TrackId == trackId.Value);

            var orderedQuery = ((sortOrder ?? SortOrder.desc) == SortOrder.asc)
                ? q.OrderBy(p => p.CreatedAt)
                : q.OrderByDescending(p => p.CreatedAt);

            var total = q.Count();

            if (total == 0)
                return NotFound(new { error = "error_photos_not_found" });

            var photos = orderedQuery
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(c => new
                {
                    c.PlayerCreationId,
                    c.AssociatedUsernames,
                    c.TrackId,
                    takenBy = c.Author.Username,
                    c.CreatedAt
                })
                .ToList();

            return Json(new {total, photos});
        }
        
        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}