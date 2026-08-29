using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesertDash.World
{
    public sealed class WorldObjectPool
    {
        private readonly Transform _poolRoot;
        private readonly RuntimeMaterialLibrary _materials;
        private readonly Dictionary<WorldObjectKind, Stack<GameObject>> _available = new Dictionary<WorldObjectKind, Stack<GameObject>>();
        private readonly Dictionary<GameObject, WorldObjectKind> _kinds = new Dictionary<GameObject, WorldObjectKind>();

        public WorldObjectPool(Transform parent, RuntimeMaterialLibrary materials)
        {
            _materials = materials;
            _poolRoot = new GameObject("WorldObjectPool").transform;
            _poolRoot.SetParent(parent, false);

            foreach (WorldObjectKind kind in Enum.GetValues(typeof(WorldObjectKind)))
            {
                _available[kind] = new Stack<GameObject>();
            }
        }

        public GameObject Get(WorldObjectKind kind, Transform parent, Vector3 localPosition)
        {
            var item = _available[kind].Count > 0 ? _available[kind].Pop() : Create(kind);
            item.transform.SetParent(parent, false);
            item.transform.localPosition = localPosition;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;
            item.SetActive(true);

            var pickup = item.GetComponent<PooledPickup>();
            if (pickup != null)
            {
                pickup.Activate(DeactivateUntilSegmentRecycle);
            }

            return item;
        }

        public void Release(GameObject item)
        {
            if (item == null || !_kinds.TryGetValue(item, out var kind))
            {
                return;
            }

            item.SetActive(false);
            item.transform.SetParent(_poolRoot, false);
            _available[kind].Push(item);
        }

        private GameObject Create(WorldObjectKind kind)
        {
            GameObject item;
            switch (kind)
            {
                case WorldObjectKind.Coin:
                    item = CreateCoin();
                    break;
                case WorldObjectKind.RoadworkBarrier:
                    item = CreateRoadworkBarrier();
                    break;
                case WorldObjectKind.ProduceCart:
                    item = CreateProduceCart();
                    break;
                case WorldObjectKind.SouqAwning:
                    item = CreateSouqAwning();
                    break;
                case WorldObjectKind.PulseBoard:
                    item = CreateShield();
                    break;
                case WorldObjectKind.CoinMagnet:
                    item = CreateCoinMagnet();
                    break;
                case WorldObjectKind.ScoreBoost:
                    item = CreateScoreBoost();
                    break;
                case WorldObjectKind.DowntownBus:
                    item = CreateDowntownBus();
                    break;
                case WorldObjectKind.YellowTaxi:
                    item = CreateYellowTaxi();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

            item.name = GetFantasyName(kind);
            _kinds[item] = kind;
            return item;
        }

        private static string GetFantasyName(WorldObjectKind kind)
        {
            switch (kind)
            {
                case WorldObjectKind.Coin: return "StarToken";
                case WorldObjectKind.RoadworkBarrier: return "LowCrystalFence";
                case WorldObjectKind.ProduceCart: return "ArcaneRuneTotem";
                case WorldObjectKind.SouqAwning: return "LowAetherGate";
                case WorldObjectKind.PulseBoard: return "AetherPulseBoard";
                case WorldObjectKind.CoinMagnet: return "StarMagnet";
                case WorldObjectKind.ScoreBoost: return "RuneScoreBoost";
                case WorldObjectKind.DowntownBus: return "SleepingSkySerpent";
                case WorldObjectKind.YellowTaxi: return "FloatingRuneBoulder";
                default: return kind.ToString();
            }
        }

        private GameObject CreateCoin()
        {
            var root = CreateRootWithTrigger<CoinPickup>(new Vector3(0.85f, 1.05f, 0.85f));
            root.AddComponent<PickupSpin>();
            var visual = CreatePrimitive(PrimitiveType.Cylinder, "StarToken", root.transform, _materials.Coin);
            visual.transform.localScale = new Vector3(0.43f, 0.09f, 0.43f);
            visual.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            var core = CreatePrimitive(PrimitiveType.Cylinder, "RuneCore", root.transform, _materials.RuneCyan);
            core.transform.localScale = new Vector3(0.19f, 0.105f, 0.19f);
            core.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            var starA = CreatePrimitive(PrimitiveType.Cube, "StarRay", root.transform, _materials.StarGold);
            starA.transform.localScale = new Vector3(0.08f, 0.53f, 0.07f);
            var starB = CreatePrimitive(PrimitiveType.Cube, "StarRay", root.transform, _materials.StarGold);
            starB.transform.localScale = new Vector3(0.08f, 0.53f, 0.07f);
            starB.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            return root;
        }

        private GameObject CreateShield()
        {
            var root = CreateRootWithTrigger<ShieldPickup>(new Vector3(1.1f, 1.35f, 1.1f));
            root.AddComponent<PickupSpin>();
            var outer = CreatePrimitive(PrimitiveType.Sphere, "ShieldOrb", root.transform, _materials.Shield);
            outer.transform.localScale = Vector3.one * 0.72f;
            var core = CreatePrimitive(PrimitiveType.Cube, "BoardCore", root.transform, _materials.Coin);
            core.transform.localScale = new Vector3(0.62f, 0.10f, 0.26f);
            core.transform.localRotation = Quaternion.Euler(0f, 0f, -18f);
            return root;
        }

        private GameObject CreateCoinMagnet()
        {
            var root = CreateRootWithTrigger<CoinMagnetPickup>(new Vector3(1.2f, 1.35f, 1.2f));
            root.AddComponent<PickupSpin>();
            var orb = CreatePrimitive(PrimitiveType.Sphere, "MagnetOrb", root.transform, _materials.Shield);
            orb.transform.localScale = Vector3.one * 0.78f;
            var left = CreatePrimitive(PrimitiveType.Cube, "LeftPole", root.transform, _materials.Magnet);
            left.transform.localPosition = new Vector3(-0.23f, 0f, 0f);
            left.transform.localScale = new Vector3(0.18f, 0.58f, 0.16f);
            var right = CreatePrimitive(PrimitiveType.Cube, "RightPole", root.transform, _materials.Magnet);
            right.transform.localPosition = new Vector3(0.23f, 0f, 0f);
            right.transform.localScale = new Vector3(0.18f, 0.58f, 0.16f);
            var bridge = CreatePrimitive(PrimitiveType.Cube, "Bridge", root.transform, _materials.White);
            bridge.transform.localPosition = new Vector3(0f, -0.23f, 0f);
            bridge.transform.localScale = new Vector3(0.46f, 0.16f, 0.16f);
            return root;
        }

        private GameObject CreateScoreBoost()
        {
            var root = CreateRootWithTrigger<ScoreBoostPickup>(new Vector3(1.15f, 1.35f, 1.15f));
            root.AddComponent<PickupSpin>();
            var orb = CreatePrimitive(PrimitiveType.Sphere, "BoostOrb", root.transform, _materials.ScoreBoost);
            orb.transform.localScale = Vector3.one * 0.76f;
            var xTop = CreatePrimitive(PrimitiveType.Cube, "XTop", root.transform, _materials.White);
            xTop.transform.localScale = new Vector3(0.15f, 0.62f, 0.15f);
            xTop.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            var xBottom = CreatePrimitive(PrimitiveType.Cube, "XBottom", root.transform, _materials.White);
            xBottom.transform.localScale = new Vector3(0.15f, 0.62f, 0.15f);
            xBottom.transform.localRotation = Quaternion.Euler(0f, 0f, -45f);
            return root;
        }

        private GameObject CreateDowntownBus()
        {
            var root = CreateHazardRoot(new Vector3(2.25f, 2.75f, 6.8f), new Vector3(0f, 1.375f, 0f));
            root.name = "SleepingSkySerpent";
            for (var segment = -2; segment <= 2; segment++)
            {
                var body = CreatePrimitive(PrimitiveType.Sphere, "SerpentStoneSegment", root.transform, segment % 2 == 0 ? _materials.VoidStone : _materials.PathStone);
                body.transform.localPosition = new Vector3(Mathf.Sin(segment * 1.4f) * 0.10f, 1.22f + Mathf.Abs(segment) * 0.06f, segment * 1.25f);
                body.transform.localScale = new Vector3(2.18f, 2.25f, 1.55f);
            }

            var head = CreatePrimitive(PrimitiveType.Sphere, "SerpentHead", root.transform, _materials.TempleStone);
            head.transform.localPosition = new Vector3(0f, 1.48f, -2.85f);
            head.transform.localScale = new Vector3(2.22f, 2.34f, 1.78f);
            for (var eyeSide = -1; eyeSide <= 1; eyeSide += 2)
            {
                var eye = CreatePrimitive(PrimitiveType.Sphere, "SleepingRuneEye", root.transform, _materials.RuneCyan);
                eye.transform.localPosition = new Vector3(eyeSide * 0.52f, 1.74f, -3.66f);
                eye.transform.localScale = new Vector3(0.21f, 0.11f, 0.08f);
            }

            for (var crystal = -2; crystal <= 2; crystal++)
            {
                var spine = CreatePrimitive(PrimitiveType.Cube, "CrystalSpine", root.transform, crystal % 2 == 0 ? _materials.CrystalViolet : _materials.CrystalCyan);
                spine.transform.localPosition = new Vector3(0f, 2.42f, crystal * 1.12f);
                spine.transform.localScale = new Vector3(0.28f, 0.86f, 0.28f);
                spine.transform.localRotation = Quaternion.Euler(14f, crystal * 17f, 18f);
            }

            return root;
        }

        private GameObject CreateYellowTaxi()
        {
            var root = CreateHazardRoot(new Vector3(2.15f, 1.38f, 4.25f), new Vector3(0f, 0.69f, 0f));
            root.name = "FloatingRuneBoulder";
            var rock = CreatePrimitive(PrimitiveType.Sphere, "RuneBoulder", root.transform, _materials.VoidStone);
            rock.transform.localPosition = new Vector3(0f, 0.74f, 0f);
            rock.transform.localScale = new Vector3(2.12f, 1.34f, 4.15f);
            rock.transform.localRotation = Quaternion.Euler(8f, 12f, -5f);

            for (var rune = -2; rune <= 2; rune++)
            {
                var marker = CreatePrimitive(PrimitiveType.Cube, "BoulderRune", root.transform, rune % 2 == 0 ? _materials.RuneCyan : _materials.RuneViolet);
                marker.transform.localPosition = new Vector3((rune % 2) * 0.42f, 0.80f + Mathf.Abs(rune) * 0.12f, rune * 0.58f);
                marker.transform.localScale = new Vector3(0.08f, 0.30f, 0.26f);
                marker.transform.localRotation = Quaternion.Euler(0f, rune * 15f, rune * 18f);
            }

            return root;
        }

        private GameObject CreateRoadworkBarrier()
        {
            var root = CreateHazardRoot(new Vector3(2.25f, 0.82f, 0.75f), new Vector3(0f, 0.41f, 0f));
            root.name = "LowCrystalFence";
            var baseStone = CreatePrimitive(PrimitiveType.Cube, "CrystalFenceBase", root.transform, _materials.VoidStone);
            baseStone.transform.localPosition = new Vector3(0f, 0.18f, 0f);
            baseStone.transform.localScale = new Vector3(2.25f, 0.34f, 0.75f);
            for (var shard = -3; shard <= 3; shard++)
            {
                var crystal = CreatePrimitive(PrimitiveType.Cube, "FenceCrystal", root.transform, shard % 2 == 0 ? _materials.CrystalViolet : _materials.CrystalCyan);
                crystal.transform.localPosition = new Vector3(shard * 0.31f, 0.48f + Mathf.Abs(shard % 2) * 0.08f, 0f);
                crystal.transform.localScale = new Vector3(0.20f, 0.66f, 0.24f);
                crystal.transform.localRotation = Quaternion.Euler(0f, shard * 12f, shard * -5f);
            }
            return root;
        }

        private GameObject CreateProduceCart()
        {
            var root = CreateHazardRoot(new Vector3(2.15f, 2.1f, 1.1f), new Vector3(0f, 1.05f, 0f));
            root.name = "ArcaneRuneTotem";
            var baseStone = CreatePrimitive(PrimitiveType.Cylinder, "TotemBase", root.transform, _materials.PathStone);
            baseStone.transform.localPosition = new Vector3(0f, 0.22f, 0f);
            baseStone.transform.localScale = new Vector3(0.95f, 0.22f, 0.95f);
            var pillar = CreatePrimitive(PrimitiveType.Cube, "TotemPillar", root.transform, _materials.TempleStone);
            pillar.transform.localPosition = new Vector3(0f, 1.08f, 0f);
            pillar.transform.localScale = new Vector3(1.28f, 1.70f, 0.86f);
            var core = CreatePrimitive(PrimitiveType.Sphere, "TotemCore", root.transform, _materials.RuneRose);
            core.transform.localPosition = new Vector3(0f, 1.28f, -0.49f);
            core.transform.localScale = Vector3.one * 0.40f;
            var crown = CreatePrimitive(PrimitiveType.Cube, "TotemCrown", root.transform, _materials.CrystalViolet);
            crown.transform.localPosition = new Vector3(0f, 2.03f, 0f);
            crown.transform.localScale = new Vector3(0.48f, 0.72f, 0.48f);
            crown.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            return root;
        }

        private GameObject CreateSouqAwning()
        {
            var root = CreateHazardRoot(new Vector3(2.3f, 0.55f, 0.9f), new Vector3(0f, 1.65f, 0f));
            root.name = "LowAetherGate";
            var beam = CreatePrimitive(PrimitiveType.Cube, "AetherGateBeam", root.transform, _materials.RuneViolet);
            beam.transform.localPosition = new Vector3(0f, 1.65f, 0f);
            beam.transform.localScale = new Vector3(2.3f, 0.55f, 0.9f);
            for (var rune = -2; rune <= 2; rune++)
            {
                var glyph = CreatePrimitive(PrimitiveType.Cube, "GateRune", root.transform, rune % 2 == 0 ? _materials.RuneCyan : _materials.RuneRose);
                glyph.transform.localPosition = new Vector3(rune * 0.44f, 1.65f, -0.47f);
                glyph.transform.localScale = new Vector3(0.13f, 0.42f, 0.035f);
                glyph.transform.localRotation = Quaternion.Euler(0f, 0f, rune * 12f);
            }
            for (var side = -1; side <= 1; side += 2)
            {
                var post = CreatePrimitive(PrimitiveType.Cube, "GatePost", root.transform, _materials.TempleStone);
                post.transform.localPosition = new Vector3(side * 1.35f, 0.82f, 0f);
                post.transform.localScale = new Vector3(0.18f, 1.65f, 0.18f);
                var cap = CreatePrimitive(PrimitiveType.Sphere, "GatePostLight", root.transform, side < 0 ? _materials.RuneCyan : _materials.RuneRose);
                cap.transform.localPosition = new Vector3(side * 1.35f, 1.70f, 0f);
                cap.transform.localScale = Vector3.one * 0.28f;
            }

            return root;
        }

        private GameObject CreateHazardRoot(Vector3 colliderSize, Vector3 colliderCenter)
        {
            var root = new GameObject("Hazard");
            root.AddComponent<Hazard>();
            var collider = root.AddComponent<BoxCollider>();
            collider.size = colliderSize;
            collider.center = colliderCenter;
            return root;
        }

        private static GameObject CreateRootWithTrigger<T>(Vector3 colliderSize) where T : Component
        {
            var root = new GameObject(typeof(T).Name);
            root.AddComponent<T>();
            var collider = root.AddComponent<BoxCollider>();
            collider.center = Vector3.zero;
            collider.size = colliderSize;
            collider.isTrigger = true;
            return root;
        }

        private static void DeactivateUntilSegmentRecycle(GameObject item)
        {
            item.SetActive(false);
        }

        private void AddAccentStripes(Transform parent, int count, float centerY, float frontZ, float height)
        {
            for (var i = 0; i < count; i++)
            {
                var stripe = CreatePrimitive(PrimitiveType.Cube, "Accent", parent, i % 2 == 0 ? _materials.CurbBlack : _materials.White);
                stripe.transform.localPosition = new Vector3((i - (count - 1) * 0.5f) * 0.65f, centerY, frontZ);
                stripe.transform.localScale = new Vector3(0.14f, height, 0.03f);
                stripe.transform.localRotation = Quaternion.Euler(0f, 0f, -25f);
            }
        }

        private static GameObject CreatePrimitive(PrimitiveType type, string name, Transform parent, Material material)
        {
            var primitive = GameObject.CreatePrimitive(type);
            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            var collider = primitive.GetComponent<Collider>();
            if (collider != null)
            {
                UnityEngine.Object.Destroy(collider);
            }

            primitive.GetComponent<Renderer>().sharedMaterial = material;
            return primitive;
        }
    }
}
