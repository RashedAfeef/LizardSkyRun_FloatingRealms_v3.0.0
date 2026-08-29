using DesertDash.Core;
using DesertDash.World;
using UnityEngine;

namespace DesertDash.Player
{
    public sealed class RunnerCharacterVisual : MonoBehaviour
    {
        private const string MeshyHeroResource = "Characters/JordanianHero/JordanianHero_Meshy_Animated_Full";
        private const string MeshyTextureResource = "Characters/JordanianHero/JordanianHero_BaseColor";
        private const string MeshyMotionFolder = "Characters/JordanianHero/Motions";
        private const float MeshyHeroHeight = 2.16f;

        private GameManager _game;
        private CharacterController _controller;
        private JordanianHeroRig _rig;
        private Transform _meshyHeroPivot;
        private MeshyHeroAnimator _meshyAnimator;
        private Material _meshyMaterial;
        private GameObject _board;
        private GameObject _shieldBubble;
        private GameObject _magnetBadge;
        private ParticleSystem _dust;
        private bool _sliding;
        private bool _boardActive;
        private float _slideProgress;
        private float _phase;
        private float _impactPulse;
        private float _stumbleRemaining;
        private float _celebrationRemaining;
        private float _blinkRemaining;
        private float _blinkCooldown = 2.1f;
        private float _lastLanePosition;

        public void Initialize(GameManager game, RuntimeMaterialLibrary materials)
        {
            _game = game;
            _game.MissionCompleted += OnMissionCompleted;
            _controller = GetComponentInParent<CharacterController>();
            if (!TryBuildMeshyHero())
            {
                _rig = new JordanianHeroBuilder(materials).Build(transform);
                Debug.LogWarning("The supplied animated lizard could not be loaded. The articulated procedural fallback is active.");
            }
            BuildPowerUpVisuals(materials);
            BuildDust(materials);
            _lastLanePosition = transform.position.x;
        }

        private void LateUpdate()
        {
            if (_game == null)
            {
                return;
            }

            var running = _game.State == GameState.Running;
            var grounded = _controller != null && _controller.isGrounded;
            var velocity = _controller != null ? _controller.velocity : Vector3.zero;
            var speed = running ? Mathf.Max(1f, _game.CurrentSpeed) : 2f;
            var normalizedSpeed = running ? Mathf.InverseLerp(8f, 25f, speed) : 0f;
            _phase += Time.deltaTime * speed * (running ? 0.69f : 0.18f);

            var stride = running && grounded && !_sliding ? Mathf.Sin(_phase) : 0f;
            var oppositeStride = running && grounded && !_sliding ? Mathf.Sin(_phase + Mathf.PI * 0.5f) : 0f;
            var idleBreath = Mathf.Sin(Time.unscaledTime * 2.1f);
            var bob = running && grounded && !_sliding ? Mathf.Abs(oppositeStride) * Mathf.Lerp(0.040f, 0.065f, normalizedSpeed) : idleBreath * 0.010f;
            var lateralVelocity = (transform.position.x - _lastLanePosition) / Mathf.Max(Time.unscaledDeltaTime, 0.001f);
            _lastLanePosition = transform.position.x;
            var laneLean = running ? Mathf.Clamp(-lateralVelocity * 0.65f, -14f, 14f) : 0f;

            if (_meshyHeroPivot != null && _meshyAnimator != null)
            {
                AnimateMeshyHero(running, grounded, velocity, speed, normalizedSpeed, laneLean);
                AnimateImpact();
                UpdateDust(running, grounded, normalizedSpeed);
                return;
            }

            if (_rig == null || _rig.Root == null)
            {
                return;
            }

            AnimateRoot(running, grounded, stride, bob, laneLean);
            if (_game.State == GameState.GameOver)
            {
                PoseGameOver();
            }
            else if (_celebrationRemaining > 0f)
            {
                _celebrationRemaining -= Time.unscaledDeltaTime;
                PoseCelebration();
            }
            else if (_stumbleRemaining > 0f)
            {
                _stumbleRemaining -= Time.unscaledDeltaTime;
                PoseStumble(laneLean);
            }
            else if (_sliding)
            {
                PoseSliding(laneLean, SlidePoseWeight());
            }
            else if (running && !grounded)
            {
                PoseAirborne(velocity.y, laneLean);
            }
            else
            {
                PoseRunningOrIdle(stride, idleBreath, running, normalizedSpeed);
            }

            AnimateFace();
            AnimateKeffiyeh(stride, laneLean, velocity.y, running);
            AnimateAccessories(stride, running, normalizedSpeed);
            AnimateImpact();

            UpdateDust(running, grounded, normalizedSpeed);
        }

