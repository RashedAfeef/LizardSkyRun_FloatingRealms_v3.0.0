using DesertDash.World;
using UnityEngine;

namespace DesertDash.Player
{
    public sealed class JordanianHeroBuilder
    {
        private readonly RuntimeMaterialLibrary _materials;

        public JordanianHeroBuilder(RuntimeMaterialLibrary materials)
        {
            _materials = materials;
        }

        public JordanianHeroRig Build(Transform parent)
        {
            var rig = new JordanianHeroRig();
            rig.Root = CreatePivot("JordanianHeroRig", parent, Vector3.zero);
            rig.Pelvis = CreatePivot("Pelvis", rig.Root, new Vector3(0f, 0.93f, 0f));
            rig.Spine = CreatePivot("Spine", rig.Pelvis, new Vector3(0f, 0.20f, 0f));
            rig.Chest = CreatePivot("Chest", rig.Spine, new Vector3(0f, 0.30f, 0f));
            rig.Neck = CreatePivot("Neck", rig.Chest, new Vector3(0f, 0.32f, 0f));
            rig.Head = CreatePivot("Head", rig.Neck, new Vector3(0f, 0.22f, 0f));

            BuildTorso(rig);
            BuildHeadAndFace(rig);
            BuildArms(rig);
            BuildLegs(rig);
            BuildKeffiyeh(rig);
            BuildModernAccessories(rig);
            return rig;
        }

        private void BuildTorso(JordanianHeroRig rig)
        {
            CreateCube("Hips", rig.Pelvis, new Vector3(0f, 0f, 0f), new Vector3(0.62f, 0.27f, 0.38f), _materials.HeroDenim);
            CreateCube("Waistband", rig.Pelvis, new Vector3(0f, 0.14f, 0f), new Vector3(0.65f, 0.09f, 0.40f), _materials.LeatherBrown);
            CreateCube("BeltBuckle", rig.Pelvis, new Vector3(0f, 0.14f, 0.215f), new Vector3(0.15f, 0.10f, 0.035f), _materials.JewelrySilver);

            CreateCapsule("Abdomen", rig.Spine, new Vector3(0f, 0.12f, 0f), new Vector3(0.30f, 0.24f, 0.22f), _materials.HeroShirt);
            CreateCube("ChestCore", rig.Chest, new Vector3(0f, 0.04f, 0f), new Vector3(0.83f, 0.48f, 0.43f), _materials.HeroShirt);

            var leftJacket = CreateCube("LeftJacketPanel", rig.Chest, new Vector3(-0.225f, 0.04f, 0.228f), new Vector3(0.39f, 0.47f, 0.055f), _materials.HeroJacket);
            leftJacket.transform.localRotation = Quaternion.Euler(0f, -5f, -2f);
            var rightJacket = CreateCube("RightJacketPanel", rig.Chest, new Vector3(0.225f, 0.04f, 0.228f), new Vector3(0.39f, 0.47f, 0.055f), _materials.HeroJacket);
            rightJacket.transform.localRotation = Quaternion.Euler(0f, 5f, 2f);

            CreateCube("JacketBack", rig.Chest, new Vector3(0f, 0.04f, -0.22f), new Vector3(0.82f, 0.47f, 0.07f), _materials.HeroJacketLight);
            CreateCube("LeftLapel", rig.Chest, new Vector3(-0.12f, 0.15f, 0.272f), new Vector3(0.14f, 0.33f, 0.035f), _materials.HeroJacketLight).transform.localRotation = Quaternion.Euler(0f, 0f, -18f);
            CreateCube("RightLapel", rig.Chest, new Vector3(0.12f, 0.15f, 0.272f), new Vector3(0.14f, 0.33f, 0.035f), _materials.HeroJacketLight).transform.localRotation = Quaternion.Euler(0f, 0f, 18f);
            CreateCube("JacketZipper", rig.Chest, new Vector3(0f, -0.01f, 0.270f), new Vector3(0.024f, 0.42f, 0.025f), _materials.JewelrySilver);
            CreateCube("LeftPocket", rig.Chest, new Vector3(-0.24f, -0.10f, 0.278f), new Vector3(0.22f, 0.045f, 0.030f), _materials.HeroJacketLight).transform.localRotation = Quaternion.Euler(0f, 0f, -12f);
            CreateCube("RightPocket", rig.Chest, new Vector3(0.24f, -0.10f, 0.278f), new Vector3(0.22f, 0.045f, 0.030f), _materials.HeroJacketLight).transform.localRotation = Quaternion.Euler(0f, 0f, 12f);
            CreateCube("JacketHem", rig.Spine, new Vector3(0f, -0.02f, 0f), new Vector3(0.68f, 0.10f, 0.42f), _materials.HeroJacket);

            CreateCube("JordanGreenShirtBand", rig.Chest, new Vector3(0f, 0.01f, 0.296f), new Vector3(0.08f, 0.32f, 0.018f), _materials.JordanGreen);
            CreateCube("JordanRedShirtBand", rig.Chest, new Vector3(0.075f, 0.01f, 0.296f), new Vector3(0.035f, 0.32f, 0.019f), _materials.JordanRed);
        }

