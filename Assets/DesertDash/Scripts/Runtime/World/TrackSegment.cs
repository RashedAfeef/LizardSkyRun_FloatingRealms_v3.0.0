using System;
using System.Collections.Generic;
using DesertDash.Core;
using UnityEngine;

namespace DesertDash.World
{
    public sealed class TrackSegment
    {
        private readonly Transform _root;
        private readonly Transform _gameplayRoot;
        private readonly RunnerConfig _config;
        private readonly WorldObjectPool _pool;
        private readonly List<GameObject> _spawned = new List<GameObject>();

        public float StartZ => _root.position.z;

        public TrackSegment(int index, Transform parent, RunnerConfig config, RuntimeMaterialLibrary materials, WorldObjectPool pool)
        {
            _config = config;
            _pool = pool;
            _root = new GameObject($"TrackSegment_{index:00}").transform;
            _root.SetParent(parent, false);
            _gameplayRoot = new GameObject("Gameplay").transform;
            _gameplayRoot.SetParent(_root, false);
            BuildStaticGeometry(materials, index);
        }

        public void Place(float startZ, float difficulty, bool safe)
        {
            _root.position = new Vector3(0f, 0f, startZ);
            ReleaseGameplay();
            SpawnGameplay(difficulty, safe);
        }

        private void BuildStaticGeometry(RuntimeMaterialLibrary materials, int index)
        {
            CreateBlock("FloatingMoonstoneCauseway", new Vector3(0f, -0.18f, _config.segmentLength * 0.5f), new Vector3(10.2f, 0.35f, _config.segmentLength), materials.Road, _root);
            CreateBlock("CausewayUnderside", new Vector3(0f, -0.82f, _config.segmentLength * 0.5f), new Vector3(8.8f, 1.05f, _config.segmentLength), materials.VoidStone, _root, false);
            CreateBlock("CausewayUnderGlow", new Vector3(0f, -1.38f, _config.segmentLength * 0.5f), new Vector3(6.6f, 0.08f, _config.segmentLength), materials.RuneViolet, _root, false);
            CreateBlock("LeftRuneEdge", new Vector3(-5.05f, 0.06f, _config.segmentLength * 0.5f), new Vector3(0.16f, 0.16f, _config.segmentLength), materials.RuneCyan, _root, false);
            CreateBlock("RightRuneEdge", new Vector3(5.05f, 0.06f, _config.segmentLength * 0.5f), new Vector3(0.16f, 0.16f, _config.segmentLength), materials.RuneCyan, _root, false);

            for (var curb = 0; curb < 10; curb++)
            {
                var material = curb % 2 == 0 ? materials.RuneViolet : materials.RuneCyan;
                var z = 1.5f + curb * 3f;
                CreateBlock("LeftEdgeRune", new Vector3(-5.13f, 0.14f, z), new Vector3(0.34f, 0.22f, 1.75f), material, _root, false);
                CreateBlock("RightEdgeRune", new Vector3(5.13f, 0.14f, z), new Vector3(0.34f, 0.22f, 1.75f), material, _root, false);
            }

            for (var laneDivider = -1; laneDivider <= 1; laneDivider += 2)
            {
                for (var marker = 0; marker < 5; marker++)
                {
                    CreateBlock("GlowingLaneRune", new Vector3(laneDivider * _config.laneSpacing * 0.5f, 0.015f, 2.2f + marker * 6f), new Vector3(0.10f, 0.03f, 2.5f), materials.Lane, _root, false);
                }
            }

            new FantasyThemeBuilder(_root, _config, materials, index).Build();
        }

        private void SpawnGameplay(float difficulty, bool safe)
        {
            var seed = Mathf.RoundToInt(StartZ * 19f) ^ 0x51D4;
            var random = new System.Random(seed);

            if (safe)
            {
                SpawnCoinTrail(0, 6f, 7, 2.7f);
                return;
            }

            var rowCount = difficulty > 0.48f ? 2 : 1;
            var lastSafeLane = 0;
            for (var row = 0; row < rowCount; row++)
            {
                var rowZ = 8f + row * 13f;
                lastSafeLane = SpawnChallengeRow(random, difficulty, rowZ, row);
            }

            if (difficulty > 0.16f && random.NextDouble() < _config.powerUpSpawnChance)
            {
                var powerRoll = random.NextDouble();
                var powerUp = powerRoll < 0.38 ? WorldObjectKind.PulseBoard : powerRoll < 0.70 ? WorldObjectKind.CoinMagnet : WorldObjectKind.ScoreBoost;
                Spawn(powerUp, lastSafeLane, _config.segmentLength - 2.4f, 1.15f);
            }
        }

