using System.Collections.Generic;
using UnityEngine;

namespace DesertDash.World
{
    public sealed class RuntimeMaterialLibrary
    {
        private readonly Dictionary<string, Material> _materials = new Dictionary<string, Material>();

        public Material Road => Get("MoonstoneCauseway", new Color(0.105f, 0.12f, 0.20f), false, false, 0.34f);
        public Material Lane => Get("LaneRune", new Color(0.10f, 0.88f, 1.00f), true, false, 0.70f);
        public Material Sand => Get("VoidEarth", new Color(0.13f, 0.10f, 0.20f));
        public Material Building => Get("TempleStone", new Color(0.32f, 0.30f, 0.47f));
        public Material BuildingAccent => Get("TempleHighlight", new Color(0.50f, 0.44f, 0.68f));
        public Material Player => Get("ArcaneGreen", new Color(0.04f, 0.66f, 0.40f), true);
        public Material PlayerAccent => Get("ArcaneRose", new Color(0.88f, 0.11f, 0.34f), true);
        public Material Skin => Get("Skin", new Color(0.72f, 0.43f, 0.28f));
        public Material Hair => Get("Hair", new Color(0.055f, 0.035f, 0.028f));
        public Material Jacket => Get("Jacket", new Color(0.56f, 0.055f, 0.075f));
        public Material Pants => Get("Pants", new Color(0.055f, 0.06f, 0.065f));
        public Material Shoes => Get("Shoes", new Color(0.92f, 0.90f, 0.82f));
        public Material White => Get("White", new Color(0.92f, 0.96f, 1f));
        public Material Dark => Get("Dark", new Color(0.035f, 0.035f, 0.032f));
        public Material Magnet => Get("Magnet", new Color(0.78f, 0.14f, 0.22f), true);
        public Material ScoreBoost => Get("ScoreBoost", new Color(0.10f, 0.48f, 0.30f), true);
        public Material Palm => Get("OliveLeaf", new Color(0.20f, 0.38f, 0.20f));
        public Material Wood => Get("OldWood", new Color(0.30f, 0.17f, 0.09f));
        public Material Dust => Get("Stardust", new Color(0.36f, 0.80f, 1.00f, 0.62f), true, true);
        public Material Coin => Get("StarCoin", new Color(1f, 0.74f, 0.12f), true, false, 0.78f, 0.28f);
        public Material Barrier => Get("PrismaticBarrier", new Color(0.54f, 0.18f, 0.94f), true, false, 0.55f);
        public Material Shield => Get("AetherShield", new Color(0.08f, 0.72f, 0.94f), true, true);

        public Material VoidStone => Get("VoidStone", new Color(0.075f, 0.065f, 0.14f), false, false, 0.18f);
        public Material PathStone => Get("MoonPathStone", new Color(0.31f, 0.32f, 0.48f), false, false, 0.42f);
        public Material RuneCyan => Get("RuneCyan", new Color(0.08f, 0.88f, 1.00f), true, false, 0.76f);
        public Material RuneViolet => Get("RuneViolet", new Color(0.52f, 0.20f, 1.00f), true, false, 0.72f);
        public Material RuneRose => Get("RuneRose", new Color(1.00f, 0.18f, 0.58f), true, false, 0.68f);
        public Material IslandRock => Get("FloatingIslandRock", new Color(0.14f, 0.12f, 0.23f), false, false, 0.12f);
        public Material IslandGrass => Get("MoonMeadow", new Color(0.14f, 0.46f, 0.43f), false, false, 0.24f);
        public Material CrystalCyan => Get("CrystalCyan", new Color(0.08f, 0.86f, 1.00f), true, false, 0.88f, 0.12f);
        public Material CrystalViolet => Get("CrystalViolet", new Color(0.58f, 0.18f, 1.00f), true, false, 0.86f, 0.10f);
        public Material CrystalRose => Get("CrystalRose", new Color(1.00f, 0.16f, 0.56f), true, false, 0.84f, 0.08f);
        public Material MoonBark => Get("MoonBark", new Color(0.62f, 0.66f, 0.78f), false, false, 0.36f, 0.14f);
        public Material MoonLeaves => Get("MoonLeaves", new Color(0.14f, 0.82f, 0.86f), true, false, 0.48f);
        public Material MoonLeavesViolet => Get("MoonLeavesViolet", new Color(0.48f, 0.22f, 0.88f), true, false, 0.44f);
        public Material MushroomStem => Get("PearlMushroomStem", new Color(0.78f, 0.76f, 0.92f), false, false, 0.32f);
        public Material MushroomCap => Get("StarMushroomCap", new Color(0.86f, 0.16f, 0.66f), true, false, 0.62f);
        public Material PortalCore => Get("PortalVeil", new Color(0.14f, 0.64f, 1.00f, 0.34f), true, true, 0.90f);
        public Material MistGlow => Get("AetherMist", new Color(0.24f, 0.72f, 1.00f, 0.20f), true, true, 0.76f);
        public Material StarGold => Get("StarGold", new Color(1.00f, 0.72f, 0.10f), true, false, 0.88f, 0.22f);
        public Material TempleStone => Get("AncientSkyTempleStone", new Color(0.36f, 0.32f, 0.50f), false, false, 0.24f);

