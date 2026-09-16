using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using GameServer.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GameServer.Controllers.Api
{
    [ApiController]
    public class ItemApiController(Database database) : Controller
    {
        [HttpGet]
        [Route("/api/item/{id}")]
        public IActionResult GetItemById(int id)
        {
            var item = database.PlayerCreations
                .AsNoTracking()
                .Include(c => c.Author)
                .FirstOrDefault(c => c.Type == PlayerCreationType.ITEM 
                && c.PlayerCreationId == id);

            if (item == null)
                return NotFound(new { error = "error_item_not_found" });

            return Json(new
            {
                item.PlayerCreationId,
                item.Name,
                item.Description,
                creatorUsername = item.Author.Username,
                item.ModerationStatus,
                item.CreatedAt
            });
        }

        [HttpGet]
        [Route("/api/items")]
        public IActionResult GetItems(
            string query,
            string username = null, 
            int page = 1, int perPage = 10, 
            SortOrder? sortOrder = null)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var q = database.PlayerCreations
                .AsNoTracking()
                .Where(c => c.Type == PlayerCreationType.ITEM
                && c.ModerationStatus != ModerationStatus.BANNED
                && c.ModerationStatus != ModerationStatus.ILLEGAL);

            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(c => c.Name.Contains(query));

            if (!string.IsNullOrWhiteSpace(username))
            {
                if (!database.Users.AsNoTracking().Any(u => u.Username == username))
                    return NotFound(new { error = "error_items_not_found_for_user" });

                q = q.Where(c => c.Author.Username == username);
            }

            var orderedQuery = ((sortOrder ?? SortOrder.desc) == SortOrder.asc)
                ? q.OrderBy(p => p.CreatedAt)
                : q.OrderByDescending(p => p.CreatedAt);

            var total = q.Count();

            if (total == 0)
                return NotFound(new { error = "error_items_not_found" });

            var items = orderedQuery
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(c => new
                {
                    c.PlayerCreationId,
                    c.Name,
                    c.Description,
                    creatorUsername = c.Author.Username,
                    c.CreatedAt
                })
                .ToList();

            return Json(new {total, items});
        }
        
        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }
    }
}