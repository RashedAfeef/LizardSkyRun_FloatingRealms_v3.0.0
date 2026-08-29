using System;
using UnityEngine;

namespace DesertDash.Core
{
    public sealed class SaveService
    {
        private const string SaveKey = "amman_albalad_run_save_v1";

        [Serializable]
        private sealed class SaveData
        {
            public int highScore;
            public int lifetimeCoins;
            public int runsPlayed;
            public int missionsCompleted;
            public float bestDistance;
            public bool soundEnabled = true;
            public bool vibrationEnabled = true;
        }

        private SaveData _data;

        public int HighScore => _data.highScore;
        public int LifetimeCoins => _data.lifetimeCoins;
        public int RunsPlayed => _data.runsPlayed;
        public int MissionsCompleted => _data.missionsCompleted;
        public float BestDistance => _data.bestDistance;
        public bool SoundEnabled => _data.soundEnabled;
        public bool VibrationEnabled => _data.vibrationEnabled;

        public SaveService()
        {
            Load();
        }

        public void RecordRun(int score, int collectedCoins, float distance, bool missionCompleted)
        {
            _data.highScore = Mathf.Max(_data.highScore, score);
            _data.lifetimeCoins += Mathf.Max(0, collectedCoins);
            _data.runsPlayed++;
            _data.bestDistance = Mathf.Max(_data.bestDistance, distance);
            if (missionCompleted)
            {
                _data.missionsCompleted++;
            }

            Persist();
        }

        public void SetSoundEnabled(bool enabled)
        {
            _data.soundEnabled = enabled;
            Persist();
        }

        public void SetVibrationEnabled(bool enabled)
        {
            _data.vibrationEnabled = enabled;
            Persist();
        }

        private void Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                _data = new SaveData();
                return;
            }

            try
            {
                _data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(SaveKey)) ?? new SaveData();
            }
            catch (Exception)
            {
                _data = new SaveData();
            }
        }

        private void Persist()
        {
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(_data));
            PlayerPrefs.Save();
        }
    }
}