        public Material LimestoneLight => Get("LimestoneLight", new Color(0.88f, 0.82f, 0.70f));
        public Material LimestoneMid => Get("LimestoneMid", new Color(0.69f, 0.61f, 0.49f));
        public Material LimestoneShadow => Get("LimestoneShadow", new Color(0.45f, 0.40f, 0.34f));
        public Material WindowGlass => Get("WindowGlass", new Color(0.17f, 0.28f, 0.31f));
        public Material BalconyIron => Get("BalconyIron", new Color(0.10f, 0.12f, 0.11f));
        public Material CurbYellow => Get("CurbYellow", new Color(0.94f, 0.62f, 0.05f));
        public Material CurbBlack => Get("CurbBlack", new Color(0.055f, 0.055f, 0.05f));
        public Material ShopRed => Get("ShopRed", new Color(0.66f, 0.08f, 0.10f));
        public Material ShopGreen => Get("ShopGreen", new Color(0.05f, 0.40f, 0.23f));
        public Material ShopBlue => Get("ShopBlue", new Color(0.08f, 0.32f, 0.47f));
        public Material AwningCream => Get("AwningCream", new Color(0.92f, 0.84f, 0.67f));
        public Material TaxiYellow => Get("TaxiYellow", new Color(0.96f, 0.68f, 0.04f));
        public Material BusGreen => Get("BusGreen", new Color(0.05f, 0.38f, 0.25f));
        public Material BusCream => Get("BusCream", new Color(0.86f, 0.82f, 0.70f));
        public Material WaterBlue => Get("WaterBlue", new Color(0.08f, 0.44f, 0.58f), true, true);
        public Material StairCoral => Get("StairCoral", new Color(0.80f, 0.27f, 0.20f));
        public Material StairTeal => Get("StairTeal", new Color(0.04f, 0.45f, 0.43f));
        public Material StairMustard => Get("StairMustard", new Color(0.88f, 0.59f, 0.08f));
        public Material TheatreStone => Get("RomanTheatreStone", new Color(0.78f, 0.69f, 0.56f), false, false, 0.18f);
        public Material TheatreHighlight => Get("RomanTheatreHighlight", new Color(0.91f, 0.84f, 0.70f), false, false, 0.16f);
        public Material TheatreShadow => Get("RomanTheatreShadow", new Color(0.40f, 0.34f, 0.29f), false, false, 0.12f);
        public Material PlazaPaving => Get("HashemitePlazaPaving", new Color(0.67f, 0.61f, 0.52f), false, false, 0.16f);
        public Material PlazaPavingLight => Get("HashemitePlazaPavingLight", new Color(0.82f, 0.76f, 0.65f), false, false, 0.18f);
        public Material PlazaGarden => Get("HashemitePlazaGarden", new Color(0.16f, 0.33f, 0.18f), false, false, 0.10f);

        public Material HeroSkin => Get("HeroSkin", new Color(0.64f, 0.38f, 0.24f), false, false, 0.30f);
        public Material HeroSkinLight => Get("HeroSkinLight", new Color(0.76f, 0.49f, 0.31f), false, false, 0.34f);
        public Material HeroHair => Get("HeroHair", new Color(0.045f, 0.025f, 0.018f), false, false, 0.16f);
        public Material HeroBeard => Get("HeroBeard", new Color(0.075f, 0.042f, 0.027f), false, false, 0.18f);
        public Material HeroEyeBrown => Get("HeroEyeBrown", new Color(0.25f, 0.10f, 0.035f), false, false, 0.48f);
        public Material HeroLip => Get("HeroLip", new Color(0.42f, 0.12f, 0.10f), false, false, 0.35f);
        public Material HeroJacket => Get("HeroJacket", new Color(0.13f, 0.20f, 0.15f), false, false, 0.26f);
        public Material HeroJacketLight => Get("HeroJacketLight", new Color(0.22f, 0.31f, 0.22f), false, false, 0.30f);
        public Material HeroShirt => Get("HeroShirt", new Color(0.92f, 0.89f, 0.80f), false, false, 0.32f);
        public Material HeroDenim => Get("HeroDenim", new Color(0.07f, 0.14f, 0.22f), false, false, 0.22f);
        public Material HeroDenimLight => Get("HeroDenimLight", new Color(0.12f, 0.23f, 0.34f), false, false, 0.24f);
        public Material HeroSneaker => Get("HeroSneaker", new Color(0.82f, 0.11f, 0.13f), false, false, 0.38f);
        public Material HeroSole => Get("HeroSole", new Color(0.92f, 0.91f, 0.86f), false, false, 0.28f);
        public Material KeffiyehWhite => Get("KeffiyehWhite", new Color(0.94f, 0.91f, 0.84f), false, false, 0.20f);
        public Material KeffiyehRed => Get("KeffiyehRed", new Color(0.62f, 0.035f, 0.045f), false, false, 0.18f);
        public Material LeatherBrown => Get("LeatherBrown", new Color(0.24f, 0.095f, 0.035f), false, false, 0.36f);
        public Material LeatherLight => Get("LeatherLight", new Color(0.46f, 0.22f, 0.08f), false, false, 0.40f);
        public Material WatchMetal => Get("WatchMetal", new Color(0.34f, 0.38f, 0.39f), false, false, 0.72f, 0.65f);
        public Material WatchGlass => Get("WatchGlass", new Color(0.025f, 0.08f, 0.09f), true, false, 0.90f, 0.20f);
        public Material JewelrySilver => Get("JewelrySilver", new Color(0.72f, 0.75f, 0.76f), false, false, 0.80f, 0.75f);
        public Material JordanGreen => Get("JordanGreen", new Color(0.02f, 0.34f, 0.18f), false, false, 0.25f);
        public Material JordanRed => Get("JordanRed", new Color(0.66f, 0.025f, 0.04f), false, false, 0.24f);

