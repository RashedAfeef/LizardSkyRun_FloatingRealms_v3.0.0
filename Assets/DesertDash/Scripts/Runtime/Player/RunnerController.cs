using DesertDash.Audio;
using DesertDash.Core;
using DesertDash.Input;
using DesertDash.World;
using UnityEngine;

namespace DesertDash.Player
{
    [RequireComponent(typeof(CharacterController), typeof(RunnerInput))]
    public sealed class RunnerController : MonoBehaviour
    {
        private CharacterController _controller;
        private RunnerInput _input;
        private GameManager _game;
        private GameAudio _audio;
        private RunnerConfig _config;
        private readonly Collider[] _magnetHits = new Collider[64];
        private readonly Collider[] _standingHits = new Collider[12];
        private RunnerCharacterVisual _visual;
        private int _targetLane;
        private float _verticalVelocity;
        private float _slideRemaining;
        private float _slideElapsed;
        private float _shieldRemaining;
        private float _magnetRemaining;
        private float _normalHeight;
        private Vector3 _normalCenter;
        private bool _isSliding;

        public float ShieldRemaining => Mathf.Max(0f, _shieldRemaining);
        public bool HasShield => _shieldRemaining > 0f;
        public float MagnetRemaining => Mathf.Max(0f, _magnetRemaining);
        public float ScoreBoostRemaining => _game == null ? 0f : _game.ScoreBoostRemaining;
        public bool IsSliding => _isSliding;

        public void Initialize(GameManager game, GameAudio gameAudio, RunnerConfig config, RunnerCharacterVisual visual)
        {
            _game = game;
            _audio = gameAudio;
            _config = config;
            _visual = visual;
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<RunnerInput>();
            _input.Initialize(config);
            _normalHeight = _controller.height;
            _normalCenter = _controller.center;
        }

        private void Update()
        {
            if (_input.PausePressed)
            {
                _game.TogglePause();
            }

            if (_game.State != GameState.Running)
            {
                return;
            }

            TickPowerUps();
            AttractNearbyCoins();
            ReadCommands();
            MoveRunner();
        }

        private void TickPowerUps()
        {
            if (_shieldRemaining > 0f)
            {
                _shieldRemaining -= Time.deltaTime;
                if (_shieldRemaining <= 0f)
                {
                    _visual.SetBoardActive(false);
                }
            }

            if (_magnetRemaining > 0f)
            {
                _magnetRemaining -= Time.deltaTime;
                if (_magnetRemaining <= 0f)
                {
                    _visual.SetMagnetActive(false);
                }
            }

            if (!_isSliding)
            {
                return;
            }

            _slideElapsed += Time.deltaTime;
            _slideRemaining -= Time.deltaTime;
            _visual.SetSlideProgress(Mathf.Clamp01(_slideElapsed / Mathf.Max(0.01f, _config.slideDuration)));
            if (_slideRemaining <= 0f)
            {
                if (CanStandUp())
                {
                    EndSlide();
                }
                else
                {
                    _slideRemaining = 0.08f;
                }
            }
        }

        private void ReadCommands()
        {
            if (_input.LaneDelta != 0)
            {
                _targetLane = RunnerMath.ClampLane(_targetLane + _input.LaneDelta);
            }

            if (_input.JumpPressed && _controller.isGrounded && !_isSliding)
            {
                _verticalVelocity = Mathf.Sqrt(_config.jumpHeight * -2f * _config.gravity);
                _audio.PlayJump();
            }

            if (_input.SlidePressed && _controller.isGrounded)
            {
                BeginSlide();
            }
        }

        private void MoveRunner()
        {
            if (_controller.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            _verticalVelocity += _config.gravity * Time.deltaTime;
            var targetX = _targetLane * _config.laneSpacing;
            var nextX = Mathf.Lerp(transform.position.x, targetX, 1f - Mathf.Exp(-_config.laneChangeSharpness * Time.deltaTime));
            var displacement = new Vector3(nextX - transform.position.x, _verticalVelocity * Time.deltaTime, _game.CurrentSpeed * Time.deltaTime);
            _controller.Move(displacement);
        }

        private void BeginSlide()
        {
            _slideRemaining = _config.slideDuration;
            _slideElapsed = 0f;
            if (_isSliding)
            {
                _visual.SetSlideProgress(0f);
                return;
            }

            _isSliding = true;
            _controller.height = 0.86f;
            _controller.center = new Vector3(_normalCenter.x, 0.43f, _normalCenter.z);
            _visual.SetSliding(true);
            _visual.SetSlideProgress(0f);
            _audio.PlaySlide();
        }

        private void EndSlide()
        {
            _isSliding = false;
            _visual.SetSlideProgress(1f);
            _controller.height = _normalHeight;
            _controller.center = _normalCenter;
            _visual.SetSliding(false);
        }

        private bool CanStandUp()
        {
            var radius = Mathf.Max(0.05f, _controller.radius - _controller.skinWidth);
            var worldCenter = transform.TransformPoint(_normalCenter);
            var halfSegment = Mathf.Max(0f, _normalHeight * 0.5f - radius);
            var top = worldCenter + transform.up * halfSegment;
            var bottom = worldCenter - transform.up * halfSegment;
            var count = Physics.OverlapCapsuleNonAlloc(top, bottom, radius, _standingHits, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            for (var index = 0; index < count; index++)
            {
                var hit = _standingHits[index];
                if (hit == null || hit == _controller || hit.transform.IsChildOf(transform))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            var hazard = hit.collider.GetComponentInParent<Hazard>();
            if (hazard == null || _game.State != GameState.Running)
            {
                return;
            }

            if (HasShield)
            {
                _shieldRemaining = 0f;
                _visual.SetBoardActive(false);
                _visual.PulseImpact();
                _audio.PlayShieldBreak();
                hazard.gameObject.SetActive(false);
                return;
            }

            _visual.PulseImpact();
            if (_game.VibrationEnabled && Application.isMobilePlatform)
            {
                Handheld.Vibrate();
            }

            _game.EndRun();
        }

        private void OnTriggerEnter(Collider other)
        {
            var coin = other.GetComponentInParent<CoinPickup>();
            if (coin != null)
            {
                coin.TryCollect(_game);
                return;
            }

            var shield = other.GetComponentInParent<ShieldPickup>();
            if (shield != null && shield.TryCollect())
            {
                _shieldRemaining = _config.shieldDuration;
                _visual.SetBoardActive(true);
                _audio.PlayShield();
                return;
            }

            var magnet = other.GetComponentInParent<CoinMagnetPickup>();
            if (magnet != null && magnet.TryCollect())
            {
                _magnetRemaining = Mathf.Max(_magnetRemaining, _config.magnetDuration);
                _visual.SetMagnetActive(true);
                _audio.PlayPowerUp();
                return;
            }

            var boost = other.GetComponentInParent<ScoreBoostPickup>();
            if (boost != null && boost.TryCollect())
            {
                _game.ActivateScoreBoost(_config.scoreBoostDuration);
                _audio.PlayPowerUp();
            }
        }

        private void AttractNearbyCoins()
        {
            if (_magnetRemaining <= 0f)
            {
                return;
            }

            var center = transform.position + Vector3.forward * 2.5f + Vector3.up;
            var count = Physics.OverlapSphereNonAlloc(center, _config.magnetRadius, _magnetHits, Physics.AllLayers, QueryTriggerInteraction.Collide);
            for (var i = 0; i < count; i++)
            {
                var coin = _magnetHits[i].GetComponentInParent<CoinPickup>();
                if (coin != null)
                {
                    coin.BeginAttraction(transform, _game);
                }
            }
        }
    }
}
