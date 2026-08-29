using UnityEngine;
using DesertDash.Core;

namespace DesertDash.World
{
    public sealed class AmmanThemeBuilder
    {
        private static readonly string[] ShopNames = { "AL BALAD", "SOUQ", "AMMAN CAFE", "BOOKS", "SPICES", "KNAFEH" };

        private readonly Transform _root;
        private readonly RunnerConfig _config;
        private readonly RuntimeMaterialLibrary _materials;
        private readonly int _index;
        private readonly System.Random _random;

        public AmmanThemeBuilder(Transform root, RunnerConfig config, RuntimeMaterialLibrary materials, int index)
        {
            _root = root;
            _config = config;
            _materials = materials;
            _index = index;
            _random = new System.Random(index * 3917 + 111);
        }

        public void Build()
        {
            var landmarkVariant = _index % 6;
            BuildShopRow(-1);
            if (landmarkVariant != 0)
            {
                BuildShopRow(1);
            }
            BuildStreetFurniture(-1);
            BuildStreetFurniture(1);

            if (_index % 2 == 0)
            {
                BuildStringLights(11f);
            }

            if (_index % 3 == 0 && landmarkVariant != 0)
            {
                BuildAlBaladBanner();
            }

            switch (landmarkVariant)
            {
                case 0:
                    BuildRomanTheatre();
                    break;
                case 1:
                    BuildPaintedStairs();
                    break;
                case 2:
                    BuildHashemitePlaza();
                    break;
                case 3:
                    BuildCitadelColumns();
                    break;
                case 4:
                    BuildPaintedStairs();
                    break;
                default:
                    BuildHashemitePlaza();
                    break;
            }
        }

        private void BuildShopRow(int side)
        {
            for (var shop = 0; shop < 3; shop++)
            {
                var width = 3.2f + (float)_random.NextDouble() * 0.75f;
                var depth = 5.2f + (float)_random.NextDouble() * 1.4f;
                var height = 3.4f + (float)_random.NextDouble() * 2.8f;
                var z = 4.4f + shop * 10.2f + (float)_random.NextDouble() * 1.1f;
                var x = side * (7.45f + (float)_random.NextDouble() * 0.45f);

                var building = new GameObject("AmmanLimestoneShop").transform;
                building.SetParent(_root, false);
                building.localPosition = new Vector3(x, 0f, z);

                var stone = shop % 2 == 0 ? _materials.LimestoneLight : _materials.Building;
                CreateBlock("StoneFacade", new Vector3(0f, height * 0.5f, 0f), new Vector3(width, height, depth), stone, building);
                CreateStoneCourses(building, side, width, height, depth);
                BuildShopFront(building, side, width, height, shop);
                BuildUpperWindows(building, side, width, height);
                BuildBalcony(building, side, width, height);
                BuildRoofDetails(building, side, width, height, depth, shop);

                var hillTier = shop % 2 + 1;
                BuildHillsideHome(side, z + 1.2f, hillTier, shop);
            }
        }

        private void CreateStoneCourses(Transform building, int side, float width, float height, float depth)
        {
            for (var course = 1; course < Mathf.FloorToInt(height / 0.55f); course++)
            {
                var y = course * 0.55f;
                CreateBlock("StoneCourse", new Vector3(-side * (width * 0.5f + 0.012f), y, 0f), new Vector3(0.025f, 0.022f, depth * 0.92f), _materials.LimestoneShadow, building);
            }
        }

        private void BuildShopFront(Transform building, int side, float width, float height, int shop)
        {
            var roadFace = -side * (width * 0.5f + 0.025f);
            var accent = shop % 3 == 0 ? _materials.ShopRed : shop % 3 == 1 ? _materials.ShopGreen : _materials.ShopBlue;
            CreateBlock("ShopOpening", new Vector3(roadFace, 0.90f, 0f), new Vector3(0.06f, 1.65f, 2.25f), _materials.Dark, building);
            CreateBlock("ShopCounter", new Vector3(roadFace - side * 0.10f, 0.48f, 0f), new Vector3(0.22f, 0.78f, 2.30f), _materials.Wood, building);
            CreateBlock("SignBoard", new Vector3(roadFace - side * 0.08f, 2.05f, 0f), new Vector3(0.16f, 0.58f, 2.52f), accent, building);
            CreateWorldText(ShopNames[(_index * 3 + shop) % ShopNames.Length], building, new Vector3(roadFace - side * 0.18f, 2.05f, 0f), side, Color.white, 0.12f);

            var awning = CreateBlock("StripedAwning", new Vector3(roadFace - side * 0.52f, 1.67f, 0f), new Vector3(1.02f, 0.10f, 2.45f), _materials.AwningCream, building);
            awning.transform.localRotation = Quaternion.Euler(0f, 0f, side * 12f);
            for (var stripe = -2; stripe <= 2; stripe++)
            {
                CreateBlock("AwningStripe", new Vector3(roadFace - side * 0.55f, 1.67f, stripe * 0.47f), new Vector3(1.05f, 0.04f, 0.19f), accent, building).transform.localRotation = Quaternion.Euler(0f, 0f, side * 12f);
            }
        }

