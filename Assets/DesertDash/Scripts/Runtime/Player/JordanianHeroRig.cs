using System.Collections.Generic;
using UnityEngine;

namespace DesertDash.Player
{
    public sealed class JordanianHeroRig
    {
        public Transform Root;
        public Transform Pelvis;
        public Transform Spine;
        public Transform Chest;
        public Transform Neck;
        public Transform Head;
        public Transform Jaw;

        public Transform LeftUpperArm;
        public Transform RightUpperArm;
        public Transform LeftForearm;
        public Transform RightForearm;
        public Transform LeftHand;
        public Transform RightHand;

        public Transform LeftUpperLeg;
        public Transform RightUpperLeg;
        public Transform LeftCalf;
        public Transform RightCalf;
        public Transform LeftFoot;
        public Transform RightFoot;

        public Transform LeftUpperEyelid;
        public Transform RightUpperEyelid;
        public Vector3 LeftUpperEyelidOpenPosition;
        public Vector3 RightUpperEyelidOpenPosition;

        public Transform ScarfLeftRoot;
        public Transform ScarfRightRoot;
        public readonly List<Transform> ScarfLeftSegments = new List<Transform>();
        public readonly List<Transform> ScarfRightSegments = new List<Transform>();

        public Transform CrossBodyBag;
        public Transform BagFlap;
        public Transform WatchScreen;
        public Transform HairFront;
        public Transform LeftShoe;
        public Transform RightShoe;
    }
}
