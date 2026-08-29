using DesertDash.World;
using UnityEngine;

namespace DesertDash.Player
{
    /// <summary>
    /// Builds an original platform-game plumber hero. The silhouette uses the
    /// readable, compact proportions of classic platform mascots without using
    /// a protected character, logo or copied mesh.
    /// </summary>
    public sealed class CartoonPlumberHeroBuilder
    {
        private readonly RuntimeMaterialLibrary _materials;

        public CartoonPlumberHeroBuilder(RuntimeMaterialLibrary materials)
        {
            _materials = materials;
        }

        public JordanianHeroRig Build(Transform parent)
        {
            var rig = new JordanianHeroRig();
            rig.Root = CreatePivot("CartoonPlumberRig", parent, Vector3.zero);
            rig.Pelvis = CreatePivot("Pelvis", rig.Root, new Vector3(0f, 0.87f, 0f));
            rig.Spine = CreatePivot("Spine", rig.Pelvis, new Vector3(0f, 0.18f, 0f));
            rig.Chest = CreatePivot("Chest", rig.Spine, new Vector3(0f, 0.25f, 0f));
            rig.Neck = CreatePivot("Neck", rig.Chest, new Vector3(0f, 0.27f, 0f));
            rig.Head = CreatePivot("Head", rig.Neck, new Vector3(0f, 0.21f, 0f));

            BuildBody(rig);
            BuildHead(rig);
            BuildArms(rig);
            BuildLegs(rig);
            return rig;
        }

        private void BuildBody(JordanianHeroRig rig)
        {
            CreateSphere("RoundedHips", rig.Pelvis, new Vector3(0f, 0.02f, 0f), new Vector3(0.72f, 0.38f, 0.50f), _materials.PlumberBlueDark);
            CreateCube("OverallWaist", rig.Pelvis, new Vector3(0f, 0.17f, 0.015f), new Vector3(0.71f, 0.13f, 0.48f), _materials.PlumberBlue);

            CreateSphere("RedShirtBelly", rig.Spine, new Vector3(0f, 0.11f, 0f), new Vector3(0.82f, 0.62f, 0.57f), _materials.PlumberRed);
            CreateSphere("RedShoulders", rig.Chest, new Vector3(0f, 0.04f, -0.015f), new Vector3(0.88f, 0.53f, 0.50f), _materials.PlumberRed);

            CreateCube("OverallBib", rig.Spine, new Vector3(0f, 0.17f, 0.304f), new Vector3(0.53f, 0.50f, 0.075f), _materials.PlumberBlue);
            CreateSphere("BibLowerRound", rig.Spine, new Vector3(0f, -0.01f, 0.315f), new Vector3(0.55f, 0.22f, 0.085f), _materials.PlumberBlueDark);
            CreateCube("BibPocket", rig.Spine, new Vector3(0f, 0.14f, 0.355f), new Vector3(0.25f, 0.18f, 0.035f), _materials.PlumberBlueDark);
            CreateCube("PocketStitch", rig.Spine, new Vector3(0f, 0.09f, 0.378f), new Vector3(0.18f, 0.018f, 0.012f), _materials.PlumberGold);

            var leftStrap = CreateCube("LeftOverallStrap", rig.Chest, new Vector3(-0.22f, 0.05f, 0.277f), new Vector3(0.105f, 0.46f, 0.055f), _materials.PlumberBlue);
            leftStrap.transform.localRotation = Quaternion.Euler(0f, 0f, -8f);
            var rightStrap = CreateCube("RightOverallStrap", rig.Chest, new Vector3(0.22f, 0.05f, 0.277f), new Vector3(0.105f, 0.46f, 0.055f), _materials.PlumberBlue);
            rightStrap.transform.localRotation = Quaternion.Euler(0f, 0f, 8f);

            CreateSphere("LeftGoldButton", rig.Spine, new Vector3(-0.205f, 0.32f, 0.365f), Vector3.one * 0.115f, _materials.PlumberGold);
            CreateSphere("RightGoldButton", rig.Spine, new Vector3(0.205f, 0.32f, 0.365f), Vector3.one * 0.115f, _materials.PlumberGold);
            CreateCube("BackOverallPanel", rig.Spine, new Vector3(0f, 0.13f, -0.30f), new Vector3(0.55f, 0.46f, 0.06f), _materials.PlumberBlueDark);
        }

