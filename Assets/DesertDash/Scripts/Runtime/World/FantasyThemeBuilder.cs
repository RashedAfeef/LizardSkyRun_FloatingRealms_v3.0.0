using DesertDash.Core;
using UnityEngine;

namespace DesertDash.World
{
    /// <summary>
    /// Builds a lightweight fantasy realm around each recycled track segment.
    /// All scenery is collider-free and deterministic so it remains inexpensive
    /// on mobile while the same segment can be reused safely.
    /// </summary>
    public sealed class FantasyThemeBuilder
    {
        private readonly Transform _root;
        private readonly RunnerConfig _config;
        private readonly RuntimeMaterialLibrary _materials;
        private readonly int _index;
        private readonly System.Random _random;

        public FantasyThemeBuilder(Transform root, RunnerConfig config, RuntimeMaterialLibrary materials, int index)
        {
            _root = root;
            _config = config;
            _materials = materials;
            _index = index;
            _random = new System.Random(index * 4789 + 173);
        }

        public void Build()
        {
            BuildFloatingIslands(-1);
            BuildFloatingIslands(1);
            BuildRoadsideCrystals(-1);
            BuildRoadsideCrystals(1);

            if (_index % 2 == 0)
            {
                BuildLanternArc();
            }

            switch (_index % 5)
            {
                case 0:
                    BuildPortalLandmark();
                    break;
                case 1:
                    BuildMoonTreeLandmark();
                    break;
                case 2:
                    BuildMushroomGrove();
                    break;
                case 3:
                    BuildSkyTempleRuin();
                    break;
                default:
                    BuildCrystalGarden();
                    break;
            }
        }

        private void BuildFloatingIslands(int side)
        {
            for (var islandIndex = 0; islandIndex < 2; islandIndex++)
            {
                var width = 4.4f + (float)_random.NextDouble() * 2.4f;
                var depth = 5.2f + (float)_random.NextDouble() * 3.2f;
                var x = side * (10.8f + (float)_random.NextDouble() * 5.2f);
                var y = -2.8f + (float)_random.NextDouble() * 2.3f;
                var z = 5.2f + islandIndex * 15.5f + (float)_random.NextDouble() * 3.6f;

                var island = new GameObject("FloatingMoonIsland").transform;
                island.SetParent(_root, false);
                island.localPosition = new Vector3(x, y, z);
                island.localRotation = Quaternion.Euler(0f, (float)_random.NextDouble() * 40f - 20f, 0f);

                CreatePrimitive(PrimitiveType.Sphere, "IslandRock", new Vector3(0f, -0.30f, 0f), new Vector3(width, 1.75f, depth), _materials.IslandRock, island);
                CreatePrimitive(PrimitiveType.Cylinder, "IslandMeadow", new Vector3(0f, 0.66f, 0f), new Vector3(width * 0.46f, 0.18f, depth * 0.46f), _materials.IslandGrass, island);
                CreatePrimitive(PrimitiveType.Cylinder, "IslandUnderGlow", new Vector3(0f, -1.12f, 0f), new Vector3(width * 0.28f, 0.06f, depth * 0.28f), _materials.RuneViolet, island);

                for (var pebble = 0; pebble < 3; pebble++)
                {
                    var pebbleX = side * (2.0f + pebble * 0.72f);
                    var pebbleY = -1.4f - pebble * 0.55f;
                    var stone = CreatePrimitive(PrimitiveType.Sphere, "OrbitingStone", new Vector3(pebbleX, pebbleY, -0.8f + pebble * 0.75f), new Vector3(0.58f, 0.36f, 0.70f), _materials.VoidStone, island);
                    stone.transform.localRotation = Quaternion.Euler(pebble * 17f, pebble * 31f, pebble * 9f);
                }

                if ((islandIndex + _index) % 2 == 0)
                {
                    BuildCrystalCluster(island, new Vector3(-side * width * 0.12f, 1.02f, 0.2f), 0.72f);
                }
                else
                {
                    BuildSmallMoonTree(island, new Vector3(side * width * 0.09f, 0.92f, 0f), 0.74f);
                }
            }
        }