        private void BuildUpperWindows(Transform building, int side, float width, float height)
        {
            var roadFace = -side * (width * 0.5f + 0.03f);
            for (var floorY = 2.85f; floorY < height - 0.35f; floorY += 1.35f)
            {
                for (var window = -1; window <= 1; window += 2)
                {
                    CreateBlock("Window", new Vector3(roadFace, floorY, window * 0.72f), new Vector3(0.07f, 0.70f, 0.82f), _materials.WindowGlass, building);
                    CreateBlock("WindowTop", new Vector3(roadFace - side * 0.015f, floorY + 0.44f, window * 0.72f), new Vector3(0.11f, 0.10f, 1.00f), _materials.LimestoneShadow, building);
                }
            }
        }

        private void BuildBalcony(Transform building, int side, float width, float height)
        {
            if (height < 4.3f)
            {
                return;
            }

            var roadFace = -side * (width * 0.5f + 0.36f);
            CreateBlock("BalconyFloor", new Vector3(roadFace, 3.55f, 0f), new Vector3(0.72f, 0.12f, 2.65f), _materials.LimestoneMid, building);
            CreateBlock("BalconyRail", new Vector3(roadFace - side * 0.32f, 3.94f, 0f), new Vector3(0.07f, 0.68f, 2.65f), _materials.BalconyIron, building);
            for (var rail = -2; rail <= 2; rail++)
            {
                CreateBlock("RailPost", new Vector3(roadFace - side * 0.32f, 3.94f, rail * 0.52f), new Vector3(0.10f, 0.78f, 0.07f), _materials.BalconyIron, building);
            }
        }

        private void BuildRoofDetails(Transform building, int side, float width, float height, float depth, int shop)
        {
            CreateBlock("Parapet", new Vector3(0f, height + 0.20f, 0f), new Vector3(width + 0.12f, 0.38f, depth + 0.12f), _materials.LimestoneMid, building);
            CreateBlock("RoofInset", new Vector3(0f, height + 0.25f, 0f), new Vector3(width * 0.84f, 0.30f, depth * 0.84f), _materials.Dark, building);

            var tank = CreatePrimitive(PrimitiveType.Cylinder, "WaterTank", new Vector3(side * width * 0.18f, height + 0.73f, depth * 0.20f), new Vector3(0.42f, 0.48f, 0.42f), _materials.CurbBlack, building);
            tank.transform.localRotation = Quaternion.Euler(0f, shop * 20f, 90f);

            var dishRoot = new GameObject("SatelliteDish").transform;
            dishRoot.SetParent(building, false);
            dishRoot.localPosition = new Vector3(-side * width * 0.18f, height + 0.55f, -depth * 0.22f);
            var dish = CreatePrimitive(PrimitiveType.Sphere, "Dish", Vector3.zero, new Vector3(0.54f, 0.10f, 0.54f), _materials.LimestoneShadow, dishRoot);
            dish.transform.localRotation = Quaternion.Euler(25f, side * 30f, 0f);
            CreatePrimitive(PrimitiveType.Cylinder, "DishPole", new Vector3(0f, -0.26f, 0f), new Vector3(0.05f, 0.28f, 0.05f), _materials.BalconyIron, dishRoot);
        }