        private int SpawnChallengeRow(System.Random random, float difficulty, float rowZ, int rowIndex)
        {
            var roll = random.NextDouble();
            if (roll < 0.30)
            {
                var safeLane = random.Next(-1, 2);
                var singleBlockedLane = safeLane == -1 ? 0 : -1;
                for (var lane = -1; lane <= 1; lane++)
                {
                    if (lane != safeLane && (difficulty > 0.56f || lane == singleBlockedLane))
                    {
                        Spawn(RandomObstacle(random, difficulty), lane, rowZ, 0f);
                    }
                }

                SpawnCoinTrail(safeLane, rowZ - 4f, 5, 1.55f);
                return safeLane;
            }

            if (roll < 0.52)
            {
                var lane = random.Next(-1, 2);
                Spawn(WorldObjectKind.RoadworkBarrier, lane, rowZ, 0f);
                SpawnCoinArc(lane, rowZ - 3.3f, 6, 1.25f, 2.65f);
                return lane;
            }

            if (roll < 0.70)
            {
                var lane = random.Next(-1, 2);
                Spawn(WorldObjectKind.SouqAwning, lane, rowZ, 0f);
                SpawnCoinTrail(lane, rowZ - 3.2f, 5, 1.45f);
                return lane;
            }

            if (roll < 0.84 && difficulty > 0.30f && rowIndex == 0)
            {
                var blockedLane = random.Next(-1, 2);
                var safeLane = blockedLane == 0 ? (random.NextDouble() < 0.5 ? -1 : 1) : 0;
                Spawn(WorldObjectKind.DowntownBus, blockedLane, rowZ + 1.5f, 0f);
                SpawnCoinTrail(safeLane, rowZ - 4f, 7, 1.55f);
                return safeLane;
            }

            var obstacleLane = random.Next(-1, 2);
            Spawn(WorldObjectKind.ProduceCart, obstacleLane, rowZ, 0f);
            var firstSafe = obstacleLane == -1 ? 0 : -1;
            SpawnCoinZigzag(firstSafe, rowZ - 5f, 7, 1.45f);
            return firstSafe;
        }

        private static WorldObjectKind RandomObstacle(System.Random random, float difficulty)
        {
            var roll = random.NextDouble();
            if (roll < 0.38)
            {
                return WorldObjectKind.RoadworkBarrier;
            }

            if (roll < 0.70 || difficulty < 0.25f)
            {
                return WorldObjectKind.ProduceCart;
            }

            if (roll < 0.86 && difficulty > 0.38f)
            {
                return WorldObjectKind.YellowTaxi;
            }

            return WorldObjectKind.SouqAwning;
        }

        private void SpawnCoinTrail(int lane, float startZ, int count, float spacing)
        {
            for (var i = 0; i < count; i++)
            {
                Spawn(WorldObjectKind.Coin, lane, startZ + i * spacing, 1f);
            }
        }

        private void SpawnCoinArc(int lane, float startZ, int count, float spacing, float peakHeight)
        {
            for (var i = 0; i < count; i++)
            {
                var normalized = count <= 1 ? 0f : i / (float)(count - 1);
                var arc = Mathf.Sin(normalized * Mathf.PI);
                Spawn(WorldObjectKind.Coin, lane, startZ + i * spacing, 1f + arc * (peakHeight - 1f));
            }
        }

        private void SpawnCoinZigzag(int firstLane, float startZ, int count, float spacing)
        {
            for (var i = 0; i < count; i++)
            {
                var lane = RunnerMath.ClampLane(firstLane + (i / 2) % 2);
                Spawn(WorldObjectKind.Coin, lane, startZ + i * spacing, 1f);
            }
        }

        private void Spawn(WorldObjectKind kind, int lane, float localZ, float y)
        {
            var item = _pool.Get(kind, _gameplayRoot, new Vector3(lane * _config.laneSpacing, y, localZ));
            _spawned.Add(item);
        }

        private void ReleaseGameplay()
        {
            for (var i = 0; i < _spawned.Count; i++)
            {
                _pool.Release(_spawned[i]);
            }

            _spawned.Clear();
        }

        private static GameObject CreateBlock(string name, Vector3 position, Vector3 scale, Material material, Transform parent, bool keepCollider = true)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetParent(parent, false);
            block.transform.localPosition = position;
            block.transform.localScale = scale;
            block.GetComponent<Renderer>().sharedMaterial = material;
            if (!keepCollider)
            {
                UnityEngine.Object.Destroy(block.GetComponent<Collider>());
            }

            return block;
        }

    }
}