        public Material PlumberSkin => GetToon("PlumberSkin", new Color(0.96f, 0.57f, 0.38f), new Color(0.50f, 0.20f, 0.20f));
        public Material PlumberSkinLight => GetToon("PlumberSkinLight", new Color(1.00f, 0.70f, 0.50f), new Color(0.56f, 0.24f, 0.22f));
        public Material PlumberRed => GetToon("PlumberRed", new Color(0.92f, 0.055f, 0.075f), new Color(0.31f, 0.055f, 0.11f));
        public Material PlumberRedDark => GetToon("PlumberRedDark", new Color(0.55f, 0.025f, 0.045f), new Color(0.20f, 0.025f, 0.07f));
        public Material PlumberBlue => GetToon("PlumberBlue", new Color(0.045f, 0.32f, 0.86f), new Color(0.025f, 0.095f, 0.32f));
        public Material PlumberBlueDark => GetToon("PlumberBlueDark", new Color(0.025f, 0.13f, 0.48f), new Color(0.015f, 0.045f, 0.18f));
        public Material PlumberGlove => GetToon("PlumberGlove", new Color(0.98f, 0.96f, 0.84f), new Color(0.37f, 0.42f, 0.52f));
        public Material PlumberBoot => GetToon("PlumberBoot", new Color(0.27f, 0.075f, 0.025f), new Color(0.11f, 0.025f, 0.02f));
        public Material PlumberBootLight => GetToon("PlumberBootLight", new Color(0.56f, 0.20f, 0.055f), new Color(0.19f, 0.045f, 0.025f));
        public Material PlumberGold => GetToon("PlumberGold", new Color(1.00f, 0.66f, 0.025f), new Color(0.50f, 0.19f, 0.015f), new Color(1.00f, 0.84f, 0.28f));
        public Material PlumberHair => GetToon("PlumberHair", new Color(0.12f, 0.035f, 0.018f), new Color(0.035f, 0.012f, 0.02f));
        public Material PlumberEyeBlue => GetToon("PlumberEyeBlue", new Color(0.06f, 0.48f, 0.86f), new Color(0.015f, 0.11f, 0.31f));

        private Material Get(string name, Color color, bool emission = false, bool transparent = false, float smoothness = 0.22f, float metallic = 0f)
        {
            if (_materials.TryGetValue(name, out var material))
            {
                return material;
            }

            var shader = Shader.Find("Standard") ?? Shader.Find("Sprites/Default");
            material = new Material(shader) { name = name, color = color };
            material.enableInstancing = true;
            if (material.HasProperty("_Glossiness"))
            {
                material.SetFloat("_Glossiness", Mathf.Clamp01(smoothness));
            }

            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", Mathf.Clamp01(metallic));
            }
            if (emission)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 0.7f);
            }

            if (transparent)
            {
                material.SetFloat("_Mode", 3f);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.renderQueue = 3000;
                material.color = new Color(color.r, color.g, color.b, 0.28f);
            }

            _materials[name] = material;
            return material;
        }

        private Material GetToon(string name, Color color, Color shadowColor, Color? rimColor = null)
        {
            if (_materials.TryGetValue(name, out var material))
            {
                return material;
            }

            var shader = Resources.Load<Shader>("Shaders/JordanianHeroToon") ?? Shader.Find("DesertDash/Lizard Runner Toon");
            if (shader == null)
            {
                return Get(name, color, false, false, 0.12f);
            }

            material = new Material(shader) { name = name };
            material.enableInstancing = true;
            material.SetColor("_BaseColor", color);
            material.SetColor("_ShadowColor", shadowColor);
            material.SetColor("_RimColor", rimColor ?? new Color(0.35f, 0.74f, 1.00f));
            material.SetFloat("_Saturation", 1.25f);
            material.SetFloat("_Contrast", 1.10f);
            material.SetFloat("_Brightness", 1.05f);
            material.SetFloat("_LightSteps", 3f);
            material.SetFloat("_RimPower", 4.2f);
            material.SetFloat("_RimStrength", 0.16f);
            _materials[name] = material;
            return material;
        }
    }
}