        private void BuildHillsideHome(int side, float z, int tier, int variant)
        {
            var width = 3.7f;
            var height = 2.6f + variant * 0.45f;
            var x = side * (10.5f + tier * 2.2f);
            var y = tier * 1.7f;
            var home = new GameObject("HillsideHome").transform;
            home.SetParent(_root, false);
            home.localPosition = new Vector3(x, y, z);
            CreateBlock("TerracedStoneHome", new Vector3(0f, height * 0.5f, 0f), new Vector3(width, height, 5.4f), variant % 2 == 0 ? _materials.Building : _materials.LimestoneLight, home);
            CreateBlock("HomeWindow", new Vector3(-side * (width * 0.5f + 0.02f), 1.55f, 0f), new Vector3(0.05f, 0.70f, 1.10f), _materials.WindowGlass, home);
            CreateBlock("RoofLine", new Vector3(0f, height + 0.12f, 0f), new Vector3(width + 0.16f, 0.24f, 5.55f), _materials.LimestoneMid, home);
        }

        private void BuildStreetFurniture(int side)
        {
            for (var item = 0; item < 2; item++)
            {
                var z = 5.5f + item * 15.5f + (float)_random.NextDouble() * 3f;
                var x = side * 5.75f;
                var lamp = new GameObject("DowntownLamp").transform;
                lamp.SetParent(_root, false);
                lamp.localPosition = new Vector3(x, 0f, z);
                CreatePrimitive(PrimitiveType.Cylinder, "LampPost", new Vector3(0f, 1.75f, 0f), new Vector3(0.085f, 1.75f, 0.085f), _materials.BalconyIron, lamp);
                CreateBlock("LampArm", new Vector3(-side * 0.34f, 3.40f, 0f), new Vector3(0.68f, 0.07f, 0.07f), _materials.BalconyIron, lamp);
                CreatePrimitive(PrimitiveType.Sphere, "WarmLamp", new Vector3(-side * 0.66f, 3.28f, 0f), Vector3.one * 0.24f, _materials.Coin, lamp);
            }
        }

        private void BuildStringLights(float z)
        {
            CreateBlock("LightCable", new Vector3(0f, 4.20f, z), new Vector3(10.8f, 0.025f, 0.025f), _materials.BalconyIron, _root);
            for (var bulb = -5; bulb <= 5; bulb++)
            {
                var drop = 0.20f + Mathf.Abs(bulb) * 0.018f;
                CreatePrimitive(PrimitiveType.Sphere, "StringBulb", new Vector3(bulb * 0.92f, 4.20f - drop, z), Vector3.one * 0.13f, _materials.Coin, _root);
            }
        }

        private void BuildAlBaladBanner()
        {
            var z = _config.segmentLength - 2.2f;
            CreateBlock("BannerPostLeft", new Vector3(-4.75f, 2.20f, z), new Vector3(0.16f, 4.4f, 0.16f), _materials.BalconyIron, _root);
            CreateBlock("BannerPostRight", new Vector3(4.75f, 2.20f, z), new Vector3(0.16f, 4.4f, 0.16f), _materials.BalconyIron, _root);
            CreateBlock("BannerBoard", new Vector3(0f, 4.15f, z), new Vector3(4.0f, 0.72f, 0.12f), _materials.ShopGreen, _root);
            CreateWorldText("AL BALAD", _root, new Vector3(0f, 4.15f, z - 0.075f), 0, Color.white, 0.14f);
        }