        private void BuildRoadsideCrystals(int side)
        {
            for (var cluster = 0; cluster < 2; cluster++)
            {
                var z = 6.5f + cluster * 15f + (float)_random.NextDouble() * 3f;
                BuildCrystalCluster(_root, new Vector3(side * 6.15f, 0.18f, z), 0.78f + cluster * 0.08f);
            }
        }

        private void BuildCrystalCluster(Transform parent, Vector3 position, float scale)
        {
            var cluster = new GameObject("LuminousCrystalCluster").transform;
            cluster.SetParent(parent, false);
            cluster.localPosition = position;

            for (var shardIndex = 0; shardIndex < 5; shardIndex++)
            {
                var x = (shardIndex - 2) * 0.28f * scale;
                var height = (1.0f + (shardIndex % 3) * 0.38f) * scale;
                var material = shardIndex % 3 == 0 ? _materials.CrystalCyan : shardIndex % 3 == 1 ? _materials.CrystalViolet : _materials.CrystalRose;
                var shard = CreateBlock("CrystalShard", new Vector3(x, height * 0.5f, (shardIndex % 2 == 0 ? 0.16f : -0.12f) * scale), new Vector3(0.24f * scale, height, 0.24f * scale), material, cluster);
                shard.transform.localRotation = Quaternion.Euler(shardIndex * 6f, shardIndex * 23f, (shardIndex - 2) * 7f);
            }

            CreatePrimitive(PrimitiveType.Sphere, "CrystalGlow", new Vector3(0f, 0.24f * scale, 0f), Vector3.one * 0.72f * scale, _materials.MistGlow, cluster);
        }

        private void BuildPortalLandmark()
        {
            var side = _index % 2 == 0 ? 1 : -1;
            var portal = new GameObject("CelestialPortalLandmark").transform;
            portal.SetParent(_root, false);
            portal.localPosition = new Vector3(side * 11.2f, 0.35f, _config.segmentLength * 0.60f);
            portal.localRotation = Quaternion.Euler(0f, side * -12f, 0f);

            CreatePrimitive(PrimitiveType.Cylinder, "PortalIsland", new Vector3(0f, -0.25f, 0f), new Vector3(3.5f, 0.32f, 3.5f), _materials.IslandRock, portal);
            CreatePrimitive(PrimitiveType.Cylinder, "PortalPlatform", new Vector3(0f, 0.10f, 0f), new Vector3(2.8f, 0.10f, 2.8f), _materials.PathStone, portal);

            const int segmentCount = 16;
            for (var segment = 0; segment < segmentCount; segment++)
            {
                var angle = segment * Mathf.PI * 2f / segmentCount;
                var degrees = segment * 360f / segmentCount;
                var block = CreateBlock("PortalRune", new Vector3(Mathf.Cos(angle) * 2.25f, 3.05f + Mathf.Sin(angle) * 2.25f, 0f), new Vector3(0.34f, 0.88f, 0.48f), segment % 2 == 0 ? _materials.RuneCyan : _materials.RuneViolet, portal);
                block.transform.localRotation = Quaternion.Euler(0f, 0f, degrees + 90f);
            }

            CreatePrimitive(PrimitiveType.Sphere, "PortalVeil", new Vector3(0f, 3.05f, 0.06f), new Vector3(3.35f, 4.15f, 0.24f), _materials.PortalCore, portal);
            BuildCrystalCluster(portal, new Vector3(-2.75f, 0.25f, 0.25f), 0.92f);
            BuildCrystalCluster(portal, new Vector3(2.75f, 0.25f, 0.25f), 0.92f);

            var portalLight = new GameObject("PortalLight").AddComponent<Light>();
            portalLight.transform.SetParent(portal, false);
            portalLight.transform.localPosition = new Vector3(0f, 3.1f, -0.5f);
            portalLight.type = LightType.Point;
            portalLight.color = new Color(0.22f, 0.78f, 1f);
            portalLight.intensity = 2.2f;
            portalLight.range = 13f;
            portalLight.shadows = LightShadows.None;
        }

