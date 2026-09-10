using System.Collections.Generic;

namespace GameServer.Models.Response
{
    public class GamesResponse
    {
        public int Id { get; set; }
        public int HostPlayerId { get; set; }
        public string HostUsername { get; set; }
        public int MaxPlayers { get; set; }
        public int MinPlayers { get; set; }
        public bool IsMNR { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string Platform { get; set; }
        public string SpeedClass { get; set; }
        public bool IsRanked { get; set; }
        public int Track { get; set; }
        public string Privacy { get; set; }
        public int NumberLaps { get; set; }
        public List<GamesPlayerResponse> Players { get; set; }
    }

    public class GamesPlayerResponse
    {
        public int PlayerId { get; set; }
        public string Username { get; set; }
        public string State { get; set; }
        public bool HasFinished { get; set; }
    }
}