        private void BuildRomanTheatre()
        {
            var district = new GameObject("RomanTheatreAndHashemitePlazaDistrict").transform;
            district.SetParent(_root, false);
            district.localPosition = new Vector3(14.6f, 0.04f, 15f);
            district.localRotation = Quaternion.Euler(0f, 90f, 0f);

            BuildHashemiteForecourt(district);

            var theatre = new GameObject("RomanTheatreLandmark").transform;
            theatre.SetParent(district, false);
            CreateBlock("JabalJoufahTheatreBase", new Vector3(0f, -1.15f, 3.25f), new Vector3(14.8f, 2.3f, 11.2f), _materials.Sand, theatre);
            CreatePrimitive(PrimitiveType.Cylinder, "OrchestraFloor", new Vector3(0f, 0.12f, -0.02f), new Vector3(1.92f, 0.10f, 1.92f), _materials.TheatreHighlight, theatre);
            CreateBlock("StageDeck", new Vector3(0f, 0.24f, -2.05f), new Vector3(8.9f, 0.32f, 2.20f), _materials.TheatreStone, theatre);
            CreateBlock("StageFront", new Vector3(0f, 0.48f, -2.84f), new Vector3(9.25f, 0.58f, 0.38f), _materials.TheatreShadow, theatre);

            const int tierCount = 11;
            const int segmentCount = 15;
            const float minimumAngle = -82f;
            const float maximumAngle = 82f;
            var angleStep = (maximumAngle - minimumAngle) / (segmentCount - 1f);
            for (var tier = 0; tier < tierCount; tier++)
            {
                var horizontalWalkway = tier >= 4 ? 0.34f : 0f;
                horizontalWalkway += tier >= 8 ? 0.34f : 0f;
                var radius = 1.80f + tier * 0.50f + horizontalWalkway;
                var tierHeight = 0.25f + tier * 0.28f;
                var blockWidth = radius * angleStep * Mathf.Deg2Rad * 1.12f;
                var stone = tier % 3 == 0 ? _materials.TheatreHighlight : _materials.TheatreStone;

                for (var segment = 0; segment < segmentCount; segment++)
                {
                    var angle = minimumAngle + segment * angleStep;
                    var radians = angle * Mathf.Deg2Rad;
                    var seat = CreateBlock("CurvedStoneSeatingTier", new Vector3(Mathf.Sin(radians) * radius, tierHeight, Mathf.Cos(radians) * radius), new Vector3(blockWidth, 0.20f, 0.52f), stone, theatre);
                    seat.transform.localRotation = Quaternion.Euler(0f, -angle, 0f);
                }

                BuildTheatreAisleStep(theatre, radius, tierHeight + 0.08f, -43f);
                BuildTheatreAisleStep(theatre, radius, tierHeight + 0.08f, 0f);
                BuildTheatreAisleStep(theatre, radius, tierHeight + 0.08f, 43f);
            }

            BuildTheatreStageFacade(theatre);
            BuildTheatreRetainingWalls(theatre);
            BuildOdeon(district);
            BuildRomanDistrictBackdrop(district);
        }

        private void BuildHashemiteForecourt(Transform district)
        {
            CreateBlock("HashemitePlazaFloor", new Vector3(0f, 0f, -6.05f), new Vector3(12.2f, 0.14f, 6.55f), _materials.PlazaPaving, district);
            for (var stripe = -5; stripe <= 5; stripe++)
            {
                CreateBlock("PlazaLongPaverLine", new Vector3(stripe * 1.08f, 0.078f, -6.05f), new Vector3(0.035f, 0.012f, 6.30f), _materials.PlazaPavingLight, district);
            }

            for (var stripe = 0; stripe < 6; stripe++)
            {
                CreateBlock("PlazaCrossPaverLine", new Vector3(0f, 0.080f, -8.85f + stripe * 1.12f), new Vector3(11.75f, 0.012f, 0.035f), _materials.PlazaPavingLight, district);
            }

            BuildPlazaFountain(district, new Vector3(-3.65f, 0f, -6.35f));
            BuildPlazaFountain(district, new Vector3(3.65f, 0f, -6.35f));
            BuildPlazaPalm(district, new Vector3(-5.25f, 0.08f, -8.25f));
            BuildPlazaPalm(district, new Vector3(5.25f, 0.08f, -8.25f));
            BuildPlazaPalm(district, new Vector3(-5.25f, 0.08f, -3.95f));
            BuildPlazaPalm(district, new Vector3(5.25f, 0.08f, -3.95f));
            BuildPlazaBench(district, new Vector3(-2.20f, 0.13f, -8.45f), 0f);
            BuildPlazaBench(district, new Vector3(2.20f, 0.13f, -8.45f), 0f);
            BuildPlazaBench(district, new Vector3(-5.35f, 0.13f, -5.85f), 90f);
            BuildPlazaBench(district, new Vector3(5.35f, 0.13f, -5.85f), 90f);

            var gardenLeft = CreatePrimitive(PrimitiveType.Cylinder, "PlazaGarden", new Vector3(-5.20f, 0.13f, -6.30f), new Vector3(0.78f, 0.12f, 0.78f), _materials.PlazaGarden, district);
            var gardenRight = CreatePrimitive(PrimitiveType.Cylinder, "PlazaGarden", new Vector3(5.20f, 0.13f, -6.30f), new Vector3(0.78f, 0.12f, 0.78f), _materials.PlazaGarden, district);
            gardenLeft.transform.localRotation = Quaternion.Euler(0f, 22f, 0f);
            gardenRight.transform.localRotation = Quaternion.Euler(0f, -22f, 0f);
        }