        private void BuildMoonTreeLandmark()
        {
            var side = _index % 2 == 0 ? -1 : 1;
            var island = new GameObject("AncientMoonTreeIsland").transform;
            island.SetParent(_root, false);
            island.localPosition = new Vector3(side * 11f, -0.55f, _config.segmentLength * 0.58f);
            CreatePrimitive(PrimitiveType.Sphere, "TreeIsland", new Vector3(0f, -0.45f, 0f), new Vector3(5.4f, 1.45f, 5.4f), _materials.IslandRock, island);
            CreatePrimitive(PrimitiveType.Cylinder, "TreeMeadow", new Vector3(0f, 0.30f, 0f), new Vector3(2.45f, 0.16f, 2.45f), _materials.IslandGrass, island);
            BuildSmallMoonTree(island, new Vector3(0f, 0.40f, 0f), 1.65f);
            BuildCrystalCluster(island, new Vector3(-side * 1.75f, 0.50f, 0.9f), 0.70f);
        }

        private void BuildSmallMoonTree(Transform parent, Vector3 position, float scale)
        {
            var tree = new GameObject("MoonBloomTree").transform;
            tree.SetParent(parent, false);
            tree.localPosition = position;

            CreatePrimitive(PrimitiveType.Cylinder, "SilverTrunk", new Vector3(0f, 1.25f * scale, 0f), new Vector3(0.22f * scale, 1.25f * scale, 0.22f * scale), _materials.MoonBark, tree);
            for (var branchIndex = -1; branchIndex <= 1; branchIndex += 2)
            {
                var branch = CreatePrimitive(PrimitiveType.Cylinder, "SilverBranch", new Vector3(branchIndex * 0.42f * scale, 2.15f * scale, 0f), new Vector3(0.11f * scale, 0.72f * scale, 0.11f * scale), _materials.MoonBark, tree);
                branch.transform.localRotation = Quaternion.Euler(0f, 0f, branchIndex * -34f);
            }

            CreatePrimitive(PrimitiveType.Sphere, "MoonCrown", new Vector3(0f, 2.78f * scale, 0f), new Vector3(1.55f, 1.05f, 1.45f) * scale, _materials.MoonLeaves, tree);
            CreatePrimitive(PrimitiveType.Sphere, "MoonCrownLeft", new Vector3(-0.82f * scale, 2.35f * scale, 0.10f), Vector3.one * 0.82f * scale, _materials.MoonLeavesViolet, tree);
            CreatePrimitive(PrimitiveType.Sphere, "MoonCrownRight", new Vector3(0.84f * scale, 2.42f * scale, -0.08f), Vector3.one * 0.86f * scale, _materials.MoonLeaves, tree);
        }

        private void BuildMushroomGrove()
        {
            for (var side = -1; side <= 1; side += 2)
            {
                for (var mushroomIndex = 0; mushroomIndex < 3; mushroomIndex++)
                {
                    var scale = 0.72f + mushroomIndex * 0.18f;
                    var mushroom = new GameObject("GiantStarMushroom").transform;
                    mushroom.SetParent(_root, false);
                    mushroom.localPosition = new Vector3(side * (7.0f + mushroomIndex * 1.8f), 0f, 7f + mushroomIndex * 8.4f);
                    CreatePrimitive(PrimitiveType.Cylinder, "PearlStem", new Vector3(0f, 0.85f * scale, 0f), new Vector3(0.24f * scale, 0.85f * scale, 0.24f * scale), _materials.MushroomStem, mushroom);
                    CreatePrimitive(PrimitiveType.Sphere, "GlowCap", new Vector3(0f, 1.72f * scale, 0f), new Vector3(1.15f, 0.34f, 1.15f) * scale, mushroomIndex % 2 == 0 ? _materials.MushroomCap : _materials.CrystalCyan, mushroom);
                    CreatePrimitive(PrimitiveType.Sphere, "CapStar", new Vector3(0f, 1.98f * scale, 0f), Vector3.one * 0.20f * scale, _materials.StarGold, mushroom);
                }
            }
        }