        private void BuildHeadAndFace(JordanianHeroRig rig)
        {
            CreateCylinder("NeckBase", rig.Neck, new Vector3(0f, 0f, 0f), new Vector3(0.17f, 0.16f, 0.17f), _materials.HeroSkin);
            CreateSphere("HeadCranium", rig.Head, new Vector3(0f, 0.10f, 0f), new Vector3(0.50f, 0.55f, 0.47f), _materials.HeroSkin);
            rig.Jaw = CreatePivot("Jaw", rig.Head, new Vector3(0f, -0.075f, 0.025f));
            CreateSphere("JawShape", rig.Jaw, Vector3.zero, new Vector3(0.45f, 0.34f, 0.405f), _materials.HeroSkin);
            CreateSphere("Chin", rig.Jaw, new Vector3(0f, -0.13f, 0.075f), new Vector3(0.22f, 0.14f, 0.21f), _materials.HeroSkinLight);

            BuildEar(rig.Head, -1f);
            BuildEar(rig.Head, 1f);
            BuildEye(rig, -1f);
            BuildEye(rig, 1f);

            CreateCapsule("NoseBridge", rig.Head, new Vector3(0f, 0.075f, 0.235f), new Vector3(0.055f, 0.105f, 0.055f), _materials.HeroSkinLight);
            CreateSphere("NoseTip", rig.Head, new Vector3(0f, -0.015f, 0.277f), new Vector3(0.115f, 0.09f, 0.105f), _materials.HeroSkinLight);
            CreateSphere("LeftNostril", rig.Head, new Vector3(-0.040f, -0.034f, 0.326f), new Vector3(0.025f, 0.018f, 0.015f), _materials.HeroHair);
            CreateSphere("RightNostril", rig.Head, new Vector3(0.040f, -0.034f, 0.326f), new Vector3(0.025f, 0.018f, 0.015f), _materials.HeroHair);

            CreateSphere("UpperLip", rig.Jaw, new Vector3(0f, -0.025f, 0.238f), new Vector3(0.19f, 0.045f, 0.045f), _materials.HeroLip);
            CreateSphere("LowerLip", rig.Jaw, new Vector3(0f, -0.075f, 0.235f), new Vector3(0.18f, 0.055f, 0.045f), _materials.HeroLip);
            CreateCube("SmileLine", rig.Jaw, new Vector3(0f, -0.048f, 0.270f), new Vector3(0.17f, 0.015f, 0.015f), _materials.HeroHair);
            CreateCube("TeethHighlight", rig.Jaw, new Vector3(0f, -0.050f, 0.277f), new Vector3(0.10f, 0.018f, 0.012f), _materials.White);

            BuildHair(rig);
            BuildBeard(rig);
        }