        private bool TryBuildMeshyHero()
        {
            var model = Resources.Load<GameObject>(MeshyHeroResource);
            if (model == null)
            {
                return false;
            }

            var pivotObject = new GameObject("LizardRunner_Meshy_Pivot");
            pivotObject.transform.SetParent(transform, false);
            _meshyHeroPivot = pivotObject.transform;

            var instance = Instantiate(model, _meshyHeroPivot, false);
            instance.name = "AnimatedLizardRunner";
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;

            AlignMeshyHeroForward(instance.transform);
            if (!FitMeshyHeroToRunner(instance.transform))
            {
                Destroy(pivotObject);
                _meshyHeroPivot = null;
                return false;
            }

            if (!ConfigureMeshyMaterials(instance))
            {
                Destroy(pivotObject);
                _meshyHeroPivot = null;
                return false;
            }
            var animator = instance.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                animator = instance.AddComponent<Animator>();
            }

            _meshyAnimator = instance.AddComponent<MeshyHeroAnimator>();
            if (!_meshyAnimator.Initialize(animator, MeshyHeroResource, MeshyMotionFolder))
            {
                Destroy(pivotObject);
                _meshyHeroPivot = null;
                _meshyAnimator = null;
                return false;
            }

            Debug.Log("Animated Meshy lizard loaded with Running, Run_03 and Walking animation clips.");
            return true;
        }

        private void AnimateMeshyHero(bool running, bool grounded, Vector3 velocity, float speed, float normalizedSpeed, float laneLean)
        {
            var gameOver = _game.State == GameState.GameOver;
            _meshyAnimator.Tick(running, grounded, _sliding, gameOver, _boardActive, speed, velocity.y, laneLean, _slideProgress);

            var forwardLean = running ? Mathf.Lerp(2.5f, 7f, normalizedSpeed) : 0f;
            var targetPosition = Vector3.zero;
            if (_sliding)
            {
                var slideWeight = SlidePoseWeight();
                forwardLean = Mathf.Lerp(forwardLean, 6f, slideWeight);
                targetPosition = Vector3.Lerp(Vector3.zero, new Vector3(0f, -0.27f, 0.18f), slideWeight);
            }
            else if (running && !grounded)
            {
                forwardLean = velocity.y > 0f ? -7f : 8f;
                targetPosition = new Vector3(0f, 0.03f, 0.02f);
            }
            else if (_boardActive && running)
            {
                forwardLean = 3f;
                targetPosition = new Vector3(0f, 0.10f, 0.02f);
            }
            else if (gameOver)
            {
                forwardLean = 68f;
                targetPosition = new Vector3(0f, -0.08f, 0.34f);
            }

            var fallRoll = gameOver ? 12f : 0f;
            var rotation = Quaternion.Euler(forwardLean, 0f, laneLean + fallRoll);
            _meshyHeroPivot.localPosition = Vector3.Lerp(_meshyHeroPivot.localPosition, targetPosition, 1f - Mathf.Exp(-16f * Time.deltaTime));
            _meshyHeroPivot.localRotation = Quaternion.Slerp(_meshyHeroPivot.localRotation, rotation, 1f - Mathf.Exp(-15f * Time.deltaTime));
        }

        private void AlignMeshyHeroForward(Transform instance)
        {
            var head = FindDescendant(instance, "Head");
            var headFront = FindDescendant(instance, "headfront");
            if (head == null || headFront == null)
            {
                return;
            }

            var modelForward = Vector3.ProjectOnPlane(headFront.position - head.position, Vector3.up);
            if (modelForward.sqrMagnitude < 0.0001f)
            {
                return;
            }

            var correction = Vector3.SignedAngle(modelForward, transform.forward, Vector3.up);
            instance.Rotate(0f, correction, 0f, Space.World);
        }

        private bool FitMeshyHeroToRunner(Transform instance)
        {
            var renderers = instance.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                return false;
            }

            var bounds = renderers[0].bounds;
            for (var index = 1; index < renderers.Length; index++)
            {
                bounds.Encapsulate(renderers[index].bounds);
            }

            if (bounds.size.y < 0.001f)
            {
                return false;
            }

            var scale = MeshyHeroHeight / bounds.size.y;
            instance.localScale *= scale;

            bounds = renderers[0].bounds;
            for (var index = 1; index < renderers.Length; index++)
            {
                bounds.Encapsulate(renderers[index].bounds);
            }

