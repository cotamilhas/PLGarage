using Newtonsoft.Json;
using System;
using System.IO;

namespace GameServer.Models.Config
{
    public class SkillRatingConfig
    {
        private static readonly Lazy<SkillRatingConfig> instance = new(GetFromFile);

        public static SkillRatingConfig Instance => instance.Value;

        public int StartingRating { get; set; } = 1500;
        public int KFactor { get; set; } = 32;
        public int RatingScale { get; set; } = 400;

        private static SkillRatingConfig GetFromFile()
        {
            SkillRatingConfig config;
            if (File.Exists("./skill_rating.json"))
            {
                string file = File.ReadAllText("./skill_rating.json");
                config = JsonConvert.DeserializeObject<SkillRatingConfig>(file)
                    ?? throw new InvalidDataException("skill_rating.json contains invalid configuration.");
            }
            else
            {
                config = new SkillRatingConfig();
                File.WriteAllText("./skill_rating.json", JsonConvert.SerializeObject(config, Formatting.Indented));
            }

            if (config.StartingRating < 0 || config.KFactor <= 0 || config.RatingScale <= 0)
                throw new InvalidDataException("Skill rating values must be valid: starting rating cannot be negative; K-factor and rating scale must be positive.");

            return config;
        }
    }
}
