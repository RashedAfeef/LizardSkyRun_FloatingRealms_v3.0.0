using UnityEngine;

namespace DesertDash.Core
{
    public static class RunnerMath
    {
        public static int ClampLane(int lane)
        {
            return Mathf.Clamp(lane, -1, 1);
        }

        public static float SpeedAt(float elapsedSeconds, float startSpeed, float maximumSpeed, float acceleration)
        {
            return Mathf.Min(maximumSpeed, startSpeed + Mathf.Max(0f, elapsedSeconds) * Mathf.Max(0f, acceleration));
        }

        public static int ScoreFor(float distance, int coins, int coinScore)
        {
            return Mathf.Max(0, Mathf.FloorToInt(distance)) + Mathf.Max(0, coins) * Mathf.Max(0, coinScore);
        }

        public static int ScoreFor(float distance, int coins, int coinScore, int multiplier)
        {
            return ScoreFor(distance, coins, coinScore) * Mathf.Max(1, multiplier);
        }

        public static int BaseMultiplierAt(float distance, float stepDistance, int maximumMultiplier)
        {
            var safeStep = Mathf.Max(1f, stepDistance);
            return Mathf.Clamp(1 + Mathf.FloorToInt(Mathf.Max(0f, distance) / safeStep), 1, Mathf.Max(1, maximumMultiplier));
        }
    }
}
