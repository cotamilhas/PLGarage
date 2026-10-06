using System;
using System.Collections.Generic;

namespace GameServer.Implementation.Common
{
    public readonly record struct SkillRatingParticipant(int Rating, int Rank);

    public static class SkillRating
    {
        public static int[] Calculate(
            IReadOnlyList<SkillRatingParticipant> participants,
            int kFactor,
            int ratingScale)
        {
            if (kFactor <= 0)
                throw new ArgumentOutOfRangeException(nameof(kFactor), "K-factor must be positive.");
            if (ratingScale <= 0)
                throw new ArgumentOutOfRangeException(nameof(ratingScale), "Rating scale must be positive.");

            var changes = new int[participants.Count];

            for (int i = 0; i < participants.Count; i++)
            {
                double scoreDifference = 0;
                for (int j = 0; j < participants.Count; j++)
                {
                    if (i == j)
                        continue;

                    var participant = participants[i];
                    var opponent = participants[j];
                    double actualScore = participant.Rank < opponent.Rank
                        ? 1.0
                        : participant.Rank == opponent.Rank
                            ? 0.5
                            : 0.0;
                    double ratingDifference = (double)opponent.Rating - participant.Rating;
                    double expectedScore = 1.0 / (1.0 + Math.Pow(10.0, ratingDifference / ratingScale));
                    scoreDifference += actualScore - expectedScore;
                }

                double averageScoreDifference = scoreDifference / (participants.Count - 1);
                changes[i] = (int)Math.Round(kFactor * averageScoreDifference, MidpointRounding.AwayFromZero);
            }

            return changes;
        }
    }
}