        private void BuildEye(JordanianHeroRig rig, float side)
        {
            var x = side * 0.135f;
            CreateSphere(side < 0f ? "LeftEyeWhite" : "RightEyeWhite", rig.Head, new Vector3(x, 0.145f, 0.232f), new Vector3(0.155f, 0.115f, 0.075f), _materials.White);
            CreateSphere(side < 0f ? "LeftIris" : "RightIris", rig.Head, new Vector3(x, 0.145f, 0.294f), new Vector3(0.073f, 0.073f, 0.025f), _materials.HeroEyeBrown);
            CreateSphere(side < 0f ? "LeftPupil" : "RightPupil", rig.Head, new Vector3(x, 0.145f, 0.315f), new Vector3(0.033f, 0.041f, 0.014f), _materials.Dark);
            CreateSphere(side < 0f ? "LeftEyeGlint" : "RightEyeGlint", rig.Head, new Vector3(x - 0.017f, 0.170f, 0.326f), Vector3.one * 0.018f, _materials.White);

            var eyelid = CreateSphere(side < 0f ? "LeftUpperEyelid" : "RightUpperEyelid", rig.Head, new Vector3(x, 0.221f, 0.286f), new Vector3(0.17f, 0.055f, 0.042f), _materials.HeroSkin);
            var brow = CreateCapsule(side < 0f ? "LeftBrow" : "RightBrow", rig.Head, new Vector3(x, 0.273f, 0.267f), new Vector3(0.030f, 0.105f, 0.028f), _materials.HeroHair);
            brow.transform.localRotation = Quaternion.Euler(0f, 0f, side * 78f);

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
            CreateSphere(side < 0f ? "LeftEar" : "RightEar", head, new Vector3(side * 0.265f, 0.075f, 0f), new Vector3(0.115f, 0.18f, 0.085f), _materials.HeroSkin);
            CreateSphere(side < 0f ? "LeftInnerEar" : "RightInnerEar", head, new Vector3(side * 0.273f, 0.075f, 0.032f), new Vector3(0.055f, 0.105f, 0.035f), _materials.HeroSkinLight);
            if (side < 0f)
            {
                CreateSphere("SilverStud", head, new Vector3(side * 0.284f, 0.015f, 0.065f), Vector3.one * 0.032f, _materials.JewelrySilver);
            }
        }

        private void BuildHair(JordanianHeroRig rig)
        {
            CreateSphere("HairCrown", rig.Head, new Vector3(0f, 0.335f, -0.015f), new Vector3(0.51f, 0.24f, 0.46f), _materials.HeroHair);
            CreateSphere("HairLeftFade", rig.Head, new Vector3(-0.235f, 0.235f, -0.010f), new Vector3(0.095f, 0.24f, 0.36f), _materials.HeroHair);
            CreateSphere("HairRightFade", rig.Head, new Vector3(0.235f, 0.235f, -0.010f), new Vector3(0.095f, 0.24f, 0.36f), _materials.HeroHair);
            rig.HairFront = CreatePivot("HairFront", rig.Head, new Vector3(0f, 0.355f, 0.165f));
            for (var lockIndex = -2; lockIndex <= 2; lockIndex++)
            {
                var lockTransform = CreateCapsule("HairLock", rig.HairFront, new Vector3(lockIndex * 0.072f, -Mathf.Abs(lockIndex) * 0.012f, 0.018f), new Vector3(0.055f, 0.115f, 0.055f), _materials.HeroHair);
                lockTransform.transform.localRotation = Quaternion.Euler(68f, 0f, lockIndex * -9f);
            }
        }

        private void BuildBeard(JordanianHeroRig rig)
        {
            CreateSphere("BeardChin", rig.Jaw, new Vector3(0f, -0.155f, 0.080f), new Vector3(0.30f, 0.16f, 0.28f), _materials.HeroBeard);
            CreateSphere("BeardLeft", rig.Jaw, new Vector3(-0.185f, -0.040f, 0.080f), new Vector3(0.12f, 0.25f, 0.27f), _materials.HeroBeard);
            CreateSphere("BeardRight", rig.Jaw, new Vector3(0.185f, -0.040f, 0.080f), new Vector3(0.12f, 0.25f, 0.27f), _materials.HeroBeard);
            CreateCapsule("MoustacheLeft", rig.Jaw, new Vector3(-0.074f, 0.003f, 0.253f), new Vector3(0.025f, 0.082f, 0.024f), _materials.HeroBeard).transform.localRotation = Quaternion.Euler(0f, 0f, 72f);
            CreateCapsule("MoustacheRight", rig.Jaw, new Vector3(0.074f, 0.003f, 0.253f), new Vector3(0.025f, 0.082f, 0.024f), _materials.HeroBeard).transform.localRotation = Quaternion.Euler(0f, 0f, -72f);
        }

        private void BuildArms(JordanianHeroRig rig)
        {
            BuildArm(rig, -1f);
            BuildArm(rig, 1f);
        }