        private void BuildHead(JordanianHeroRig rig)
        {
            CreateCylinder("ShortNeck", rig.Neck, Vector3.zero, new Vector3(0.22f, 0.13f, 0.22f), _materials.PlumberSkin);
            CreateSphere("LargeHead", rig.Head, new Vector3(0f, 0.08f, 0f), new Vector3(0.72f, 0.69f, 0.64f), _materials.PlumberSkin);

            rig.Jaw = CreatePivot("Jaw", rig.Head, new Vector3(0f, -0.10f, 0.025f));
            CreateSphere("RoundedCheeks", rig.Jaw, new Vector3(0f, 0f, 0.03f), new Vector3(0.65f, 0.43f, 0.56f), _materials.PlumberSkin);
            CreateSphere("Chin", rig.Jaw, new Vector3(0f, -0.17f, 0.12f), new Vector3(0.31f, 0.18f, 0.25f), _materials.PlumberSkinLight);

            BuildEar(rig.Head, -1f);
            BuildEar(rig.Head, 1f);
            BuildEye(rig, -1f);
            BuildEye(rig, 1f);

            CreateSphere("LargeRoundNose", rig.Head, new Vector3(0f, 0.015f, 0.385f), new Vector3(0.29f, 0.25f, 0.29f), _materials.PlumberSkinLight);
            CreateSphere("NoseHighlight", rig.Head, new Vector3(-0.045f, 0.065f, 0.515f), new Vector3(0.070f, 0.055f, 0.030f), _materials.PlumberGlove);
            BuildMoustache(rig);

            CreateSphere("SmileMouth", rig.Jaw, new Vector3(0f, -0.135f, 0.313f), new Vector3(0.24f, 0.075f, 0.045f), _materials.PlumberHair);
            CreateSphere("SmileTeeth", rig.Jaw, new Vector3(0f, -0.119f, 0.347f), new Vector3(0.135f, 0.040f, 0.025f), _materials.PlumberGlove);

            BuildHair(rig);
            BuildCap(rig);
        }

        private void BuildEye(JordanianHeroRig rig, float side)
        {
            var label = side < 0f ? "Left" : "Right";
            var x = side * 0.145f;
            CreateSphere(label + "EyeWhite", rig.Head, new Vector3(x, 0.145f, 0.292f), new Vector3(0.18f, 0.245f, 0.095f), _materials.PlumberGlove);
            CreateSphere(label + "Iris", rig.Head, new Vector3(x, 0.145f, 0.370f), new Vector3(0.092f, 0.120f, 0.035f), _materials.PlumberEyeBlue);
            CreateSphere(label + "Pupil", rig.Head, new Vector3(x, 0.145f, 0.398f), new Vector3(0.043f, 0.064f, 0.020f), _materials.Dark);
            CreateSphere(label + "EyeGlint", rig.Head, new Vector3(x - 0.018f, 0.185f, 0.420f), Vector3.one * 0.025f, _materials.PlumberGlove);

            var eyelid = CreateSphere(label + "UpperEyelid", rig.Head, new Vector3(x, 0.255f, 0.355f), new Vector3(0.17f, 0.055f, 0.042f), _materials.PlumberSkin);
            var brow = CreateCapsule(label + "Brow", rig.Head, new Vector3(x, 0.300f, 0.302f), new Vector3(0.035f, 0.105f, 0.035f), _materials.PlumberHair);
            brow.transform.localRotation = Quaternion.Euler(0f, 0f, side * 72f);

            if (side < 0f)
            {
                rig.LeftUpperEyelid = eyelid.transform;
                rig.LeftUpperEyelidOpenPosition = eyelid.transform.localPosition;
            }
            else
            {
                rig.RightUpperEyelid = eyelid.transform;
                rig.RightUpperEyelidOpenPosition = eyelid.transform.localPosition;
            }
        }