            var desiredCenter = transform.position + Vector3.up * (MeshyHeroHeight * 0.5f);
            instance.position += desiredCenter - bounds.center;
            return true;
        }

        private bool ConfigureMeshyMaterials(GameObject instance)
        {
            var texture = Resources.Load<Texture2D>(MeshyTextureResource);
            if (texture == null)
            {
                Debug.LogError($"Meshy hero base-color texture was not found at Resources/{MeshyTextureResource}.png");
                return false;
            }

            var shader = Resources.Load<Shader>("Shaders/JordanianHeroToon");
            if (shader == null)
            {
                shader = Shader.Find("DesertDash/Lizard Runner Toon");
            }

            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Lit");
            }
            if (shader == null)
            {
                shader = Shader.Find("HDRP/Lit");
            }

            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                shader = Shader.Find("Diffuse");
            }

            if (shader == null)
            {
                Debug.LogError("No compatible Lit shader was found for the Meshy hero material.");
                return false;
            }

            _meshyMaterial = new Material(shader) { name = "LizardRunner_Meshy_Runtime" };
            SetTexture(_meshyMaterial, "_BaseMap", texture);
            SetTexture(_meshyMaterial, "_BaseColorMap", texture);
            SetTexture(_meshyMaterial, "_MainTex", texture);
            SetColor(_meshyMaterial, "_BaseColor", Color.white);
            SetColor(_meshyMaterial, "_Color", Color.white);
            SetColor(_meshyMaterial, "_ShadowColor", new Color(0.16f, 0.22f, 0.34f, 1f));
            SetColor(_meshyMaterial, "_RimColor", new Color(0.34f, 0.78f, 0.92f, 1f));
            SetFloat(_meshyMaterial, "_Saturation", 1.34f);
            SetFloat(_meshyMaterial, "_Contrast", 1.08f);
            SetFloat(_meshyMaterial, "_Brightness", 1.04f);
            SetFloat(_meshyMaterial, "_LightSteps", 3f);
            SetFloat(_meshyMaterial, "_RimPower", 3.6f);
            SetFloat(_meshyMaterial, "_RimStrength", 0.24f);
            SetFloat(_meshyMaterial, "_Metallic", 0f);
            SetFloat(_meshyMaterial, "_Smoothness", 0.16f);
            SetFloat(_meshyMaterial, "_Glossiness", 0.16f);

            var renderers = instance.GetComponentsInChildren<Renderer>(true);
            for (var rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
            {
                var renderer = renderers[rendererIndex];
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;

                var materials = renderer.sharedMaterials;
                for (var materialIndex = 0; materialIndex < materials.Length; materialIndex++)
                {
                    materials[materialIndex] = _meshyMaterial;
                }

                renderer.sharedMaterials = materials;
            }

            return true;
        }

        private static void SetTexture(Material material, string property, Texture texture)
        {
            if (material.HasProperty(property))
            {
                material.SetTexture(property, texture);
            }
        }

        private static void SetColor(Material material, string property, Color color)
        {
            if (material.HasProperty(property))
            {
                material.SetColor(property, color);
            }
        }

        private static void SetFloat(Material material, string property, float value)
        {
            if (material.HasProperty(property))
            {
                material.SetFloat(property, value);
            }
        }

        private static Transform FindDescendant(Transform root, string name)
        {
            var descendants = root.GetComponentsInChildren<Transform>(true);
            for (var index = 0; index < descendants.Length; index++)
            {
                if (string.Equals(descendants[index].name, name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return descendants[index];
                }
            }

            return null;
        }

        public void SetSliding(bool sliding)
        {
            if (sliding && !_sliding)
            {
                _slideProgress = 0f;
            }

            _sliding = sliding;
        }

        public void SetSlideProgress(float progress)
        {
            _slideProgress = Mathf.Clamp01(progress);
        }

        public void SetBoardActive(bool active)
        {
            _boardActive = active;
            if (_board != null)
            {
                _board.SetActive(active);
            }

            if (_shieldBubble != null)
            {
                _shieldBubble.SetActive(active);
            }
        }

        public void SetMagnetActive(bool active)
        {
            if (_magnetBadge != null)
            {
                _magnetBadge.SetActive(active);
            }
        }

        public void PulseImpact()
        {
            _impactPulse = 0.36f;
            _stumbleRemaining = 0.48f;
            if (_meshyAnimator != null)
            {
                _meshyAnimator.TriggerStumble();
            }
        }

        private void OnMissionCompleted()
        {
            _celebrationRemaining = 2.4f;
            if (_meshyAnimator != null)
            {
                _meshyAnimator.TriggerCelebrate();
            }
        }

        private void OnDestroy()
        {
            if (_game != null)
            {
                _game.MissionCompleted -= OnMissionCompleted;
            }

            if (_meshyMaterial != null)
            {
                Destroy(_meshyMaterial);
            }
        }

        private void AnimateRoot(bool running, bool grounded, float stride, float bob, float laneLean)
        {
            var slideWeight = SlidePoseWeight();
            var normalPosition = new Vector3(0f, bob, 0f);
            var targetPosition = Vector3.Lerp(normalPosition, new Vector3(0f, -0.26f, 0.34f), slideWeight);
            var runningLean = running ? Mathf.Lerp(4f, 8f, Mathf.InverseLerp(10f, 24f, _game.CurrentSpeed)) : 0f;
            var forwardLean = Mathf.Lerp(runningLean, 22f, slideWeight);
            var targetRotation = Quaternion.Euler(forwardLean, 0f, laneLean);
            _rig.Root.localPosition = Vector3.Lerp(_rig.Root.localPosition, targetPosition, 16f * Time.deltaTime);
            _rig.Root.localRotation = Quaternion.Slerp(_rig.Root.localRotation, targetRotation, 15f * Time.deltaTime);

            if (_rig.Pelvis != null)
            {
                var pelvisYaw = running && grounded && !_sliding ? stride * 5f : 0f;
                SetRotation(_rig.Pelvis, Quaternion.Euler(0f, pelvisYaw, -laneLean * 0.14f), 13f);
            }
        }

        private void PoseRunningOrIdle(float stride, float idleBreath, bool running, float normalizedSpeed)
        {
            var armSwing = running ? stride * Mathf.Lerp(46f, 62f, normalizedSpeed) : Mathf.Sin(Time.unscaledTime * 1.5f) * 2.5f;
            var legSwing = running ? stride * Mathf.Lerp(40f, 51f, normalizedSpeed) : 0f;
            var leftKnee = running ? Mathf.Max(0f, legSwing) * 0.74f + Mathf.Max(0f, -stride) * 8f : 0f;
            var rightKnee = running ? Mathf.Max(0f, -legSwing) * 0.74f + Mathf.Max(0f, stride) * 8f : 0f;

            SetRotation(_rig.Spine, Quaternion.Euler(running ? 2f : idleBreath * 0.8f, -stride * 3.5f, 0f), 13f);
            SetRotation(_rig.Chest, Quaternion.Euler(running ? -1f : idleBreath * -0.6f, stride * 6f, 0f), 13f);
            SetRotation(_rig.Neck, Quaternion.Euler(0f, -stride * 1.6f, 0f), 12f);
            SetRotation(_rig.Head, Quaternion.Euler(running ? -2f : idleBreath * 0.5f, stride * -1.7f, 0f), 12f);

            SetRotation(_rig.LeftUpperArm, Quaternion.Euler(armSwing, 0f, 7f), 16f);
            SetRotation(_rig.RightUpperArm, Quaternion.Euler(-armSwing, 0f, -7f), 16f);
            SetRotation(_rig.LeftForearm, Quaternion.Euler(-18f - Mathf.Max(0f, -armSwing) * 0.48f, 0f, 2f), 17f);
            SetRotation(_rig.RightForearm, Quaternion.Euler(-18f - Mathf.Max(0f, armSwing) * 0.48f, 0f, -2f), 17f);
            SetRotation(_rig.LeftHand, Quaternion.Euler(5f + stride * 4f, -stride * 5f, 0f), 15f);
            SetRotation(_rig.RightHand, Quaternion.Euler(5f - stride * 4f, stride * 5f, 0f), 15f);

            SetRotation(_rig.LeftUpperLeg, Quaternion.Euler(-legSwing, 0f, 1.5f), 17f);
            SetRotation(_rig.RightUpperLeg, Quaternion.Euler(legSwing, 0f, -1.5f), 17f);
            SetRotation(_rig.LeftCalf, Quaternion.Euler(leftKnee, 0f, 0f), 18f);
            SetRotation(_rig.RightCalf, Quaternion.Euler(rightKnee, 0f, 0f), 18f);
            SetRotation(_rig.LeftFoot, Quaternion.Euler(Mathf.Clamp(legSwing * 0.34f - leftKnee * 0.28f, -18f, 22f), 0f, 0f), 18f);
            SetRotation(_rig.RightFoot, Quaternion.Euler(Mathf.Clamp(-legSwing * 0.34f - rightKnee * 0.28f, -18f, 22f), 0f, 0f), 18f);

            if (!running)
            {
                var breathingScale = 1f + idleBreath * 0.010f;
                _rig.Chest.localScale = Vector3.Lerp(_rig.Chest.localScale, new Vector3(breathingScale, breathingScale, breathingScale), 5f * Time.unscaledDeltaTime);
            }
            else
            {
                _rig.Chest.localScale = Vector3.Lerp(_rig.Chest.localScale, Vector3.one, 8f * Time.deltaTime);
            }
        }

        private void PoseAirborne(float verticalVelocity, float laneLean)
        {
            var rising = verticalVelocity > 0f;
            SetRotation(_rig.Spine, Quaternion.Euler(rising ? -5f : 8f, 0f, -laneLean * 0.08f), 14f);
            SetRotation(_rig.Chest, Quaternion.Euler(rising ? -7f : 4f, 0f, 0f), 14f);
            SetRotation(_rig.Head, Quaternion.Euler(rising ? 3f : -4f, 0f, 0f), 14f);
            SetRotation(_rig.LeftUpperArm, Quaternion.Euler(rising ? -58f : -12f, 0f, 22f), 16f);
            SetRotation(_rig.RightUpperArm, Quaternion.Euler(rising ? -58f : -12f, 0f, -22f), 16f);
            SetRotation(_rig.LeftForearm, Quaternion.Euler(-42f, 0f, 0f), 16f);
            SetRotation(_rig.RightForearm, Quaternion.Euler(-42f, 0f, 0f), 16f);
            SetRotation(_rig.LeftHand, Quaternion.Euler(-12f, 0f, 8f), 16f);
            SetRotation(_rig.RightHand, Quaternion.Euler(-12f, 0f, -8f), 16f);
            SetRotation(_rig.LeftUpperLeg, Quaternion.Euler(30f, 0f, 4f), 16f);
            SetRotation(_rig.RightUpperLeg, Quaternion.Euler(-20f, 0f, -4f), 16f);
            SetRotation(_rig.LeftCalf, Quaternion.Euler(48f, 0f, 0f), 16f);
            SetRotation(_rig.RightCalf, Quaternion.Euler(30f, 0f, 0f), 16f);
            SetRotation(_rig.LeftFoot, Quaternion.Euler(-12f, 0f, 0f), 16f);
            SetRotation(_rig.RightFoot, Quaternion.Euler(14f, 0f, 0f), 16f);
        }

        private void PoseSliding(float laneLean, float slideWeight)
        {
            var sharpness = Mathf.Lerp(12f, 24f, slideWeight);
            SetRotation(_rig.Spine, Quaternion.Euler(-12f, 9f, -laneLean * 0.10f), sharpness);
            SetRotation(_rig.Chest, Quaternion.Euler(-7f, -12f, 3f), sharpness);
            SetRotation(_rig.Head, Quaternion.Euler(14f, 4f, 0f), sharpness);
            SetRotation(_rig.LeftUpperArm, Quaternion.Euler(-26f, -10f, 42f), sharpness);
            SetRotation(_rig.RightUpperArm, Quaternion.Euler(14f, 8f, -45f), sharpness);
            SetRotation(_rig.LeftForearm, Quaternion.Euler(-68f, 0f, 0f), sharpness);
            SetRotation(_rig.RightForearm, Quaternion.Euler(-42f, 0f, 0f), sharpness);
            SetRotation(_rig.LeftHand, Quaternion.Euler(-16f, 0f, 14f), sharpness);
            SetRotation(_rig.RightHand, Quaternion.Euler(5f, 0f, -18f), sharpness);
            SetRotation(_rig.LeftUpperLeg, Quaternion.Euler(38f, -5f, 8f), sharpness);
            SetRotation(_rig.RightUpperLeg, Quaternion.Euler(70f, 8f, -10f), sharpness);
            SetRotation(_rig.LeftCalf, Quaternion.Euler(-12f, 0f, 0f), sharpness);
            SetRotation(_rig.RightCalf, Quaternion.Euler(-92f, 0f, 0f), sharpness);
            SetRotation(_rig.LeftFoot, Quaternion.Euler(28f, 0f, 0f), sharpness);
            SetRotation(_rig.RightFoot, Quaternion.Euler(24f, 0f, 0f), sharpness);
        }

        private void PoseStumble(float laneLean)
        {
            var kick = Mathf.Sin(Mathf.InverseLerp(0.48f, 0f, _stumbleRemaining) * Mathf.PI);
            SetRotation(_rig.Root, Quaternion.Euler(20f * kick, 0f, laneLean + 12f * kick), 24f);
            SetRotation(_rig.Spine, Quaternion.Euler(18f, -8f, -10f), 22f);
            SetRotation(_rig.Chest, Quaternion.Euler(13f, 10f, 8f), 22f);
            SetRotation(_rig.Head, Quaternion.Euler(-16f, -6f, -7f), 22f);
            SetRotation(_rig.LeftUpperArm, Quaternion.Euler(-62f, 0f, 48f), 24f);
            SetRotation(_rig.RightUpperArm, Quaternion.Euler(-35f, 0f, -55f), 24f);
            SetRotation(_rig.LeftForearm, Quaternion.Euler(-70f, 0f, 0f), 24f);
            SetRotation(_rig.RightForearm, Quaternion.Euler(-58f, 0f, 0f), 24f);
            SetRotation(_rig.LeftUpperLeg, Quaternion.Euler(25f, 0f, 8f), 24f);
            SetRotation(_rig.RightUpperLeg, Quaternion.Euler(-34f, 0f, -6f), 24f);
            SetRotation(_rig.LeftCalf, Quaternion.Euler(38f, 0f, 0f), 24f);
            SetRotation(_rig.RightCalf, Quaternion.Euler(22f, 0f, 0f), 24f);
        }

        private void PoseCelebration()
        {
            var cheer = Mathf.Sin(Time.unscaledTime * 8.5f);
            var bounce = Mathf.Max(0f, cheer) * 0.10f;
            _rig.Root.localPosition = Vector3.Lerp(_rig.Root.localPosition, new Vector3(0f, bounce, 0f), 18f * Time.unscaledDeltaTime);
            SetRotation(_rig.Spine, Quaternion.Euler(-5f, cheer * 5f, 0f), 18f);
            SetRotation(_rig.Chest, Quaternion.Euler(-9f, -cheer * 8f, 0f), 18f);
            SetRotation(_rig.Head, Quaternion.Euler(-7f, cheer * 9f, cheer * 4f), 18f);
            SetRotation(_rig.LeftUpperArm, Quaternion.Euler(-155f, 0f, 24f), 22f);
            SetRotation(_rig.RightUpperArm, Quaternion.Euler(-155f, 0f, -24f), 22f);
            SetRotation(_rig.LeftForearm, Quaternion.Euler(-18f, 0f, -12f), 22f);
            SetRotation(_rig.RightForearm, Quaternion.Euler(-18f, 0f, 12f), 22f);
            SetRotation(_rig.LeftHand, Quaternion.Euler(cheer * 14f, 0f, 8f), 22f);
            SetRotation(_rig.RightHand, Quaternion.Euler(-cheer * 14f, 0f, -8f), 22f);
            SetRotation(_rig.LeftUpperLeg, Quaternion.Euler(-cheer * 15f, 0f, 4f), 20f);
            SetRotation(_rig.RightUpperLeg, Quaternion.Euler(cheer * 15f, 0f, -4f), 20f);
            SetRotation(_rig.LeftCalf, Quaternion.Euler(Mathf.Max(0f, cheer) * 25f, 0f, 0f), 20f);
            SetRotation(_rig.RightCalf, Quaternion.Euler(Mathf.Max(0f, -cheer) * 25f, 0f, 0f), 20f);
        }

        private void PoseGameOver()
        {
            _rig.Root.localPosition = Vector3.Lerp(_rig.Root.localPosition, new Vector3(0f, -0.18f, 0.34f), 7f * Time.unscaledDeltaTime);
            SetRotation(_rig.Root, Quaternion.Euler(72f, 0f, 14f), 8f);
            SetRotation(_rig.Spine, Quaternion.Euler(22f, 0f, 0f), 10f);
            SetRotation(_rig.Chest, Quaternion.Euler(18f, 0f, 0f), 10f);
            SetRotation(_rig.Head, Quaternion.Euler(-20f, 0f, -8f), 10f);
            SetRotation(_rig.LeftUpperArm, Quaternion.Euler(-24f, 0f, 55f), 10f);
            SetRotation(_rig.RightUpperArm, Quaternion.Euler(-38f, 0f, -42f), 10f);
            SetRotation(_rig.LeftForearm, Quaternion.Euler(-42f, 0f, 0f), 10f);
            SetRotation(_rig.RightForearm, Quaternion.Euler(-35f, 0f, 0f), 10f);
            SetRotation(_rig.LeftUpperLeg, Quaternion.Euler(42f, 0f, 9f), 10f);
            SetRotation(_rig.RightUpperLeg, Quaternion.Euler(18f, 0f, -8f), 10f);
            SetRotation(_rig.LeftCalf, Quaternion.Euler(-58f, 0f, 0f), 10f);
            SetRotation(_rig.RightCalf, Quaternion.Euler(-30f, 0f, 0f), 10f);
        }

        private float SlidePoseWeight()
        {
            if (!_sliding)
            {
                return 0f;
            }

            var entry = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(_slideProgress / 0.16f));
            var exit = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((1f - _slideProgress) / 0.18f));
            return Mathf.Min(entry, exit);
        }

        private void AnimateFace()
        {
            _blinkCooldown -= Time.unscaledDeltaTime;
            if (_blinkRemaining <= 0f && _blinkCooldown <= 0f)
            {
                _blinkRemaining = 0.18f;
                _blinkCooldown = 2.7f + Mathf.PingPong(Time.unscaledTime * 0.37f, 1.8f);
            }

            var blinkAmount = 0f;
            if (_blinkRemaining > 0f)
            {
                _blinkRemaining -= Time.unscaledDeltaTime;
                var normalized = 1f - Mathf.Clamp01(_blinkRemaining / 0.18f);
                blinkAmount = Mathf.Sin(normalized * Mathf.PI);
            }

            ApplyBlink(_rig.LeftUpperEyelid, _rig.LeftUpperEyelidOpenPosition, blinkAmount);
            ApplyBlink(_rig.RightUpperEyelid, _rig.RightUpperEyelidOpenPosition, blinkAmount);

            if (_rig.Jaw != null)
            {
                var exertion = _game.State == GameState.Running ? Mathf.Lerp(0.5f, 2.0f, Mathf.InverseLerp(10f, 24f, _game.CurrentSpeed)) : 0f;
                SetRotation(_rig.Jaw, Quaternion.Euler(exertion, 0f, 0f), 8f);
            }

            if (_rig.HairFront != null)
            {
                var hairSway = Mathf.Sin(_phase * 1.3f) * (_game.State == GameState.Running ? 3f : 0.8f);
                SetRotation(_rig.HairFront, Quaternion.Euler(hairSway, 0f, -hairSway * 0.3f), 9f);
            }
        }

        private static void ApplyBlink(Transform eyelid, Vector3 openPosition, float amount)
        {
            if (eyelid == null)
            {
                return;
            }

            eyelid.localPosition = Vector3.Lerp(openPosition, openPosition + Vector3.down * 0.074f, amount);
            eyelid.localScale = new Vector3(0.17f, Mathf.Lerp(0.055f, 0.19f, amount), 0.042f);
        }

        private void AnimateKeffiyeh(float stride, float laneLean, float verticalVelocity, bool running)
        {
            var wind = running ? Mathf.Lerp(5f, 17f, Mathf.InverseLerp(10f, 24f, _game.CurrentSpeed)) : 2f;
            var lift = Mathf.Clamp(verticalVelocity * 1.3f, -8f, 13f);
            var wave = Mathf.Sin(_phase * 0.78f) * wind * 0.26f;

            SetRotation(_rig.ScarfLeftRoot, Quaternion.Euler(8f - lift, -laneLean * 0.20f, -9f + wave), 8f);
            SetRotation(_rig.ScarfRightRoot, Quaternion.Euler(10f - lift, -laneLean * 0.24f, 10f - wave), 8f);

            for (var i = 0; i < _rig.ScarfLeftSegments.Count; i++)
            {
                var delayedWave = Mathf.Sin(_phase * 0.82f - i * 0.75f) * (4f + i * 1.8f);
                SetRotation(_rig.ScarfLeftSegments[i], Quaternion.Euler(wind * 0.33f + i * 2.2f, delayedWave * 0.42f, delayedWave), 7f + i);
            }

            for (var i = 0; i < _rig.ScarfRightSegments.Count; i++)
            {
                var delayedWave = Mathf.Sin(_phase * 0.82f - i * 0.75f + 1.35f) * (4f + i * 1.8f);
                SetRotation(_rig.ScarfRightSegments[i], Quaternion.Euler(wind * 0.38f + i * 2.5f, -delayedWave * 0.42f, delayedWave), 7f + i);
            }
        }

        private void AnimateAccessories(float stride, bool running, float normalizedSpeed)
        {
            if (_rig.CrossBodyBag != null)
            {
                var bounce = running ? Mathf.Abs(Mathf.Sin(_phase)) * Mathf.Lerp(0.018f, 0.045f, normalizedSpeed) : 0f;
                var sway = running ? stride * Mathf.Lerp(4f, 9f, normalizedSpeed) : Mathf.Sin(Time.unscaledTime * 1.2f) * 1.3f;
                _rig.CrossBodyBag.localPosition = Vector3.Lerp(_rig.CrossBodyBag.localPosition, new Vector3(0.38f, -0.035f + bounce, 0.29f), 12f * Time.deltaTime);
                SetRotation(_rig.CrossBodyBag, Quaternion.Euler(0f, sway * 0.30f, -sway), 10f);
            }

            if (_rig.BagFlap != null)
            {
                var flap = running ? Mathf.Abs(Mathf.Sin(_phase * 0.92f)) * 5f : 0f;
                SetRotation(_rig.BagFlap, Quaternion.Euler(-flap, 0f, 0f), 9f);
            }

            if (_rig.WatchScreen != null)
            {
                var pulse = 1f + Mathf.Sin(Time.unscaledTime * 3.2f) * 0.025f;
                _rig.WatchScreen.localScale = new Vector3(0.78f * pulse, 0.72f * pulse, 0.06f);
            }
        }

        private void AnimateImpact()
        {
            var target = _meshyHeroPivot != null ? _meshyHeroPivot : _rig != null ? _rig.Root : null;
            if (target == null)
            {
                return;
            }

            if (_impactPulse > 0f)
            {
                _impactPulse -= Time.unscaledDeltaTime;
                var pulse = 1f + Mathf.Sin(_impactPulse * 52f) * 0.055f;
                target.localScale = Vector3.one * pulse;
            }
            else
            {
                target.localScale = Vector3.Lerp(target.localScale, Vector3.one, 13f * Time.unscaledDeltaTime);
            }
        }

        private void UpdateDust(bool running, bool grounded, float normalizedSpeed)
        {
            if (_dust == null)
            {
                return;
            }

            var emission = _dust.emission;
            emission.enabled = running && grounded;
            emission.rateOverTime = Mathf.Lerp(18f, 32f, normalizedSpeed);
        }

        private void BuildPowerUpVisuals(RuntimeMaterialLibrary materials)
        {
            _board = new GameObject("AetherPulseBoard");
            _board.transform.SetParent(transform, false);
            CreatePrimitive(PrimitiveType.Cube, "BoardDeck", _board.transform, new Vector3(0f, 0.08f, 0.08f), new Vector3(0.80f, 0.08f, 1.78f), materials.RuneViolet);
            CreatePrimitive(PrimitiveType.Cube, "BoardCyanStripe", _board.transform, new Vector3(0f, 0.035f, 0.08f), new Vector3(0.42f, 0.04f, 1.90f), materials.RuneCyan);
            CreatePrimitive(PrimitiveType.Cube, "BoardStarStripe", _board.transform, new Vector3(0f, 0.105f, 0.08f), new Vector3(0.16f, 0.025f, 1.72f), materials.StarGold);

            _shieldBubble = CreatePrimitive(PrimitiveType.Sphere, "ShieldBubble", transform, new Vector3(0f, 1.16f, 0f), Vector3.one * 2.78f, materials.Shield);

            _magnetBadge = new GameObject("MagnetField");
            _magnetBadge.transform.SetParent(transform, false);
            _magnetBadge.transform.localPosition = new Vector3(0f, 1.40f, -0.34f);
            CreatePrimitive(PrimitiveType.Cube, "MagnetLeft", _magnetBadge.transform, new Vector3(-0.25f, 0f, 0f), new Vector3(0.20f, 0.62f, 0.16f), materials.Magnet);
            CreatePrimitive(PrimitiveType.Cube, "MagnetRight", _magnetBadge.transform, new Vector3(0.25f, 0f, 0f), new Vector3(0.20f, 0.62f, 0.16f), materials.Magnet);
            CreatePrimitive(PrimitiveType.Cube, "MagnetBridge", _magnetBadge.transform, new Vector3(0f, -0.25f, 0f), new Vector3(0.50f, 0.16f, 0.16f), materials.White);

            _board.SetActive(false);
            _shieldBubble.SetActive(false);
            _magnetBadge.SetActive(false);
        }

        private void BuildDust(RuntimeMaterialLibrary materials)
        {
            var dustObject = new GameObject("FootDust");
            dustObject.transform.SetParent(transform, false);
            dustObject.transform.localPosition = new Vector3(0f, 0.08f, -0.38f);
            _dust = dustObject.AddComponent<ParticleSystem>();
            var main = _dust.main;
            main.loop = true;
            main.playOnAwake = true;
            main.startLifetime = 0.46f;
            main.startSpeed = 0.72f;
            main.startSize = 0.20f;
            main.maxParticles = 100;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = _dust.emission;
            emission.rateOverTime = 24f;
            var shape = _dust.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(0.70f, 0.05f, 0.20f);
            var renderer = dustObject.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = materials.Dust;
        }

        private static void SetRotation(Transform target, Quaternion rotation, float sharpness)
        {
            if (target != null)
            {
                target.localRotation = Quaternion.Slerp(target.localRotation, rotation, 1f - Mathf.Exp(-sharpness * Time.deltaTime));
            }
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
                Destroy(collider);
            }

            var renderer = primitive.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;
            return primitive;
        }
    }
}