        private void BuildArm(JordanianHeroRig rig, float side)
        {
            var label = side < 0f ? "Left" : "Right";
            var upperArm = CreatePivot(label + "UpperArm", rig.Chest, new Vector3(side * 0.51f, 0.18f, 0f));
            CreateSphere(label + "Shoulder", upperArm, Vector3.zero, new Vector3(0.25f, 0.25f, 0.25f), _materials.HeroJacket);
            CreateCapsule(label + "UpperArmSleeve", upperArm, new Vector3(0f, -0.235f, 0f), new Vector3(0.17f, 0.275f, 0.17f), _materials.HeroJacket);
            CreateCube(label + "SleeveSeam", upperArm, new Vector3(side * 0.145f, -0.235f, 0f), new Vector3(0.018f, 0.38f, 0.10f), _materials.HeroJacketLight);
            CreateSphere(label + "Elbow", upperArm, new Vector3(0f, -0.465f, 0f), Vector3.one * 0.165f, _materials.HeroJacketLight);

            var forearm = CreatePivot(label + "Forearm", upperArm, new Vector3(0f, -0.465f, 0f));
            CreateCapsule(label + "ForearmSleeve", forearm, new Vector3(0f, -0.215f, 0f), new Vector3(0.145f, 0.245f, 0.145f), _materials.HeroJacketLight);
            CreateCylinder(label + "SleeveCuff", forearm, new Vector3(0f, -0.405f, 0f), new Vector3(0.155f, 0.060f, 0.155f), _materials.HeroJacket);

            var hand = CreatePivot(label + "Hand", forearm, new Vector3(0f, -0.435f, 0f));
            CreateSphere(label + "Palm", hand, new Vector3(0f, -0.105f, 0f), new Vector3(0.22f, 0.27f, 0.16f), _materials.HeroSkin);
            BuildFingers(hand, side, label);

            if (side < 0f)
            {
                rig.LeftUpperArm = upperArm;
                rig.LeftForearm = forearm;
                rig.LeftHand = hand;
                BuildSmartWatch(rig, forearm);
            }
            else
            {
                rig.RightUpperArm = upperArm;
                rig.RightForearm = forearm;
                rig.RightHand = hand;
                BuildJordanBracelets(forearm);
            }
        }

        private void BuildFingers(Transform hand, float side, string label)
        {
            for (var finger = 0; finger < 4; finger++)
            {
                var x = (finger - 1.5f) * 0.050f;
                CreateCapsule(label + "Finger" + finger, hand, new Vector3(x, -0.255f, 0.018f), new Vector3(0.035f, 0.085f - finger * 0.004f, 0.035f), _materials.HeroSkinLight);
            }

            var thumb = CreateCapsule(label + "Thumb", hand, new Vector3(side * 0.115f, -0.115f, 0.055f), new Vector3(0.045f, 0.095f, 0.045f), _materials.HeroSkinLight);
            thumb.transform.localRotation = Quaternion.Euler(0f, 0f, side * -32f);
        }

        private void BuildSmartWatch(JordanianHeroRig rig, Transform forearm)
        {
            CreateCylinder("WatchBand", forearm, new Vector3(0f, -0.395f, 0f), new Vector3(0.175f, 0.048f, 0.175f), _materials.Dark);
            var watchBody = CreateCube("WatchBody", forearm, new Vector3(0f, -0.395f, 0.155f), new Vector3(0.22f, 0.13f, 0.055f), _materials.WatchMetal);
            rig.WatchScreen = CreateCube("WatchScreen", watchBody.transform, new Vector3(0f, 0f, 0.56f), new Vector3(0.78f, 0.72f, 0.06f), _materials.WatchGlass).transform;
            CreateCube("WatchDisplayLine", rig.WatchScreen, new Vector3(0f, 0f, 0.60f), new Vector3(0.42f, 0.10f, 0.10f), _materials.Player);
        }

        private void BuildJordanBracelets(Transform forearm)
        {
            CreateCylinder("BlackBracelet", forearm, new Vector3(0f, -0.375f, 0f), new Vector3(0.165f, 0.025f, 0.165f), _materials.Dark);
            CreateCylinder("WhiteBracelet", forearm, new Vector3(0f, -0.405f, 0f), new Vector3(0.165f, 0.025f, 0.165f), _materials.White);
            CreateCylinder("GreenBracelet", forearm, new Vector3(0f, -0.435f, 0f), new Vector3(0.165f, 0.025f, 0.165f), _materials.JordanGreen);
            CreateCylinder("RedBracelet", forearm, new Vector3(0f, -0.465f, 0f), new Vector3(0.165f, 0.025f, 0.165f), _materials.JordanRed);
        }

        private void BuildLegs(JordanianHeroRig rig)
        {
            BuildLeg(rig, -1f);
            BuildLeg(rig, 1f);
        }