        private void BuildEar(Transform head, float side)
        {
            var label = side < 0f ? "Left" : "Right";
            CreateSphere(label + "Ear", head, new Vector3(side * 0.365f, 0.055f, 0f), new Vector3(0.16f, 0.23f, 0.12f), _materials.PlumberSkin);
            CreateSphere(label + "InnerEar", head, new Vector3(side * 0.382f, 0.055f, 0.045f), new Vector3(0.075f, 0.135f, 0.045f), _materials.PlumberSkinLight);
        }

        private void BuildMoustache(JordanianHeroRig rig)
        {
            CreateSphere("MoustacheCenter", rig.Jaw, new Vector3(0f, 0.002f, 0.425f), new Vector3(0.18f, 0.11f, 0.09f), _materials.PlumberHair);
            for (var sideIndex = -1; sideIndex <= 1; sideIndex += 2)
            {
                var side = (float)sideIndex;
                var label = side < 0f ? "Left" : "Right";
                var inner = CreateSphere(label + "MoustacheInner", rig.Jaw, new Vector3(side * 0.105f, -0.005f, 0.420f), new Vector3(0.20f, 0.105f, 0.09f), _materials.PlumberHair);
                inner.transform.localRotation = Quaternion.Euler(0f, 0f, side * -9f);
                var outer = CreateSphere(label + "MoustacheOuter", rig.Jaw, new Vector3(side * 0.225f, -0.035f, 0.397f), new Vector3(0.19f, 0.095f, 0.085f), _materials.PlumberHair);
                outer.transform.localRotation = Quaternion.Euler(0f, 0f, side * -18f);
                CreateSphere(label + "MoustacheTip", rig.Jaw, new Vector3(side * 0.315f, -0.080f, 0.365f), new Vector3(0.11f, 0.075f, 0.070f), _materials.PlumberHair);
            }
        }

        private void BuildHair(JordanianHeroRig rig)
        {
            CreateSphere("BackHair", rig.Head, new Vector3(0f, 0.11f, -0.275f), new Vector3(0.60f, 0.49f, 0.18f), _materials.PlumberHair);
            for (var sideIndex = -1; sideIndex <= 1; sideIndex += 2)
            {
                var side = (float)sideIndex;
                CreateSphere(side < 0f ? "LeftSideburn" : "RightSideburn", rig.Head, new Vector3(side * 0.305f, -0.005f, 0.005f), new Vector3(0.105f, 0.23f, 0.13f), _materials.PlumberHair);
            }

            rig.HairFront = CreatePivot("HairFront", rig.Head, new Vector3(0f, 0.286f, 0.205f));
            CreateSphere("ForeheadCurl", rig.HairFront, Vector3.zero, new Vector3(0.22f, 0.10f, 0.09f), _materials.PlumberHair);
        }

