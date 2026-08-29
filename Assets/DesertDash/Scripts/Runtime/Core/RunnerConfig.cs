using UnityEngine;

namespace DesertDash.Core
{
    [CreateAssetMenu(fileName = "RunnerConfig", menuName = "Lizard Sky Run/Runner Config")]
    public sealed class RunnerConfig : ScriptableObject
    {
        [Header("Movement")]
        [Min(1f)] public float startSpeed = 10f;
        [Min(1f)] public float maximumSpeed = 24f;
        [Min(0f)] public float accelerationPerSecond = 0.22f;
        [Min(0.5f)] public float laneSpacing = 2.7f;
        [Min(1f)] public float laneChangeSharpness = 14f;
        [Min(0.5f)] public float jumpHeight = 2.45f;
        public float gravity = -28f;
        [Min(0.1f)] public float slideDuration = 0.85f;

        [Header("World")]
        [Min(12f)] public float segmentLength = 30f;
        [Range(5, 16)] public int visibleSegments = 9;
        [Min(0f)] public float safeStartDistance = 45f;
        [Range(0f, 1f)] public float initialDifficulty = 0.12f;
        [Min(10f)] public float fullDifficultyDistance = 950f;

        [Header("Interaction")]
        [Min(10f)] public float swipeThresholdPixels = 55f;
        [Min(0.1f)] public float shieldDuration = 8f;
        [Min(0.1f)] public float magnetDuration = 8f;
        [Min(1f)] public float magnetRadius = 7.5f;
        [Min(0.1f)] public float scoreBoostDuration = 10f;
        [Min(0)] public int coinScore = 12;

        [Header("Progression")]
        [Min(50f)] public float multiplierStepDistance = 240f;
        [Range(1, 10)] public int maximumBaseMultiplier = 5;
        [Range(0f, 1f)] public float powerUpSpawnChance = 0.22f;

        public float DifficultyAt(float distance)
        {
            var progress = Mathf.Clamp01(distance / Mathf.Max(1f, fullDifficultyDistance));
            return Mathf.Lerp(initialDifficulty, 1f, progress);
        }
    }
}