        private void BuildPlazaFountain(Transform parent, Vector3 position)
        {
            CreatePrimitive(PrimitiveType.Cylinder, "PlazaFountainBasin", position + Vector3.up * 0.14f, new Vector3(0.72f, 0.14f, 0.72f), _materials.TheatreStone, parent);
            CreatePrimitive(PrimitiveType.Cylinder, "PlazaFountainWater", position + Vector3.up * 0.29f, new Vector3(0.59f, 0.035f, 0.59f), _materials.WaterBlue, parent);
            CreatePrimitive(PrimitiveType.Cylinder, "PlazaWaterJet", position + Vector3.up * 0.62f, new Vector3(0.035f, 0.31f, 0.035f), _materials.WaterBlue, parent);
        }

        private void BuildPlazaBench(Transform parent, Vector3 position, float yaw)
        {
            var bench = new GameObject("HashemitePlazaBench").transform;
            bench.SetParent(parent, false);
            bench.localPosition = position;
            bench.localRotation = Quaternion.Euler(0f, yaw, 0f);
            CreateBlock("BenchSeat", new Vector3(0f, 0.34f, 0f), new Vector3(1.55f, 0.13f, 0.48f), _materials.Wood, bench);
            CreateBlock("BenchBack", new Vector3(0f, 0.68f, 0.20f), new Vector3(1.55f, 0.58f, 0.11f), _materials.Wood, bench);
            CreateBlock("BenchLegLeft", new Vector3(-0.57f, 0.16f, 0f), new Vector3(0.10f, 0.32f, 0.38f), _materials.BalconyIron, bench);
            CreateBlock("BenchLegRight", new Vector3(0.57f, 0.16f, 0f), new Vector3(0.10f, 0.32f, 0.38f), _materials.BalconyIron, bench);
        }

        private void BuildTheatreAisleStep(Transform theatre, float radius, float height, float angle)
        {
            var radians = angle * Mathf.Deg2Rad;
            var aisle = CreateBlock("RadialAisleStep", new Vector3(Mathf.Sin(radians) * radius, height, Mathf.Cos(radians) * radius), new Vector3(0.24f, 0.10f, 0.58f), _materials.TheatreShadow, theatre);
            aisle.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }

        private void BuildTheatreStageFacade(Transform theatre)
        {
            CreateBlock("ScaenaeFronsWall", new Vector3(0f, 1.55f, -3.24f), new Vector3(9.70f, 2.70f, 0.55f), _materials.TheatreStone, theatre);
            CreateBlock("StageFacadeBase", new Vector3(0f, 0.34f, -3.58f), new Vector3(10.05f, 0.34f, 0.38f), _materials.TheatreShadow, theatre);

            for (var opening = -2; opening <= 2; opening++)
            {
                var x = opening * 1.65f;
                CreateBlock("StageDoorShadow", new Vector3(x, 1.06f, -3.54f), new Vector3(0.90f, 1.38f, 0.055f), _materials.TheatreShadow, theatre);
                CreatePrimitive(PrimitiveType.Cylinder, "StageColumnLeft", new Vector3(x - 0.55f, 1.20f, -3.62f), new Vector3(0.11f, 0.78f, 0.11f), _materials.TheatreHighlight, theatre);
                CreatePrimitive(PrimitiveType.Cylinder, "StageColumnRight", new Vector3(x + 0.55f, 1.20f, -3.62f), new Vector3(0.11f, 0.78f, 0.11f), _materials.TheatreHighlight, theatre);
                CreateBlock("StageArchLintel", new Vector3(x, 1.97f, -3.62f), new Vector3(1.25f, 0.20f, 0.18f), _materials.TheatreHighlight, theatre);
                CreateBlock("UpperFacadeOpening", new Vector3(x, 2.48f, -3.54f), new Vector3(0.66f, 0.52f, 0.055f), _materials.TheatreShadow, theatre);
            }

            CreateBlock("StageFacadeCornice", new Vector3(0f, 2.92f, -3.33f), new Vector3(10.20f, 0.28f, 0.75f), _materials.TheatreHighlight, theatre);
            CreateBlock("StageFacadeTop", new Vector3(0f, 3.22f, -3.28f), new Vector3(8.65f, 0.34f, 0.62f), _materials.TheatreStone, theatre);
        }