        private void BuildCap(JordanianHeroRig rig)
        {
            CreateSphere("CapCrown", rig.Head, new Vector3(0f, 0.350f, -0.035f), new Vector3(0.73f, 0.40f, 0.66f), _materials.PlumberRed);
            CreateCube("CapBand", rig.Head, new Vector3(0f, 0.245f, 0.025f), new Vector3(0.70f, 0.11f, 0.55f), _materials.PlumberRedDark);
            CreateSphere("CapBrim", rig.Head, new Vector3(0f, 0.245f, 0.295f), new Vector3(0.52f, 0.105f, 0.39f), _materials.PlumberRed);
            CreateSphere("CapBadge", rig.Head, new Vector3(0f, 0.365f, 0.306f), new Vector3(0.23f, 0.23f, 0.055f), _materials.PlumberGlove);

            var badgeVertical = CreateCube("OriginalBadgeVertical", rig.Head, new Vector3(0f, 0.365f, 0.342f), new Vector3(0.055f, 0.135f, 0.018f), _materials.PlumberGold);
            badgeVertical.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            var badgeHorizontal = CreateCube("OriginalBadgeHorizontal", rig.Head, new Vector3(0f, 0.365f, 0.343f), new Vector3(0.055f, 0.135f, 0.018f), _materials.PlumberGold);
            badgeHorizontal.transform.localRotation = Quaternion.Euler(0f, 0f, -45f);
        }

        private void BuildArms(JordanianHeroRig rig)
        {
            BuildArm(rig, -1f);
            BuildArm(rig, 1f);
        }

        private void BuildArm(JordanianHeroRig rig, float side)
        {
            var label = side < 0f ? "Left" : "Right";
            var upperArm = CreatePivot(label + "UpperArm", rig.Chest, new Vector3(side * 0.49f, 0.13f, 0f));
            CreateSphere(label + "RoundShoulder", upperArm, Vector3.zero, new Vector3(0.29f, 0.29f, 0.29f), _materials.PlumberRed);
            CreateCapsule(label + "ShortSleeve", upperArm, new Vector3(0f, -0.20f, 0f), new Vector3(0.22f, 0.25f, 0.22f), _materials.PlumberRed);
            CreateCylinder(label + "SleeveCuff", upperArm, new Vector3(0f, -0.37f, 0f), new Vector3(0.22f, 0.045f, 0.22f), _materials.PlumberRedDark);

            var forearm = CreatePivot(label + "Forearm", upperArm, new Vector3(0f, -0.38f, 0f));
            CreateCapsule(label + "ForearmSkin", forearm, new Vector3(0f, -0.17f, 0f), new Vector3(0.17f, 0.22f, 0.17f), _materials.PlumberSkin);

            var hand = CreatePivot(label + "Hand", forearm, new Vector3(0f, -0.34f, 0f));
            CreateSphere(label + "GlovePalm", hand, new Vector3(0f, -0.09f, 0f), new Vector3(0.31f, 0.31f, 0.25f), _materials.PlumberGlove);
            CreateCylinder(label + "GloveCuff", hand, new Vector3(0f, 0.035f, 0f), new Vector3(0.19f, 0.065f, 0.19f), _materials.PlumberGlove);
            BuildGloveFingers(hand, side, label);

            if (side < 0f)
            {
                rig.LeftUpperArm = upperArm;
                rig.LeftForearm = forearm;
                rig.LeftHand = hand;
            }
            else
            {
                rig.RightUpperArm = upperArm;
                rig.RightForearm = forearm;
                rig.RightHand = hand;
            }
        }

        private void BuildGloveFingers(Transform hand, float side, string label)
        {
            for (var finger = 0; finger < 4; finger++)
            {
                var x = (finger - 1.5f) * 0.060f;
                CreateCapsule(label + "GloveFinger" + finger, hand, new Vector3(x, -0.235f, 0.035f), new Vector3(0.045f, 0.085f - finger * 0.004f, 0.045f), _materials.PlumberGlove);
            }

            var thumb = CreateCapsule(label + "GloveThumb", hand, new Vector3(side * 0.145f, -0.12f, 0.060f), new Vector3(0.055f, 0.105f, 0.055f), _materials.PlumberGlove);
            thumb.transform.localRotation = Quaternion.Euler(0f, 0f, side * -42f);
            CreateCube(label + "GloveSeam", hand, new Vector3(0f, -0.105f, 0.136f), new Vector3(0.18f, 0.020f, 0.015f), _materials.PlumberBlueDark);
        }

