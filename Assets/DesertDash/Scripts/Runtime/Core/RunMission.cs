using UnityEngine;

namespace DesertDash.Core
{
    public enum RunMissionType
    {
        CollectCoins,
        ReachDistance,
        ScorePoints
    }

    public sealed class RunMission
    {
        public RunMissionType Type { get; }
        public int Target { get; }

        public RunMission(RunMissionType type, int target)
        {
            Type = type;
            Target = Mathf.Max(1, target);
        }

        public string Label
        {
            get
            {
                switch (Type)
                {
                    case RunMissionType.CollectCoins:
                        return $"Collect {Target} coins";
                    case RunMissionType.ReachDistance:
                        return $"Run {Target} m";
                    default:
                        return $"Score {Target} points";
                }
            }
        }

        public int Current(float distance, int coins, int score)
        {
            switch (Type)
            {
                case RunMissionType.CollectCoins:
                    return coins;
                case RunMissionType.ReachDistance:
                    return Mathf.FloorToInt(distance);
                default:
                    return score;
            }
        }

        public float Progress(float distance, int coins, int score)
        {
            return Mathf.Clamp01(Current(distance, coins, score) / (float)Target);
        }

        public static RunMission CreateForRun(int runsPlayed)
        {
            var tier = Mathf.Max(0, runsPlayed / 3);
            switch (Mathf.Abs(runsPlayed) % 3)
            {
                case 0:
                    return new RunMission(RunMissionType.CollectCoins, Mathf.Min(75, 28 + tier * 4));
                case 1:
                    return new RunMission(RunMissionType.ReachDistance, Mathf.Min(1400, 450 + tier * 75));
                default:
                    return new RunMission(RunMissionType.ScorePoints, Mathf.Min(5000, 1100 + tier * 250));
            }
        }
    }
}