        private void BuildSkyTempleRuin()
        {
            var side = _index % 2 == 0 ? 1 : -1;
            var temple = new GameObject("SkyTempleRuins").transform;
            temple.SetParent(_root, false);
            temple.localPosition = new Vector3(side * 11.5f, 0f, _config.segmentLength * 0.55f);

            CreatePrimitive(PrimitiveType.Cylinder, "TempleIsland", new Vector3(0f, -0.65f, 0f), new Vector3(4.5f, 0.55f, 4.5f), _materials.IslandRock, temple);
            CreateBlock("TempleStep", new Vector3(0f, 0.12f, 0f), new Vector3(6.2f, 0.24f, 4.4f), _materials.PathStone, temple);
            for (var column = -1; column <= 1; column++)
            {
                var height = column == 0 ? 4.6f : 3.3f + (column + 1) * 0.35f;
                CreatePrimitive(PrimitiveType.Cylinder, "RuneColumn", new Vector3(column * 2.05f, height * 0.5f, 0f), new Vector3(0.42f, height * 0.5f, 0.42f), _materials.TempleStone, temple);
                CreatePrimitive(PrimitiveType.Sphere, "ColumnRune", new Vector3(column * 2.05f, height + 0.24f, 0f), Vector3.one * 0.48f, column == 0 ? _materials.RuneCyan : _materials.RuneViolet, temple);
            }
            CreateBlock("FloatingLintel", new Vector3(0f, 4.75f, 0f), new Vector3(5.4f, 0.44f, 0.72f), _materials.TempleStone, temple);
        }

        private void BuildCrystalGarden()
        {
            var garden = new GameObject("PrismaticCrystalGarden").transform;
            garden.SetParent(_root, false);
            garden.localPosition = new Vector3(0f, 0f, _config.segmentLength * 0.56f);
            BuildCrystalCluster(garden, new Vector3(-7.2f, 0.05f, -2.5f), 1.25f);
            BuildCrystalCluster(garden, new Vector3(7.2f, 0.05f, 1.2f), 1.35f);
            BuildCrystalCluster(garden, new Vector3(-9.4f, 1.2f, 4.8f), 0.92f);
            BuildCrystalCluster(garden, new Vector3(9.8f, 1.8f, -5.2f), 0.86f);
        }

        private void BuildLanternArc()
        {
            var z = _config.segmentLength - 3.2f;
            for (var lantern = -4; lantern <= 4; lantern++)
            {
                var normalized = lantern / 4f;
                var y = 5.6f + (1f - normalized * normalized) * 1.25f;
                CreatePrimitive(PrimitiveType.Sphere, "FloatingStarLantern", new Vector3(lantern * 1.30f, y, z), Vector3.one * 0.22f, lantern % 2 == 0 ? _materials.StarGold : _materials.RuneCyan, _root);
            }
        }

        private static GameObject CreateBlock(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            return CreatePrimitive(PrimitiveType.Cube, name, position, scale, material, parent);
        }

        private static GameObject CreatePrimitive(PrimitiveType type, string name, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            var primitive = GameObject.CreatePrimitive(type);
            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            primitive.transform.localPosition = position;
            primitive.transform.localScale = scale;
            primitive.GetComponent<Renderer>().sharedMaterial = material;
            var collider = primitive.GetComponent<Collider>();
            if (collider != null)
            {
                UnityEngine.Object.Destroy(collider);
            }

            return primitive;
        }
    }
}