        private void BuildTheatreRetainingWalls(Transform theatre)
        {
            var left = CreateBlock("LeftCaveaWall", new Vector3(-7.15f, 1.44f, 3.05f), new Vector3(0.42f, 2.88f, 8.15f), _materials.TheatreShadow, theatre);
            left.transform.localRotation = Quaternion.Euler(0f, -7f, 0f);
            var right = CreateBlock("RightCaveaWall", new Vector3(7.15f, 1.44f, 3.05f), new Vector3(0.42f, 2.88f, 8.15f), _materials.TheatreShadow, theatre);
            right.transform.localRotation = Quaternion.Euler(0f, 7f, 0f);

            for (var side = -1; side <= 1; side += 2)
            {
                for (var arch = 0; arch < 4; arch++)
                {
                    CreateBlock("SideVault", new Vector3(side * 7.38f, 0.70f + arch * 0.55f, 0.40f + arch * 1.45f), new Vector3(0.10f, 0.55f, 0.82f), _materials.TheatreHighlight, theatre);
                }
            }
        }

        private void BuildOdeon(Transform district)
        {
            var odeon = new GameObject("OdeonTheatreLandmark").transform;
            odeon.SetParent(district, false);
            odeon.localPosition = new Vector3(-7.05f, 0.08f, -5.35f);
            odeon.localRotation = Quaternion.Euler(0f, 78f, 0f);
            CreateBlock("OdeonFloor", new Vector3(0f, 0.08f, -0.65f), new Vector3(4.0f, 0.16f, 2.4f), _materials.TheatreStone, odeon);
            for (var tier = 0; tier < 5; tier++)
            {
                var radius = 0.85f + tier * 0.40f;
                for (var segment = 0; segment < 7; segment++)
                {
                    var angle = -68f + segment * (136f / 6f);
                    var radians = angle * Mathf.Deg2Rad;
                    var seat = CreateBlock("OdeonCurvedTier", new Vector3(Mathf.Sin(radians) * radius, 0.18f + tier * 0.20f, Mathf.Cos(radians) * radius), new Vector3(0.52f, 0.15f, 0.40f), tier % 2 == 0 ? _materials.TheatreHighlight : _materials.TheatreStone, odeon);
                    seat.transform.localRotation = Quaternion.Euler(0f, -angle, 0f);
                }
            }
        }

        private void BuildRomanDistrictBackdrop(Transform district)
        {
            CreateBlock("JabalJoufahSlope", new Vector3(0f, -0.95f, 10.15f), new Vector3(19.0f, 2.2f, 7.5f), _materials.Sand, district);
            for (var row = 0; row < 3; row++)
            {
                for (var column = -3; column <= 3; column++)
                {
                    var width = 1.85f + ((column + row + 8) % 3) * 0.18f;
                    var height = 1.90f + ((column * column + row) % 3) * 0.34f;
                    var home = new GameObject("DenseJabalJoufahHome").transform;
                    home.SetParent(district, false);
                    home.localPosition = new Vector3(column * 2.35f + (row % 2 == 0 ? 0f : 0.82f), row * 1.52f, 8.65f + row * 1.50f);
                    var stone = (column + row) % 2 == 0 ? _materials.LimestoneLight : _materials.Building;
                    CreateBlock("HillsideStoneFacade", new Vector3(0f, height * 0.5f, 0f), new Vector3(width, height, 1.75f), stone, home);
                    CreateBlock("HillsideDarkWindow", new Vector3(0f, height * 0.58f, -0.90f), new Vector3(0.60f, 0.56f, 0.05f), _materials.WindowGlass, home);
                    CreateBlock("HillsideRoofLine", new Vector3(0f, height + 0.10f, 0f), new Vector3(width + 0.14f, 0.20f, 1.88f), _materials.LimestoneMid, home);
                    if ((column + row) % 3 == 0)
                    {
                        CreatePrimitive(PrimitiveType.Cylinder, "BlackRoofTank", new Vector3(0.35f, height + 0.43f, 0f), new Vector3(0.24f, 0.30f, 0.24f), _materials.CurbBlack, home);
                    }
                }
            }
        }

