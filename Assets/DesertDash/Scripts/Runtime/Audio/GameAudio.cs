using DesertDash.Core;
using UnityEngine;

namespace DesertDash.Audio
{
    public sealed class GameAudio : MonoBehaviour
    {
        private AudioSource _source;
        private AudioSource _musicSource;
        private GameManager _game;
        private AudioClip _coin;
        private AudioClip _jump;
        private AudioClip _slide;
        private AudioClip _shield;
        private AudioClip _shieldBreak;
        private AudioClip _powerUp;
        private AudioClip _mission;
        private AudioClip _countdown;
        private AudioClip _hit;
        private int _lastCountdownNumber = -1;

        public void Initialize(GameManager game)
        {
            _game = game;
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            _musicSource.spatialBlend = 0f;
            _musicSource.volume = 0.25f;
            _coin = CreateTone("Coin", 880f, 0.08f, 0.18f);
            _jump = CreateTone("Jump", 430f, 0.12f, 0.16f, 190f);
            _slide = CreateTone("Slide", 185f, 0.14f, 0.13f, 80f);
            _shield = CreateTone("Shield", 620f, 0.2f, 0.16f, 480f);
            _shieldBreak = CreateTone("ShieldBreak", 260f, 0.18f, 0.20f, 95f);
            _powerUp = CreateTone("PowerUp", 420f, 0.30f, 0.16f, 980f);
            _mission = CreateTone("Mission", 660f, 0.42f, 0.14f, 1320f);
            _countdown = CreateTone("Countdown", 520f, 0.10f, 0.13f, 620f);
            _hit = CreateTone("Hit", 120f, 0.28f, 0.22f, 55f);
            _musicSource.clip = CreateBeatLoop();

            _game.CoinCollected += PlayCoin;
            _game.RunEnded += PlayHit;
            _game.MissionCompleted += PlayMission;
            _game.StateChanged += OnStateChanged;
        }

        private void Update()
        {
            if (_game == null)
            {
                return;
            }

            if (_game.State == GameState.Running && !_game.SoundEnabled && _musicSource.isPlaying)
            {
                _musicSource.Pause();
            }

            if (_game.State != GameState.Countdown)
            {
                return;
            }

            var number = Mathf.CeilToInt(_game.CountdownRemaining);
            if (number != _lastCountdownNumber && number > 0 && number <= 3)
            {
                _lastCountdownNumber = number;
                Play(_countdown);
            }
        }

        private void OnDestroy()
        {
            if (_game == null)
            {
                return;
            }

            _game.CoinCollected -= PlayCoin;
            _game.RunEnded -= PlayHit;
            _game.MissionCompleted -= PlayMission;
            _game.StateChanged -= OnStateChanged;
        }

        public void PlayJump()
        {
            Play(_jump);
        }

        public void PlayShield()
        {
            Play(_shield);
        }

        public void PlayShieldBreak()
        {
            Play(_shieldBreak);
        }

        public void PlaySlide()
        {
            Play(_slide);
        }

        public void PlayPowerUp()
        {
            Play(_powerUp);
        }

        private void PlayCoin()
        {
            Play(_coin);
        }

        private void PlayHit()
        {
            Play(_hit);
        }

        private void PlayMission()
        {
            Play(_mission);
        }

        private void OnStateChanged(GameState state)
        {
            if (_musicSource == null)
            {
                return;
            }

            if (state == GameState.Countdown)
            {
                _lastCountdownNumber = -1;
            }
            else if (state == GameState.Running)
            {
                if (_game.SoundEnabled && !_musicSource.isPlaying)
                {
                    _musicSource.Play();
                }
                else if (_game.SoundEnabled)
                {
                    _musicSource.UnPause();
                }
            }
            else if (state == GameState.Paused)
            {
                _musicSource.Pause();
            }
            else if (state == GameState.GameOver || state == GameState.Ready)
            {
                _musicSource.Stop();
            }
        }

        private void Play(AudioClip clip)
        {
            if (_game.SoundEnabled && clip != null)
            {
                _source.PlayOneShot(clip);
            }
        }

        private static AudioClip CreateTone(string clipName, float startFrequency, float duration, float volume, float endFrequency = -1f)
        {
            const int sampleRate = 44100;
            var sampleCount = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[sampleCount];
            var targetFrequency = endFrequency > 0f ? endFrequency : startFrequency;

            for (var i = 0; i < sampleCount; i++)
            {
                var t = i / (float)sampleRate;
                var normalized = i / (float)Mathf.Max(1, sampleCount - 1);
                var frequency = Mathf.Lerp(startFrequency, targetFrequency, normalized);
                var envelope = 1f - normalized;
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * volume;
            }

            var clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static AudioClip CreateBeatLoop()
        {
            const int sampleRate = 44100;
            const float duration = 8f;
            var melody = new[]
            {
                220.00f, 233.08f, 277.18f, 293.66f,
                329.63f, 293.66f, 277.18f, 233.08f,
                220.00f, 277.18f, 293.66f, 349.23f,
                415.30f, 349.23f, 293.66f, 277.18f
            };
            var sampleCount = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[sampleCount];
            for (var i = 0; i < sampleCount; i++)
            {
                var t = i / (float)sampleRate;
                var beatPhase = t % 0.5f;
                var stepTime = t % 0.50f;
                var step = Mathf.FloorToInt(t / 0.50f) % melody.Length;
                var note = melody[step];

                var drum = Mathf.Exp(-beatPhase * 25f) * Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(145f, 62f, Mathf.Clamp01(beatPhase * 10f)) * beatPhase);
                var rimPhase = (t + 0.25f) % 0.50f;
                var rim = rimPhase < 0.025f ? Mathf.Sin(2f * Mathf.PI * 1800f * rimPhase) * Mathf.Exp(-rimPhase * 80f) : 0f;
                var pluckEnvelope = Mathf.Exp(-stepTime * 5.8f);
                var pluck = (Mathf.Sin(2f * Mathf.PI * note * stepTime) + Mathf.Sin(2f * Mathf.PI * note * 2f * stepTime) * 0.34f) * pluckEnvelope;
                var drone = Mathf.Sin(2f * Mathf.PI * 110f * t) * 0.025f;
                data[i] = drum * 0.12f + rim * 0.035f + pluck * 0.07f + drone;
            }

            var clip = AudioClip.Create("FloatingRealmsOriginalLoop", sampleCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
