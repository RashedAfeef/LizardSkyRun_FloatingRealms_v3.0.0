using DesertDash.Core;
using NUnit.Framework;

namespace DesertDash.Tests
{
    public sealed class RunnerMathTests
    {
        [TestCase(-9, -1)]
        [TestCase(-1, -1)]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(8, 1)]
        public void ClampLane_AlwaysReturnsOneOfThreeLanes(int input, int expected)
        {
            Assert.That(RunnerMath.ClampLane(input), Is.EqualTo(expected));
        }

        [Test]
        public void SpeedAt_AcceleratesAndStopsAtMaximum()
        {
            Assert.That(RunnerMath.SpeedAt(10f, 10f, 20f, 0.5f), Is.EqualTo(15f));
            Assert.That(RunnerMath.SpeedAt(100f, 10f, 20f, 0.5f), Is.EqualTo(20f));
        }

        [Test]
        public void SpeedAt_NeverRewindsForNegativeTime()
        {
            Assert.That(RunnerMath.SpeedAt(-4f, 10f, 20f, 0.5f), Is.EqualTo(10f));
        }

        [Test]
        public void ScoreFor_CombinesDistanceAndCoins()
        {
            Assert.That(RunnerMath.ScoreFor(123.9f, 4, 12), Is.EqualTo(171));
        }

        [Test]
        public void ScoreFor_ClampsNegativeInputs()
        {
            Assert.That(RunnerMath.ScoreFor(-5f, -2, 12), Is.Zero);
        }

        [Test]
        public void ScoreFor_WithMultiplier_MultipliesTheWholeAward()
        {
            Assert.That(RunnerMath.ScoreFor(100f, 5, 10, 3), Is.EqualTo(450));
        }

        [TestCase(0f, 1)]
        [TestCase(239f, 1)]
        [TestCase(240f, 2)]
        [TestCase(9999f, 5)]
        public void BaseMultiplierAt_ProgressesAndClamps(float distance, int expected)
        {
            Assert.That(RunnerMath.BaseMultiplierAt(distance, 240f, 5), Is.EqualTo(expected));
        }

        [Test]
        public void RunMission_UsesARepeatableThreeMissionRotation()
        {
            Assert.That(RunMission.CreateForRun(0).Type, Is.EqualTo(RunMissionType.CollectCoins));
            Assert.That(RunMission.CreateForRun(1).Type, Is.EqualTo(RunMissionType.ReachDistance));
            Assert.That(RunMission.CreateForRun(2).Type, Is.EqualTo(RunMissionType.ScorePoints));
        }
    }
}
