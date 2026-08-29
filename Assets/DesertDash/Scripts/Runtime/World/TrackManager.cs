using System.Collections.Generic;
using DesertDash.Core;
using UnityEngine;

namespace DesertDash.World
{
    public sealed class TrackManager : MonoBehaviour
    {
        private readonly List<TrackSegment> _segments = new List<TrackSegment>();
        private Transform _runner;
        private RunnerConfig _config;
        private GameManager _game;
        private float _furthestStartZ;

        public void Initialize(Transform runner, GameManager game, RunnerConfig config, RuntimeMaterialLibrary materials)
        {
            _runner = runner;
            _game = game;
            _config = config;
            var pool = new WorldObjectPool(transform, materials);
            var firstStart = -15f;

            for (var i = 0; i < config.visibleSegments; i++)
            {
                var segment = new TrackSegment(i, transform, config, materials, pool);
                var startZ = firstStart + i * config.segmentLength;
                segment.Place(startZ, config.initialDifficulty, startZ < config.safeStartDistance);
                _segments.Add(segment);
                _furthestStartZ = startZ;
            }
        }

        private void Update()
        {
            if (_runner == null || _game == null || _game.State == GameState.Ready)
            {
                return;
            }

            for (var i = 0; i < _segments.Count; i++)
            {
                var segment = _segments[i];
                if (_runner.position.z <= segment.StartZ + _config.segmentLength + 5f)
                {
                    continue;
                }

                _furthestStartZ += _config.segmentLength;
                segment.Place(_furthestStartZ, _game.Difficulty, false);
            }
        }
    }
}