        private void BuildCitadelColumns()
        {
            var citadel = new GameObject("CitadelLandmark").transform;
            citadel.SetParent(_root, false);
            citadel.localPosition = new Vector3(-13f, 2.2f, 15f);
            citadel.localRotation = Quaternion.Euler(0f, 90f, 0f);
            CreateBlock("CitadelHill", new Vector3(0f, -1.15f, 0f), new Vector3(10f, 2.3f, 7f), _materials.Sand, citadel);
            CreateBlock("TempleBase", new Vector3(0f, 0.12f, 0f), new Vector3(7.8f, 0.40f, 3.6f), _materials.LimestoneMid, citadel);
            for (var column = -2; column <= 2; column++)
            {
                var x = column * 1.32f;
                CreatePrimitive(PrimitiveType.Cylinder, "CitadelColumn", new Vector3(x, 1.72f, 0f), new Vector3(0.20f, 1.60f, 0.20f), _materials.LimestoneLight, citadel);
                CreateBlock("ColumnCapital", new Vector3(x, 3.30f, 0f), new Vector3(0.66f, 0.18f, 0.66f), _materials.LimestoneMid, citadel);
            }

            CreateBlock("TempleLintel", new Vector3(0f, 3.56f, 0f), new Vector3(7.1f, 0.34f, 0.72f), _materials.LimestoneLight, citadel);
        }

        private void BuildPaintedStairs()
        {
            var side = _index % 2 == 0 ? 1 : -1;
            var stairs = new GameObject("DowntownPaintedStairs").transform;
            stairs.SetParent(_root, false);
            stairs.localPosition = new Vector3(side * 6.35f, 0f, 16f);
            for (var step = 0; step < 12; step++)
            {
                var material = step % 3 == 0 ? _materials.StairCoral : step % 3 == 1 ? _materials.StairTeal : _materials.StairMustard;
                CreateBlock("PaintedStep", new Vector3(side * step * 0.28f, 0.10f + step * 0.18f, 0f), new Vector3(0.62f, 0.18f, 3.0f), material, stairs);
            }
        }

        private void BuildHashemitePlaza()
        {
            var side = _index % 2 == 0 ? 1 : -1;
            var plaza = new GameObject("HashemitePlazaInspiredSpace").transform;
            plaza.SetParent(_root, false);
            plaza.localPosition = new Vector3(side * 9.8f, 0.04f, 15f);
            CreateBlock("PlazaFloor", Vector3.zero, new Vector3(7.2f, 0.12f, 10.5f), _materials.LimestoneLight, plaza);
            CreatePrimitive(PrimitiveType.Cylinder, "FountainBasin", new Vector3(0f, 0.18f, 0f), new Vector3(1.25f, 0.16f, 1.25f), _materials.LimestoneMid, plaza);
            CreatePrimitive(PrimitiveType.Cylinder, "FountainWater", new Vector3(0f, 0.36f, 0f), new Vector3(1.05f, 0.06f, 1.05f), _materials.WaterBlue, plaza);
            CreatePrimitive(PrimitiveType.Cylinder, "WaterJet", new Vector3(0f, 0.78f, 0f), new Vector3(0.06f, 0.42f, 0.06f), _materials.WaterBlue, plaza);
            BuildPlazaPalm(plaza, new Vector3(-2.35f, 0f, -3.7f));
            BuildPlazaPalm(plaza, new Vector3(2.35f, 0f, 3.7f));
        }

        private void BuildPlazaPalm(Transform parent, Vector3 position)
        {
            var palm = new GameObject("PlazaPalm").transform;
            palm.SetParent(parent, false);
            palm.localPosition = position;
            CreatePrimitive(PrimitiveType.Cylinder, "PalmTrunk", new Vector3(0f, 1.35f, 0f), new Vector3(0.16f, 1.35f, 0.16f), _materials.Wood, palm);
            for (var leaf = 0; leaf < 6; leaf++)
            {
                var frond = CreateBlock("PalmFrond", new Vector3(0f, 2.72f, 0f), new Vector3(0.22f, 0.07f, 1.35f), _materials.Palm, palm);
                frond.transform.localRotation = Quaternion.Euler(18f, leaf * 60f, 0f);
            }
        }

        private static void CreateWorldText(string value, Transform parent, Vector3 position, int side, Color color, float characterSize)
        {
            var textObject = new GameObject("SignText");
            textObject.transform.SetParent(parent, false);
            textObject.transform.localPosition = position;
            textObject.transform.localRotation = side == 0 ? Quaternion.identity : Quaternion.Euler(0f, -side * 90f, 0f);
            var text = textObject.AddComponent<TextMesh>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 64;
            text.characterSize = characterSize;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.color = color;
            var renderer = textObject.GetComponent<MeshRenderer>();
            if (text.font != null)
            {
                renderer.sharedMaterial = text.font.material;
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
            var renderer = primitive.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            var collider = primitive.GetComponent<Collider>();
            if (collider != null)
            {
                Object.Destroy(collider);
            }

            return primitive;
        }
    }
}
