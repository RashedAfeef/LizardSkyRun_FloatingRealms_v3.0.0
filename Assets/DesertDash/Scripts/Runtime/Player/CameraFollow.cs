using DesertDash.Core;
using UnityEngine;

namespace DesertDash.Player
{
    public sealed class CameraFollow : MonoBehaviour
    {
        private Transform _target;
        private GameManager _game;
        private Vector3 _velocity;
        private Camera _camera;
        private float _lastTargetX;
        private float _shakeRemaining;
        private float _shakeMagnitude;

        public void Initialize(Transform target, GameManager game)
        {
            _target = target;
            _game = game;
            _camera = GetComponent<Camera>();
            transform.position = target.position + new Vector3(0f, 3.45f, -4.70f);
            _lastTargetX = target.position.x;
            _game.RunEnded += OnRunEnded;
        }

        private void OnDestroy()
        {
            if (_game != null)
            {
                _game.RunEnded -= OnRunEnded;
            }
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            var lateralSpeed = (_target.position.x - _lastTargetX) / Mathf.Max(Time.unscaledDeltaTime, 0.001f);
            _lastTargetX = _target.position.x;
            var showcase = _game != null && (_game.State == GameState.Ready || _game.State == GameState.GameOver);
            Vector3 desired;
            Vector3 lookPoint;
            if (showcase)
            {
                var orbit = Mathf.Sin(Time.unscaledTime * 0.30f) * 0.28f;
                desired = _target.position + new Vector3(2.65f + orbit, 2.35f, 3.70f);
                lookPoint = _target.position + new Vector3(-0.46f, 1.18f, 0f);
            }
            else
            {
                var speedRatio = _game == null ? 0f : Mathf.InverseLerp(10f, 24f, _game.CurrentSpeed);
                var lateralLag = Mathf.Clamp(lateralSpeed * 0.035f, -0.28f, 0.28f);
                desired = _target.position + new Vector3(-lateralLag, Mathf.Lerp(3.42f, 3.66f, speedRatio), Mathf.Lerp(-4.65f, -5.25f, speedRatio));
                lookPoint = _target.position + new Vector3(lateralLag * 0.18f, 1.15f, 5.65f);
            }

            if (_shakeRemaining > 0f)
            {
                _shakeRemaining -= Time.unscaledDeltaTime;
                desired += Random.insideUnitSphere * _shakeMagnitude * Mathf.Clamp01(_shakeRemaining * 5f);
            }

            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, 0.12f);
            var lookRotation = Quaternion.LookRotation(lookPoint - transform.position);
            var roll = showcase ? 0f : Mathf.Clamp(-lateralSpeed * 0.42f, -4.2f, 4.2f);
            lookRotation *= Quaternion.Euler(0f, 0f, roll);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 9f * Time.unscaledDeltaTime);

            if (_camera != null && _game != null)
            {
                var speedRatio = Mathf.InverseLerp(10f, 24f, _game.CurrentSpeed);
                var targetFieldOfView = showcase ? 42f : Mathf.Lerp(49f, 56f, speedRatio);
                _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFieldOfView, 1f - Mathf.Exp(-4.5f * Time.unscaledDeltaTime));
            }
        }

        private void OnRunEnded()
        {
            _shakeRemaining = 0.42f;
            _shakeMagnitude = 0.48f;
        }
    }
}
