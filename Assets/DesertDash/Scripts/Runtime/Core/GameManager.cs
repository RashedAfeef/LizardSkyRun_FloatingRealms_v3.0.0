using System;
using UnityEngine;

namespace DesertDash.Core
{
    public sealed class GameManager : MonoBehaviour
    {
        public event Action<GameState> StateChanged;
        public event Action CoinCollected;
        public event Action RunEnded;
        public event Action MissionCompleted;

        private RunnerConfig _config;
        private SaveService _saveService;
        private float _elapsed;
        private float _distance;
        private float _score;
        private float _countdownRemaining;
        private float _scoreBoostRemaining;
        private int _coins;
        private bool _missionCompleted;
        private RunMission _mission;
        private GameState _state = GameState.Ready;

        public GameState State => _state;
        public float Distance => _distance;
        public int Coins => _coins;
        public int Score => Mathf.Max(0, Mathf.FloorToInt(_score));
        public int HighScore => Mathf.Max(_saveService.HighScore, Score);
        public float CurrentSpeed => RunnerMath.SpeedAt(_elapsed, _config.startSpeed, _config.maximumSpeed, _config.accelerationPerSecond);
        public float Difficulty => _config.DifficultyAt(_distance);
        public int BaseMultiplier => RunnerMath.BaseMultiplierAt(_distance, _config.multiplierStepDistance, _config.maximumBaseMultiplier);
        public int CurrentMultiplier => BaseMultiplier * (_scoreBoostRemaining > 0f ? 2 : 1);
        public float ScoreBoostRemaining => Mathf.Max(0f, _scoreBoostRemaining);
        public float CountdownRemaining => Mathf.Max(0f, _countdownRemaining);
        public RunMission Mission => _mission;
        public bool IsMissionComplete => _missionCompleted;
        public float MissionProgress => _mission == null ? 0f : _mission.Progress(_distance, _coins, Score);
        public int MissionCurrent => _mission == null ? 0 : _mission.Current(_distance, _coins, Score);
        public int LifetimeCoins => _saveService.LifetimeCoins;
        public int RunsPlayed => _saveService.RunsPlayed;
        public int MissionsCompletedCount => _saveService.MissionsCompleted;
        public float BestDistance => Mathf.Max(_saveService.BestDistance, _distance);
        public bool SoundEnabled => _saveService.SoundEnabled;
        public bool VibrationEnabled => _saveService.VibrationEnabled;

        public void Initialize(RunnerConfig config)
        {
            _config = config;
            _saveService = new SaveService();
            _mission = RunMission.CreateForRun(_saveService.RunsPlayed);
            SetState(GameState.Ready);
        }

        private void Update()
        {
            if (_state == GameState.Countdown)
            {
                _countdownRemaining -= Time.deltaTime;
                if (_countdownRemaining <= 0f)
                {
                    SetState(GameState.Running);
                }

                return;
            }

            if (_state != GameState.Running)
            {
                return;
            }

            var multiplier = CurrentMultiplier;
            _elapsed += Time.deltaTime;
            var distanceDelta = CurrentSpeed * Time.deltaTime;
            _distance += distanceDelta;
            _score += distanceDelta * multiplier;
            if (_scoreBoostRemaining > 0f)
            {
                _scoreBoostRemaining -= Time.deltaTime;
            }

            CheckMission();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused && _state == GameState.Running)
            {
                TogglePause();
            }
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused && _state == GameState.Running)
            {
                TogglePause();
            }
        }

        public void StartRun()
        {
            if (_state != GameState.Ready)
            {
                return;
            }

            Time.timeScale = 1f;
            _elapsed = 0f;
            _distance = 0f;
            _score = 0f;
            _coins = 0;
            _scoreBoostRemaining = 0f;
            _missionCompleted = false;
            _mission = RunMission.CreateForRun(_saveService.RunsPlayed);
            _countdownRemaining = 3f;
            SetState(GameState.Countdown);
        }

        public void TogglePause()
        {
            if (_state == GameState.Running)
            {
                Time.timeScale = 0f;
                SetState(GameState.Paused);
            }
            else if (_state == GameState.Paused)
            {
                Time.timeScale = 1f;
                SetState(GameState.Running);
            }
        }

        public void CollectCoin()
        {
            if (_state != GameState.Running)
            {
                return;
            }

            _coins++;
            _score += _config.coinScore * CurrentMultiplier;
            CoinCollected?.Invoke();
            CheckMission();
        }

        public void ActivateScoreBoost(float duration)
        {
            if (_state != GameState.Running)
            {
                return;
            }

            _scoreBoostRemaining = Mathf.Max(_scoreBoostRemaining, duration);
        }

        public void EndRun()
        {
            if (_state != GameState.Running)
            {
                return;
            }

            _saveService.RecordRun(Score, _coins, _distance, _missionCompleted);
            SetState(GameState.GameOver);
            RunEnded?.Invoke();
        }

        public void SetSoundEnabled(bool enabled)
        {
            _saveService.SetSoundEnabled(enabled);
        }

        public void SetVibrationEnabled(bool enabled)
        {
            _saveService.SetVibrationEnabled(enabled);
        }

        private void CheckMission()
        {
            if (_missionCompleted || _mission == null || _mission.Progress(_distance, _coins, Score) < 1f)
            {
                return;
            }

            _missionCompleted = true;
            MissionCompleted?.Invoke();
        }

        private void SetState(GameState next)
        {
            _state = next;
            StateChanged?.Invoke(next);
        }
    }
}
