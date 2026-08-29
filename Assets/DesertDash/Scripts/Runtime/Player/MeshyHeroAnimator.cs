using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace DesertDash.Player
{
    public sealed class MeshyHeroAnimator : MonoBehaviour
    {
        private enum Motion { Idle, Running, Airborne, Sliding, Stumble }

        private readonly Dictionary<string, int> _muscleIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixer;
        private AnimationClipPlayable[] _playables;
        private float[] _weights;
        private HumanPoseHandler _poseHandler;
        private HumanPose _humanPose;
        private bool _initialized;
        private bool _wasGrounded = true;
        private bool _wasSliding;
        private float _runBlend;
        private float _slideBlend;
        private float _stumbleRemaining;
        private float _landingRemaining;
        private float _celebrateRemaining;
        private float _fallBlend;

        public bool Initialize(Animator animator, string resourcePath, string motionFolder)
        {
            if (animator == null || animator.avatar == null || !animator.avatar.isValid || !animator.avatar.isHuman)
            {
                Debug.LogWarning("The Meshy character needs a valid Humanoid Avatar before procedural motions can be applied.");
                return false;
            }

            var clips = CollectClips(resourcePath, motionFolder);
            var running = FindClip(clips, "Running", "Run_03", "Run");
            var alternateRun = FindClip(clips, "Run_03", "Running", "Run");
            var walking = FindClip(clips, "Walking", "Walk", "Running");
            var idle = FindClip(clips, "Idle", "Standing Idle", "Character_output.fbx");
            var dedicatedJump = FindClip(clips, "Running Jump", "Jump", "Leap");
            var dedicatedSlide = FindClip(clips, "Slide", "Crouch", "Roll");
            var dedicatedStumble = FindClip(clips, "Stumble", "Hit", "Fall", "Trip");
            if (idle == null) idle = walking;
            var jump = dedicatedJump != null ? dedicatedJump : alternateRun;
            var slide = dedicatedSlide != null ? dedicatedSlide : walking;
            var stumble = dedicatedStumble != null ? dedicatedStumble : alternateRun;
            if (running == null || idle == null || jump == null || slide == null || stumble == null)
            {
                Debug.LogWarning("The Meshy hero animation source is missing the minimum clips needed for the motion graph.");
                return false;
            }

            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.runtimeAnimatorController = null;
            _poseHandler = new HumanPoseHandler(animator.avatar, animator.transform);
            BuildMuscleIndex();

            _graph = PlayableGraph.Create("Lizard Runner Motion Graph");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            _mixer = AnimationMixerPlayable.Create(_graph, 5);
            _playables = new AnimationClipPlayable[5];
            _weights = new float[5];

            CreatePlayable((int)Motion.Idle, idle, 0.58d);
            CreatePlayable((int)Motion.Running, running, 1d);
            CreatePlayable((int)Motion.Airborne, jump, dedicatedJump != null ? 0.78d : 0d);
            CreatePlayable((int)Motion.Sliding, slide, dedicatedSlide != null ? 0.88d : 0d);
            CreatePlayable((int)Motion.Stumble, stumble, dedicatedStumble != null ? 1d : 0d);
            _weights[(int)Motion.Idle] = 1f;
            for (var index = 0; index < _weights.Length; index++)
            {
                _mixer.SetInputWeight(index, _weights[index]);
            }

            var output = AnimationPlayableOutput.Create(_graph, "Lizard Runner", animator);
            output.SetSourcePlayable(_mixer);
            _graph.Play();
            _initialized = true;
            return true;
        }

        public void Tick(bool running, bool grounded, bool sliding, bool gameOver, bool boardActive, float runnerSpeed, float verticalVelocity, float laneLean, float slideProgress)
        {
            if (!_initialized)
            {
                return;
            }

            var deltaTime = Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            if (grounded && !_wasGrounded && !sliding)
            {
                _landingRemaining = 0.28f;
            }

            if (sliding && !_wasSliding)
            {
                _playables[(int)Motion.Sliding].SetTime(0d);
            }

            _wasGrounded = grounded;
            _wasSliding = sliding;
            _runBlend = Mathf.MoveTowards(_runBlend, running ? 1f : 0f, deltaTime * 4.5f);
            _slideBlend = Mathf.MoveTowards(_slideBlend, sliding ? 1f : 0f, deltaTime * (sliding ? 9f : 6f));
            _stumbleRemaining = Mathf.Max(0f, _stumbleRemaining - deltaTime);
            _landingRemaining = Mathf.Max(0f, _landingRemaining - deltaTime);
            _celebrateRemaining = Mathf.Max(0f, _celebrateRemaining - deltaTime);
            _fallBlend = Mathf.MoveTowards(_fallBlend, gameOver && _stumbleRemaining <= 0f ? 1f : 0f, deltaTime * 2.2f);

            var target = Motion.Idle;
            if (_stumbleRemaining > 0f || gameOver)
            {
                target = Motion.Stumble;
            }
            else if (running)
            {
                target = sliding ? Motion.Sliding : grounded ? Motion.Running : Motion.Airborne;
            }

            var blend = 1f - Mathf.Exp(-12f * deltaTime);
            for (var index = 0; index < _weights.Length; index++)
            {
                _weights[index] = Mathf.Lerp(_weights[index], index == (int)target ? 1f : 0f, blend);
                _mixer.SetInputWeight(index, _weights[index]);
            }

            _playables[(int)Motion.Running].SetSpeed(Mathf.Lerp(0.86f, 1.46f, Mathf.InverseLerp(8f, 25f, runnerSpeed)));
            ApplyProceduralPose(running, grounded, sliding, gameOver, boardActive, runnerSpeed, verticalVelocity, laneLean, slideProgress);
        }

        public void TriggerStumble()
        {
            if (!_initialized)
            {
                return;
            }

            _stumbleRemaining = 0.82f;
            _playables[(int)Motion.Stumble].SetTime(0d);
        }

        public void TriggerCelebrate()
        {
            if (_initialized)
            {
                _celebrateRemaining = 1.9f;
            }
        }

        private void ApplyProceduralPose(bool running, bool grounded, bool sliding, bool gameOver, bool boardActive, float runnerSpeed, float verticalVelocity, float laneLean, float slideProgress)
        {
            if (_poseHandler == null)
            {
                return;
            }

            _poseHandler.GetHumanPose(ref _humanPose);
            var lean = Mathf.Clamp(laneLean / 14f, -1f, 1f);
            ApplyLaneChangePose(lean, running);

            if (!running && !gameOver && _celebrateRemaining <= 0f) ApplyIdlePose();
            if (running && grounded && !sliding && !boardActive) ApplyRunPose(runnerSpeed);
            if (running && !grounded && !sliding) ApplyJumpPose(verticalVelocity);
            if (_slideBlend > 0.001f) ApplySlidePose(slideProgress, _slideBlend);
            if (boardActive && running && grounded && !sliding) ApplyBoardPose(runnerSpeed);
            if (_landingRemaining > 0f) ApplyLandingPose(_landingRemaining / 0.28f);
            if (_stumbleRemaining > 0f) ApplyStumblePose(_stumbleRemaining / 0.82f);
            if (_fallBlend > 0f) ApplyFallPose(_fallBlend);
            if (_celebrateRemaining > 0f && !gameOver) ApplyCelebratePose(_celebrateRemaining / 1.9f);

            ClampMuscles();
            _poseHandler.SetHumanPose(ref _humanPose);
        }

        private void ApplyIdlePose()
        {
            var time = Time.unscaledTime;
            var breath = Mathf.Sin(time * 2.15f);
            var look = Mathf.Sin(time * 0.43f);
            AddMuscle("Spine Front-Back", breath * 0.025f);
            AddMuscle("Chest Front-Back", -breath * 0.035f);
            AddMuscle("Chest Twist Left-Right", look * 0.025f);
            AddMuscle("Head Nod Down-Up", breath * 0.018f);
            AddMuscle("Head Turn Left-Right", look * 0.075f);
            AddMuscle("Left Shoulder Front-Back", breath * 0.018f);
            AddMuscle("Right Shoulder Front-Back", breath * -0.018f);
        }

        private void ApplyRunPose(float runnerSpeed)
        {
            var sprint = Mathf.InverseLerp(10f, 24f, runnerSpeed);
            var cycleTime = Time.time * runnerSpeed * 0.72f;
            var cycle = Mathf.Sin(cycleTime);
            var footfall = Mathf.Abs(Mathf.Cos(cycleTime));
            AddMuscle("Spine Front-Back", Mathf.Lerp(0.025f, 0.15f, sprint));
            AddMuscle("Chest Front-Back", Mathf.Lerp(0.015f, 0.08f, sprint));
            AddMuscle("Spine Twist Left-Right", cycle * Mathf.Lerp(0.025f, 0.075f, sprint));
            AddMuscle("Chest Twist Left-Right", -cycle * Mathf.Lerp(0.035f, 0.10f, sprint));
            AddMuscle("Head Nod Down-Up", -footfall * 0.022f);
            AddMuscle("Left Shoulder Front-Back", -cycle * 0.045f);
            AddMuscle("Right Shoulder Front-Back", cycle * 0.045f);
            AddMuscle("Left Hand Down-Up", cycle * 0.025f);
            AddMuscle("Right Hand Down-Up", -cycle * 0.025f);

            var launch = 1f - _runBlend;
            AddMuscle("Spine Front-Back", launch * 0.18f);
            AddMuscle("Left Upper Leg Front-Back", launch * 0.12f);
            AddMuscle("Right Upper Leg Front-Back", -launch * 0.12f);
        }

        private void ApplyLaneChangePose(float lean, bool running)
        {
            AddMuscle("Spine Left-Right", -lean * 0.12f);
            AddMuscle("Chest Left-Right", -lean * 0.10f);
            AddMuscle("Spine Twist Left-Right", lean * 0.07f);
            AddMuscle("Head Tilt Left-Right", lean * 0.05f);
            if (!running) return;

            AddMuscle("Left Arm Down-Up", Mathf.Max(0f, lean) * 0.12f);
            AddMuscle("Right Arm Down-Up", Mathf.Max(0f, -lean) * 0.12f);
            AddMuscle("Left Upper Leg In-Out", -lean * 0.08f);
            AddMuscle("Right Upper Leg In-Out", -lean * 0.08f);
        }

        private void ApplyJumpPose(float verticalVelocity)
        {
            var rising = Mathf.Clamp01((verticalVelocity + 1.5f) / 8f);
            var falling = Mathf.Clamp01(-verticalVelocity / 8f);
            var hang = 1f - Mathf.Clamp01(Mathf.Abs(verticalVelocity) / 2.5f);
            AddMuscle("Spine Front-Back", -0.20f * rising + 0.18f * falling);
            AddMuscle("Chest Front-Back", -0.14f * rising + 0.10f * falling);
            AddMuscle("Head Nod Down-Up", 0.08f * rising - 0.08f * falling);
            AddMuscle("Left Arm Front-Back", -0.56f * rising - 0.16f * falling);
            AddMuscle("Right Arm Front-Back", -0.56f * rising - 0.16f * falling);
            AddMuscle("Left Arm Down-Up", hang * 0.18f);
            AddMuscle("Right Arm Down-Up", hang * 0.18f);
            AddMuscle("Left Forearm Stretch", -0.42f);
            AddMuscle("Right Forearm Stretch", -0.42f);
            AddMuscle("Left Upper Leg Front-Back", 0.48f * rising + 0.24f * falling);
            AddMuscle("Right Upper Leg Front-Back", 0.28f * rising + 0.42f * falling);
            AddMuscle("Left Lower Leg Stretch", -0.62f * rising - 0.38f * falling);
            AddMuscle("Right Lower Leg Stretch", -0.40f * rising - 0.58f * falling);
            AddMuscle("Left Foot Up-Down", -0.20f * rising + 0.18f * falling);
            AddMuscle("Right Foot Up-Down", -0.14f * rising + 0.20f * falling);
        }

        private void ApplySlidePose(float slideProgress, float blend)
        {
            var progress = Mathf.Clamp01(slideProgress);
            var entryShape = 1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress / 0.22f));
            var exitShape = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((progress - 0.78f) / 0.22f));
            var entry = blend * entryShape;
            var exit = blend * exitShape;
            var hold = blend * Mathf.Clamp01(1f - Mathf.Max(entryShape, exitShape));
            var glide = Mathf.Sin(progress * Mathf.PI) * hold;

            // Entry: compress quickly and throw both arms back to preserve forward momentum.
            AddMuscle("Spine Front-Back", 0.34f * entry);
            AddMuscle("Chest Front-Back", 0.20f * entry);
            AddMuscle("Head Nod Down-Up", -0.08f * entry);
            AddMuscle("Left Arm Front-Back", -0.30f * entry);
            AddMuscle("Right Arm Front-Back", -0.30f * entry);
            AddMuscle("Left Forearm Stretch", -0.24f * entry);
            AddMuscle("Right Forearm Stretch", -0.24f * entry);
            AddMuscle("Left Upper Leg Front-Back", 0.34f * entry);
            AddMuscle("Right Upper Leg Front-Back", 0.34f * entry);
            AddMuscle("Left Lower Leg Stretch", -0.68f * entry);
            AddMuscle("Right Lower Leg Stretch", -0.68f * entry);

            // Hold: a readable one-knee slide with the lead leg extended and arms balancing.
            AddMuscle("Spine Front-Back", 0.24f * hold);
            AddMuscle("Chest Front-Back", 0.12f * hold);
            AddMuscle("Spine Twist Left-Right", -0.16f * hold);
            AddMuscle("Chest Twist Left-Right", 0.12f * hold);
            AddMuscle("Head Nod Down-Up", -0.08f * hold);
            AddMuscle("Head Turn Left-Right", 0.06f * hold);
            AddMuscle("Left Arm Down-Up", 0.48f * hold);
            AddMuscle("Left Arm Front-Back", 0.30f * hold);
            AddMuscle("Left Forearm Stretch", -0.70f * hold);
            AddMuscle("Left Hand Down-Up", -0.18f * hold);
            AddMuscle("Right Arm Down-Up", 0.44f * hold);
            AddMuscle("Right Arm Front-Back", -0.30f * hold);
            AddMuscle("Right Forearm Stretch", -0.44f * hold);
            AddMuscle("Right Hand In-Out", 0.16f * hold);
            AddMuscle("Left Upper Leg Front-Back", 0.62f * hold);
            AddMuscle("Left Upper Leg In-Out", -0.08f * hold);
            AddMuscle("Left Lower Leg Stretch", -0.14f * hold);
            AddMuscle("Left Foot Up-Down", 0.28f * hold);
            AddMuscle("Right Upper Leg Front-Back", 0.48f * hold);
            AddMuscle("Right Upper Leg In-Out", 0.14f * hold);
            AddMuscle("Right Lower Leg Stretch", -0.90f * hold);
            AddMuscle("Right Foot Up-Down", 0.20f * hold);

            // Exit: plant the lead foot, bring the torso forward and push back into the run.
            AddMuscle("Spine Front-Back", 0.38f * exit);
            AddMuscle("Chest Front-Back", 0.22f * exit);
            AddMuscle("Left Arm Front-Back", 0.22f * exit);
            AddMuscle("Right Arm Front-Back", -0.22f * exit);
            AddMuscle("Left Forearm Stretch", -0.32f * exit);
            AddMuscle("Right Forearm Stretch", -0.32f * exit);
            AddMuscle("Left Upper Leg Front-Back", 0.46f * exit);
            AddMuscle("Right Upper Leg Front-Back", 0.30f * exit);
            AddMuscle("Left Lower Leg Stretch", -0.48f * exit);
            AddMuscle("Right Lower Leg Stretch", -0.60f * exit);
            AddMuscle("Left Foot Up-Down", 0.18f * exit);
            AddMuscle("Right Foot Up-Down", 0.12f * exit);

            _humanPose.bodyPosition += Vector3.down * (0.07f * entry + 0.13f * hold + 0.06f * exit + 0.015f * glide);
            _humanPose.bodyPosition += Vector3.forward * (0.025f * entry + 0.05f * hold + 0.02f * exit);
        }

        private void ApplyBoardPose(float runnerSpeed)
        {
            var balance = Mathf.Sin(Time.time * Mathf.Lerp(2.4f, 3.8f, Mathf.InverseLerp(10f, 24f, runnerSpeed)));
            AddMuscle("Spine Front-Back", 0.12f);
            AddMuscle("Spine Twist Left-Right", balance * 0.07f);
            AddMuscle("Chest Left-Right", balance * 0.06f);
            AddMuscle("Left Arm Down-Up", 0.56f);
            AddMuscle("Right Arm Down-Up", 0.56f);
            AddMuscle("Left Arm Front-Back", 0.10f);
            AddMuscle("Right Arm Front-Back", -0.10f);
            AddMuscle("Left Forearm Stretch", -0.18f);
            AddMuscle("Right Forearm Stretch", -0.18f);
            AddMuscle("Left Upper Leg Front-Back", 0.18f);
            AddMuscle("Right Upper Leg Front-Back", 0.10f);
            AddMuscle("Left Lower Leg Stretch", -0.32f);
            AddMuscle("Right Lower Leg Stretch", -0.32f);
            AddMuscle("Left Foot Up-Down", 0.08f);
            AddMuscle("Right Foot Up-Down", 0.08f);
            _humanPose.bodyPosition += Vector3.down * 0.035f;
        }

        private void ApplyLandingPose(float remainingRatio)
        {
            var amount = Mathf.Sin((1f - remainingRatio) * Mathf.PI);
            AddMuscle("Spine Front-Back", 0.38f * amount);
            AddMuscle("Chest Front-Back", 0.20f * amount);
            AddMuscle("Head Nod Down-Up", -0.10f * amount);
            AddMuscle("Left Arm Front-Back", 0.18f * amount);
            AddMuscle("Right Arm Front-Back", 0.18f * amount);
            AddMuscle("Left Upper Leg Front-Back", 0.36f * amount);
            AddMuscle("Right Upper Leg Front-Back", 0.36f * amount);
            AddMuscle("Left Lower Leg Stretch", -0.52f * amount);
            AddMuscle("Right Lower Leg Stretch", -0.52f * amount);
            _humanPose.bodyPosition += Vector3.down * 0.045f * amount;
        }

        private void ApplyStumblePose(float remainingRatio)
        {
            var wave = Mathf.Sin((1f - remainingRatio) * Mathf.PI * 2f);
            AddMuscle("Spine Front-Back", 0.38f);
            AddMuscle("Spine Twist Left-Right", wave * 0.36f);
            AddMuscle("Chest Left-Right", wave * 0.28f);
            AddMuscle("Left Arm Down-Up", 0.62f);
            AddMuscle("Right Arm Down-Up", 0.62f);
            AddMuscle("Left Arm Front-Back", -wave * 0.34f);
            AddMuscle("Right Arm Front-Back", wave * 0.34f);
            AddMuscle("Head Tilt Left-Right", -wave * 0.26f);
        }

        private void ApplyFallPose(float amount)
        {
            AddMuscle("Spine Front-Back", 0.66f * amount);
            AddMuscle("Chest Front-Back", 0.42f * amount);
            AddMuscle("Head Nod Down-Up", 0.34f * amount);
            AddMuscle("Left Arm Down-Up", 0.46f * amount);
            AddMuscle("Right Arm Down-Up", 0.46f * amount);
            AddMuscle("Left Forearm Stretch", -0.58f * amount);
            AddMuscle("Right Forearm Stretch", -0.58f * amount);
            AddMuscle("Left Upper Leg Front-Back", 0.32f * amount);
            AddMuscle("Right Upper Leg Front-Back", 0.20f * amount);
            AddMuscle("Left Lower Leg Stretch", -0.54f * amount);
            AddMuscle("Right Lower Leg Stretch", -0.42f * amount);
        }

        private void ApplyCelebratePose(float remainingRatio)
        {
            var progress = 1f - remainingRatio;
            var bounce = Mathf.Abs(Mathf.Sin(progress * Mathf.PI * 3f));
            AddMuscle("Spine Front-Back", -0.18f);
            AddMuscle("Chest Front-Back", -0.14f);
            AddMuscle("Left Arm Down-Up", 0.90f);
            AddMuscle("Right Arm Down-Up", 0.90f);
            AddMuscle("Left Forearm Stretch", -0.32f);
            AddMuscle("Right Forearm Stretch", -0.32f);
            AddMuscle("Left Hand In-Out", -0.24f);
            AddMuscle("Right Hand In-Out", 0.24f);
            AddMuscle("Head Turn Left-Right", Mathf.Sin(progress * Mathf.PI * 2f) * 0.20f);
            _humanPose.bodyPosition += Vector3.up * bounce * 0.03f;
        }

        private void BuildMuscleIndex()
        {
            _muscleIndices.Clear();
            for (var index = 0; index < HumanTrait.MuscleCount; index++)
            {
                _muscleIndices[HumanTrait.MuscleName[index]] = index;
            }
        }

        private void AddMuscle(string name, float value)
        {
            if (_humanPose.muscles == null || !_muscleIndices.TryGetValue(name, out var index)) return;
            _humanPose.muscles[index] += value;
        }

        private void ClampMuscles()
        {
            if (_humanPose.muscles == null) return;
            for (var index = 0; index < _humanPose.muscles.Length; index++)
            {
                _humanPose.muscles[index] = Mathf.Clamp(_humanPose.muscles[index], -1f, 1f);
            }
        }

        private void CreatePlayable(int index, AnimationClip clip, double speed)
        {
            _playables[index] = AnimationClipPlayable.Create(_graph, clip);
            _playables[index].SetApplyFootIK(true);
            _playables[index].SetApplyPlayableIK(false);
            _playables[index].SetSpeed(speed);
            _graph.Connect(_playables[index], 0, _mixer, index);
        }

        private static List<AnimationClip> CollectClips(string resourcePath, string motionFolder)
        {
            var clips = new List<AnimationClip>();
            AddUnique(clips, Resources.LoadAll<AnimationClip>(resourcePath));
            AddUnique(clips, Resources.LoadAll<AnimationClip>(motionFolder));
            return clips;
        }

        private static void AddUnique(List<AnimationClip> destination, AnimationClip[] source)
        {
            if (source == null) return;
            for (var index = 0; index < source.Length; index++)
            {
                if (source[index] != null && !destination.Contains(source[index])) destination.Add(source[index]);
            }
        }

        private static AnimationClip FindClip(List<AnimationClip> clips, params string[] names)
        {
            if (clips == null || names == null) return null;
            for (var pass = 0; pass < 2; pass++)
            for (var nameIndex = 0; nameIndex < names.Length; nameIndex++)
            for (var clipIndex = 0; clipIndex < clips.Count; clipIndex++)
            {
                var clip = clips[clipIndex];
                if (clip == null) continue;
                var matches = pass == 0
                    ? string.Equals(clip.name, names[nameIndex], StringComparison.OrdinalIgnoreCase)
                    : clip.name.IndexOf(names[nameIndex], StringComparison.OrdinalIgnoreCase) >= 0;
                if (matches) return clip;
            }
            return null;
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
            _poseHandler = null;
        }
    }
}