        private void BuildLegs(JordanianHeroRig rig)
        {
            BuildLeg(rig, -1f);
            BuildLeg(rig, 1f);
        }

        private void BuildLeg(JordanianHeroRig rig, float side)
        {
            var label = side < 0f ? "Left" : "Right";
            var upperLeg = CreatePivot(label + "UpperLeg", rig.Pelvis, new Vector3(side * 0.23f, -0.05f, 0f));
            CreateCapsule(label + "OverallThigh", upperLeg, new Vector3(0f, -0.17f, 0f), new Vector3(0.24f, 0.22f, 0.24f), _materials.PlumberBlue);
            CreateSphere(label + "OverallKnee", upperLeg, new Vector3(0f, -0.34f, 0f), Vector3.one * 0.235f, _materials.PlumberBlueDark);

            var calf = CreatePivot(label + "Calf", upperLeg, new Vector3(0f, -0.34f, 0f));
            CreateCapsule(label + "LowerOverall", calf, new Vector3(0f, -0.15f, 0f), new Vector3(0.21f, 0.19f, 0.21f), _materials.PlumberBlue);
            CreateCube(label + "TrouserCuff", calf, new Vector3(0f, -0.275f, 0f), new Vector3(0.40f, 0.10f, 0.36f), _materials.PlumberBlueDark);

            var foot = CreatePivot(label + "Foot", calf, new Vector3(0f, -0.29f, 0f));
            var shoe = CreateSphere(label + "BigBoot", foot, new Vector3(0f, 0.02f, 0.18f), new Vector3(0.48f, 0.25f, 0.72f), _materials.PlumberBoot);
            CreateSphere(label + "BootToe", foot, new Vector3(0f, 0.035f, 0.43f), new Vector3(0.46f, 0.22f, 0.33f), _materials.PlumberBootLight);
            CreateCube(label + "BootSole", foot, new Vector3(0f, -0.095f, 0.19f), new Vector3(0.47f, 0.075f, 0.72f), _materials.PlumberHair);
            CreateCube(label + "BootHighlight", foot, new Vector3(0f, 0.11f, 0.32f), new Vector3(0.25f, 0.025f, 0.15f), _materials.PlumberGold);

            if (side < 0f)
            {
                rig.LeftUpperLeg = upperLeg;
                rig.LeftCalf = calf;
                rig.LeftFoot = foot;
                rig.LeftShoe = shoe.transform;
            }
            else
            {
                rig.RightUpperLeg = upperLeg;
                rig.RightCalf = calf;
                rig.RightFoot = foot;
                rig.RightShoe = shoe.transform;
            }
        }

        private static Transform CreatePivot(string name, Transform parent, Vector3 position)
        {
            var pivot = new GameObject(name).transform;
            pivot.SetParent(parent, false);
            pivot.localPosition = position;
            return pivot;
        }

        private GameObject CreateCube(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            return CreatePrimitive(PrimitiveType.Cube, name, parent, position, scale, material);
        }

        private GameObject CreateSphere(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            return CreatePrimitive(PrimitiveType.Sphere, name, parent, position, scale, material);
        }

        private GameObject CreateCapsule(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            return CreatePrimitive(PrimitiveType.Capsule, name, parent, position, scale, material);
        }

        private GameObject CreateCylinder(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            return CreatePrimitive(PrimitiveType.Cylinder, name, parent, position, scale, material);
        }

        private static GameObject CreatePrimitive(PrimitiveType type, string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            var primitive = GameObject.CreatePrimitive(type);
            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            primitive.transform.localPosition = position;
            primitive.transform.localScale = scale;
            var collider = primitive.GetComponent<Collider>();
            if (collider != null)
            {
                Object.Destroy(collider);
            }

            var renderer = primitive.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            var castsShadow = Mathf.Max(scale.x, scale.y, scale.z) >= 0.11f;
            renderer.shadowCastingMode = castsShadow ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = castsShadow;
            return primitive;
        }
    }
}
