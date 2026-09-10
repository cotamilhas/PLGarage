using GameServer.Implementation.Common;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GameServer.Controllers.Api
{
    [ApiController]
    public class GamesApiController(Database database) : Controller
    {
        [HttpGet]
        [Route("/api/games")]
        public IActionResult GetGames(int page = 1, int perPage = 10)
        {
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 10) perPage = 10;

            var games = Games.GetGames().OrderBy(game => game.Id).ToList();
            var total = games.Count;
            var pagedGames = games.Skip(PageCalculator.GetPageStart(page, perPage)).Take(perPage).ToList();
            var playerIds = pagedGames
                .SelectMany(game => game.Players.Select(player => player.PlayerId).Append(game.HostPlayerId))
                .Distinct()
                .ToList();
            var usernames = database.Users
                .Where(user => playerIds.Contains(user.UserId))
                .ToDictionary(user => user.UserId, user => user.Username);

            var response = pagedGames
                .Select(game => new GamesResponse
                {
                    Id = game.Id,
                    HostPlayerId = game.HostPlayerId,
                    HostUsername = usernames[game.HostPlayerId],
                    MaxPlayers = game.MaxPlayers,
                    MinPlayers = game.MinPlayers,
                    IsMNR = game.IsMNR,
                    Type = game.Type.ToString(),
                    Platform = game.Platform.ToString(),
                    NumberLaps = game.NumberLaps,
                    SpeedClass = game.SpeedClass,
                    IsRanked = game.IsRanked,
                    Track = game.Track,
                    Privacy = game.Privacy,
                    State = game.State.ToString(),
                    Players = game.Players.Select(player => new GamesPlayerResponse
                    {
                        PlayerId = player.PlayerId,
                        Username = usernames[player.PlayerId],
                        State = player.State.ToString(),
                        HasFinished = player.HasFinished
                    }).ToList()
                }).ToList();

            return Json(new
            {
                total,
                games = response
            });
        }
    }
}
