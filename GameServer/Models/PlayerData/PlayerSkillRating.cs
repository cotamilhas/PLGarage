using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class PlayerSkillRating
    {
        public int PlayerId { get; set; }

        public Platform Platform { get; set; }

        public int Rating { get; set; }

        public int RacesRated { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }
    }
}