        private void BuildLeg(JordanianHeroRig rig, float side)
        {
            var label = side < 0f ? "Left" : "Right";
            var upperLeg = CreatePivot(label + "UpperLeg", rig.Pelvis, new Vector3(side * 0.205f, -0.06f, 0f));
            CreateCapsule(label + "Thigh", upperLeg, new Vector3(0f, -0.225f, 0f), new Vector3(0.205f, 0.265f, 0.205f), _materials.HeroDenim);
            CreateCube(label + "DenimSeam", upperLeg, new Vector3(side * 0.175f, -0.225f, 0f), new Vector3(0.020f, 0.38f, 0.12f), _materials.HeroDenimLight);
            CreateSphere(label + "Knee", upperLeg, new Vector3(0f, -0.425f, 0f), Vector3.one * 0.18f, _materials.HeroDenimLight);

            var calf = CreatePivot(label + "Calf", upperLeg, new Vector3(0f, -0.43f, 0f));
            CreateCapsule(label + "LowerLeg", calf, new Vector3(0f, -0.195f, 0f), new Vector3(0.175f, 0.235f, 0.175f), _materials.HeroDenim);
            CreateCube(label + "Cuff", calf, new Vector3(0f, -0.365f, 0f), new Vector3(0.34f, 0.09f, 0.30f), _materials.HeroDenimLight);

            var foot = CreatePivot(label + "Foot", calf, new Vector3(0f, -0.385f, 0f));
            var shoe = CreateCube(label + "SneakerBody", foot, new Vector3(0f, 0.045f, 0.135f), new Vector3(0.36f, 0.20f, 0.61f), _materials.HeroSneaker);
            CreateCube(label + "Sole", foot, new Vector3(0f, -0.058f, 0.145f), new Vector3(0.38f, 0.060f, 0.66f), _materials.HeroSole);
            CreateCube(label + "ToeCap", foot, new Vector3(0f, 0.055f, 0.435f), new Vector3(0.34f, 0.16f, 0.18f), _materials.HeroSole);
            CreateCube(label + "Heel", foot, new Vector3(0f, 0.055f, -0.170f), new Vector3(0.35f, 0.22f, 0.13f), _materials.Dark);
            for (var lace = 0; lace < 4; lace++)
            {
                CreateCube(label + "Lace" + lace, foot, new Vector3(0f, 0.155f, 0.08f + lace * 0.075f), new Vector3(0.27f, 0.018f, 0.028f), _materials.White).transform.localRotation = Quaternion.Euler(-5f, 0f, lace % 2 == 0 ? 8f : -8f);
            }

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

        private void BuildKeffiyeh(JordanianHeroRig rig)
        {
            CreateCylinder("KeffiyehNeckWrapWhite", rig.Neck, new Vector3(0f, 0.01f, 0f), new Vector3(0.34f, 0.105f, 0.34f), _materials.KeffiyehWhite);
            CreateCylinder("KeffiyehNeckWrapRed", rig.Neck, new Vector3(0f, 0.045f, 0f), new Vector3(0.35f, 0.028f, 0.35f), _materials.KeffiyehRed);
            CreateCylinder("KeffiyehLowerBand", rig.Neck, new Vector3(0f, -0.055f, 0f), new Vector3(0.33f, 0.025f, 0.33f), _materials.KeffiyehRed);
            CreateSphere("KeffiyehKnot", rig.Chest, new Vector3(0f, 0.285f, 0.295f), new Vector3(0.20f, 0.17f, 0.12f), _materials.KeffiyehWhite);
            CreateCube("KnotRedBand", rig.Chest, new Vector3(0f, 0.285f, 0.365f), new Vector3(0.13f, 0.035f, 0.028f), _materials.KeffiyehRed);

            rig.ScarfLeftRoot = BuildScarfTail("LeftKeffiyehTail", rig.Chest, new Vector3(-0.17f, 0.245f, 0.295f), -1f, rig.ScarfLeftSegments);
            rig.ScarfRightRoot = BuildScarfTail("RightKeffiyehTail", rig.Chest, new Vector3(0.17f, 0.245f, 0.285f), 1f, rig.ScarfRightSegments);
        }

        private Transform BuildScarfTail(string name, Transform parent, Vector3 position, float side, System.Collections.Generic.List<Transform> segments)
        {
            var root = CreatePivot(name, parent, position);
            root.localRotation = Quaternion.Euler(4f, side * 5f, side * 8f);
            var current = root;
            for (var segmentIndex = 0; segmentIndex < 4; segmentIndex++)
            {
                var length = segmentIndex == 3 ? 0.24f : 0.28f;
                var width = Mathf.Lerp(0.23f, 0.16f, segmentIndex / 3f);
                var pivot = CreatePivot(name + "Segment" + segmentIndex, current, segmentIndex == 0 ? Vector3.zero : new Vector3(0f, -0.26f, 0f));
                CreateCube("WhiteCloth", pivot, new Vector3(0f, -length * 0.5f, 0f), new Vector3(width, length, 0.045f), _materials.KeffiyehWhite);
                CreateCube("RedEdgeLeft", pivot, new Vector3(-width * 0.36f, -length * 0.5f, 0.027f), new Vector3(0.025f, length * 0.92f, 0.012f), _materials.KeffiyehRed);
                CreateCube("RedEdgeRight", pivot, new Vector3(width * 0.36f, -length * 0.5f, 0.027f), new Vector3(0.025f, length * 0.92f, 0.012f), _materials.KeffiyehRed);
                CreateCube("RedCrossTop", pivot, new Vector3(0f, -length * 0.30f, 0.028f), new Vector3(width * 0.88f, 0.025f, 0.012f), _materials.KeffiyehRed);
                CreateCube("RedCrossBottom", pivot, new Vector3(0f, -length * 0.68f, 0.028f), new Vector3(width * 0.88f, 0.025f, 0.012f), _materials.KeffiyehRed);
                segments.Add(pivot);
                current = pivot;
            }

            for (var tassel = -2; tassel <= 2; tassel++)
            {
                var thread = CreateCapsule("KeffiyehTassel", current, new Vector3(tassel * 0.027f, -0.31f, 0f), new Vector3(0.012f, 0.055f, 0.012f), tassel % 2 == 0 ? _materials.KeffiyehRed : _materials.KeffiyehWhite);
                thread.transform.localRotation = Quaternion.Euler(0f, 0f, tassel * 4f);
            }

            return root;
        }

        private void BuildModernAccessories(JordanianHeroRig rig)
        {
            var strap = CreateCube("CrossBodyStrap", rig.Chest, new Vector3(-0.02f, 0.015f, 0.305f), new Vector3(0.075f, 0.76f, 0.035f), _materials.LeatherBrown);
            strap.transform.localRotation = Quaternion.Euler(0f, 0f, -31f);
            CreateCube("StrapHighlight", strap.transform, new Vector3(0f, 0f, 0.58f), new Vector3(0.48f, 0.94f, 0.08f), _materials.LeatherLight);
            CreateCube("StrapBuckle", rig.Chest, new Vector3(-0.12f, 0.10f, 0.342f), new Vector3(0.12f, 0.15f, 0.035f), _materials.JewelrySilver).transform.localRotation = Quaternion.Euler(0f, 0f, -31f);

            rig.CrossBodyBag = CreatePivot("CrossBodyBag", rig.Pelvis, new Vector3(0.38f, -0.035f, 0.29f));
            CreateCube("BagBody", rig.CrossBodyBag, Vector3.zero, new Vector3(0.48f, 0.52f, 0.22f), _materials.LeatherBrown);
            rig.BagFlap = CreateCube("BagFlap", rig.CrossBodyBag, new Vector3(0f, 0.10f, 0.125f), new Vector3(0.46f, 0.25f, 0.055f), _materials.LeatherLight).transform;
            CreateCube("BagBuckle", rig.BagFlap, new Vector3(0f, -0.22f, 0.62f), new Vector3(0.16f, 0.22f, 0.08f), _materials.JewelrySilver);
            CreateCube("BagGreenTag", rig.CrossBodyBag, new Vector3(0.16f, -0.17f, 0.135f), new Vector3(0.07f, 0.16f, 0.025f), _materials.JordanGreen);
            CreateCube("BagRedTag", rig.CrossBodyBag, new Vector3(0.095f, -0.17f, 0.137f), new Vector3(0.035f, 0.16f, 0.026f), _materials.JordanRed);

            var necklace = CreateCylinder("Necklace", rig.Neck, new Vector3(0f, -0.075f, 0.18f), new Vector3(0.21f, 0.015f, 0.21f), _materials.JewelrySilver);
            necklace.transform.localRotation = Quaternion.Euler(72f, 0f, 0f);
            CreateCube("NecklacePendant", rig.Chest, new Vector3(0f, 0.135f, 0.30f), new Vector3(0.07f, 0.11f, 0.025f), _materials.JewelrySilver).transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
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
            var castsShadow = Mathf.Max(scale.x, scale.y, scale.z) >= 0.12f;
            renderer.shadowCastingMode = castsShadow ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = castsShadow;
            return primitive;
        }
    }
}
