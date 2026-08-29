using DesertDash.Core;
using DesertDash.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DesertDash.UI
{
    public sealed class RuntimeHud : MonoBehaviour
    {
        private static readonly Color Navy = new Color(0.055f, 0.045f, 0.13f, 0.95f);
        private static readonly Color Cyan = new Color(0.08f, 0.82f, 1.00f, 1f);
        private static readonly Color Orange = new Color(0.84f, 0.18f, 0.72f, 1f);

        private GameManager _game;
        private RunnerController _runner;
        private Text _scoreText;
        private Text _coinText;
        private Text _speedText;
        private Text _distanceText;
        private Text _multiplierText;
        private Text _shieldText;
        private Text _magnetText;
        private Text _boostText;
        private Text _missionText;
        private Image _missionFill;
        private Text _countdownText;
        private Text _gameOverScore;
        private Text _gameOverDetails;
        private Text _highScore;
        private Text _soundLabel;
        private Text _vibrationLabel;
        private GameObject _startPanel;
        private GameObject _gameOverPanel;
        private GameObject _pausePanel;
        private GameObject _countdownPanel;
        private GameObject _missionBanner;
        private float _missionBannerRemaining;

        public void Initialize(GameManager game, RunnerController runner)
        {
            _game = game;
            _runner = runner;
            EnsureEventSystem();
            BuildCanvas();
            _game.StateChanged += OnStateChanged;
            _game.MissionCompleted += OnMissionCompleted;
            OnStateChanged(_game.State);
        }

        private void OnDestroy()
        {
            if (_game != null)
            {
                _game.StateChanged -= OnStateChanged;
                _game.MissionCompleted -= OnMissionCompleted;
            }
        }

        private void Update()
        {
            if (_game == null)
            {
                return;
            }

            _scoreText.text = $"SCORE  {_game.Score:000000}";
            _coinText.text = $"COINS  {_game.Coins:000}";
            _speedText.text = $"SPEED  {_game.CurrentSpeed:0.0}";
            _distanceText.text = $"{_game.Distance:0000} m";
            _multiplierText.text = $"x{_game.CurrentMultiplier}";
            var shield = _runner.ShieldRemaining;
            _shieldText.gameObject.SetActive(shield > 0f);
            _shieldText.text = $"BOARD  {shield:0.0}s";
            var magnet = _runner.MagnetRemaining;
            _magnetText.gameObject.SetActive(magnet > 0f);
            _magnetText.text = $"MAGNET  {magnet:0.0}s";
            var boost = _runner.ScoreBoostRemaining;
            _boostText.gameObject.SetActive(boost > 0f);
            _boostText.text = $"2X BOOST  {boost:0.0}s";

            if (_game.Mission != null)
            {
                _missionText.text = _game.IsMissionComplete
                    ? "MISSION COMPLETE"
                    : $"MISSION  {_game.Mission.Label}  {_game.MissionCurrent}/{_game.Mission.Target}";
                _missionFill.fillAmount = _game.MissionProgress;
                _missionFill.color = _game.IsMissionComplete ? new Color(0.28f, 0.94f, 0.48f) : Cyan;
            }

            if (_game.State == GameState.Countdown)
            {
                var count = Mathf.CeilToInt(_game.CountdownRemaining);
                _countdownText.text = count > 0 ? count.ToString() : "GO!";
            }

            if (_missionBannerRemaining > 0f)
            {
                _missionBannerRemaining -= Time.unscaledDeltaTime;
                _missionBanner.SetActive(true);
            }
            else if (_missionBanner != null)
            {
                _missionBanner.SetActive(false);
            }
        }

        private void BuildCanvas()
        {
            var canvasObject = new GameObject("GameCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var safeArea = CreateRect("SafeArea", canvasObject.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            safeArea.gameObject.AddComponent<SafeAreaPanel>();
            BuildHud(safeArea);
            _startPanel = BuildStartPanel(safeArea);
            _gameOverPanel = BuildGameOverPanel(safeArea);
            _pausePanel = BuildPausePanel(safeArea);
            _countdownPanel = BuildCountdownPanel(safeArea);
            _missionBanner = BuildMissionBanner(safeArea);
        }

        private void BuildHud(RectTransform parent)
        {
            var topBar = CreatePanel("TopBar", parent, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -122f), Vector2.zero, new Color(0.055f, 0.045f, 0.13f, 0.88f));
            _scoreText = CreateText("Score", topBar.transform, "SCORE  000000", 34, TextAnchor.MiddleLeft, Color.white, new Vector2(25f, 0f), new Vector2(630f, 122f), new Vector2(0f, 0.5f));
            _coinText = CreateText("Coins", topBar.transform, "COINS  000", 34, TextAnchor.MiddleCenter, new Color(1f, 0.75f, 0.12f), Vector2.zero, new Vector2(430f, 122f), new Vector2(0.5f, 0.5f));
            _distanceText = CreateText("Distance", topBar.transform, "0000 m", 27, TextAnchor.MiddleRight, new Color(0.80f, 0.88f, 0.94f), new Vector2(-380f, 0f), new Vector2(260f, 122f), new Vector2(1f, 0.5f));
            _speedText = CreateText("Speed", topBar.transform, "SPEED  10.0", 25, TextAnchor.MiddleRight, Color.white, new Vector2(-135f, 0f), new Vector2(260f, 122f), new Vector2(1f, 0.5f));
            _multiplierText = CreateText("Multiplier", parent, "x1", 44, TextAnchor.MiddleCenter, new Color(0.88f, 0.58f, 0.08f), new Vector2(74f, -168f), new Vector2(150f, 70f), new Vector2(0f, 1f));
            _shieldText = CreateText("Shield", parent, "BOARD  0.0s", 25, TextAnchor.MiddleLeft, Cyan, new Vector2(28f, -250f), new Vector2(390f, 52f), new Vector2(0f, 1f));
            _magnetText = CreateText("Magnet", parent, "MAGNET  0.0s", 25, TextAnchor.MiddleLeft, new Color(1f, 0.35f, 0.38f), new Vector2(28f, -305f), new Vector2(390f, 52f), new Vector2(0f, 1f));
            _boostText = CreateText("Boost", parent, "2X BOOST  0.0s", 25, TextAnchor.MiddleLeft, new Color(0.88f, 0.58f, 0.08f), new Vector2(28f, -360f), new Vector2(390f, 52f), new Vector2(0f, 1f));
            var missionPanel = CreatePanel("MissionPanel", parent, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-380f, -205f), new Vector2(380f, -130f), new Color(0.055f, 0.045f, 0.13f, 0.84f));
            _missionText = CreateText("MissionText", missionPanel.transform, "MISSION", 22, TextAnchor.MiddleCenter, Color.white, new Vector2(0f, 12f), new Vector2(720f, 42f), new Vector2(0.5f, 0.5f));
            _missionFill = CreateProgressBar(missionPanel.transform, new Vector2(0f, -21f), new Vector2(690f, 10f));
            CreateButton("Pause", topBar.transform, "II", () => _game.TogglePause(), new Vector2(-34f, 0f), new Vector2(78f, 72f), new Vector2(1f, 0.5f), Cyan);
        }

        private GameObject BuildStartPanel(RectTransform parent)
        {
            var panel = CreatePanel("StartPanel", parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0.035f, 0.025f, 0.10f, 0.58f));
            CreateText("Title", panel.transform, "LIZARD SKY RUN", 82, TextAnchor.MiddleCenter, Cyan, new Vector2(-420f, 245f), new Vector2(980f, 140f), new Vector2(0.5f, 0.5f));
            CreateText("Subtitle", panel.transform, "THE FLOATING REALMS", 27, TextAnchor.MiddleCenter, Orange, new Vector2(-420f, 154f), new Vector2(900f, 60f), new Vector2(0.5f, 0.5f));
            CreateText("Stats", panel.transform, $"BEST  {_game.HighScore:000000}     COINS  {_game.LifetimeCoins:0000}", 25, TextAnchor.MiddleCenter, Color.white, new Vector2(-420f, 83f), new Vector2(900f, 56f), new Vector2(0.5f, 0.5f));
            CreateText("Mission", panel.transform, $"NEXT MISSION  •  {_game.Mission.Label}", 24, TextAnchor.MiddleCenter, new Color(0.88f, 0.58f, 0.08f), new Vector2(-420f, 18f), new Vector2(920f, 56f), new Vector2(0.5f, 0.5f));
            CreateText("Help", panel.transform, "SWIPE to change lanes  •  UP to jump  •  DOWN to slide\nAnimated lizard runner  •  Explore the floating realms", 23, TextAnchor.MiddleCenter, new Color(0.76f, 0.86f, 1.00f), new Vector2(-420f, -68f), new Vector2(960f, 104f), new Vector2(0.5f, 0.5f));
            CreateButton("Start", panel.transform, "START RUN", () => _game.StartRun(), new Vector2(-420f, -213f), new Vector2(420f, 102f), new Vector2(0.5f, 0.5f), Orange);
            return panel.gameObject;
        }

        private GameObject BuildGameOverPanel(RectTransform parent)
        {
            var panel = CreatePanel("GameOverPanel", parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Navy);
            CreateText("Title", panel.transform, "REALM RUN COMPLETE", 64, TextAnchor.MiddleCenter, Orange, new Vector2(0f, 205f), new Vector2(1200f, 130f), new Vector2(0.5f, 0.5f));
            _gameOverScore = CreateText("FinalScore", panel.transform, "SCORE  000000", 44, TextAnchor.MiddleCenter, Color.white, new Vector2(0f, 62f), new Vector2(800f, 80f), new Vector2(0.5f, 0.5f));
            _gameOverDetails = CreateText("Details", panel.transform, "DISTANCE  0 m   •   COINS  0", 27, TextAnchor.MiddleCenter, new Color(0.80f, 0.88f, 0.94f), new Vector2(0f, -4f), new Vector2(900f, 55f), new Vector2(0.5f, 0.5f));
            _highScore = CreateText("HighScore", panel.transform, "BEST  000000", 32, TextAnchor.MiddleCenter, Cyan, new Vector2(0f, -62f), new Vector2(800f, 70f), new Vector2(0.5f, 0.5f));
            CreateButton("Retry", panel.transform, "RUN AGAIN", Restart, new Vector2(0f, -190f), new Vector2(420f, 104f), new Vector2(0.5f, 0.5f), Orange);
            return panel.gameObject;
        }

        private GameObject BuildPausePanel(RectTransform parent)
        {
            var panel = CreatePanel("PausePanel", parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Navy);
            CreateText("Title", panel.transform, "PAUSED", 76, TextAnchor.MiddleCenter, Cyan, new Vector2(0f, 120f), new Vector2(900f, 130f), new Vector2(0.5f, 0.5f));
            CreateButton("Resume", panel.transform, "RESUME", () => _game.TogglePause(), new Vector2(0f, -25f), new Vector2(420f, 104f), new Vector2(0.5f, 0.5f), Orange);
            var soundButton = CreateButton("Sound", panel.transform, "SOUND ON", ToggleSound, new Vector2(0f, -155f), new Vector2(320f, 78f), new Vector2(0.5f, 0.5f), Cyan);
            _soundLabel = soundButton.GetComponentInChildren<Text>();
            var vibrationButton = CreateButton("Vibration", panel.transform, "VIBRATION ON", ToggleVibration, new Vector2(0f, -250f), new Vector2(320f, 72f), new Vector2(0.5f, 0.5f), new Color(0.88f, 0.58f, 0.08f));
            _vibrationLabel = vibrationButton.GetComponentInChildren<Text>();
            RefreshSoundLabel();
            RefreshVibrationLabel();
            return panel.gameObject;
        }

        private GameObject BuildCountdownPanel(RectTransform parent)
        {
            var panel = CreatePanel("CountdownPanel", parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0f, 0f, 0f, 0.18f));
            _countdownText = CreateText("Countdown", panel.transform, "3", 170, TextAnchor.MiddleCenter, Color.white, Vector2.zero, new Vector2(500f, 260f), new Vector2(0.5f, 0.5f));
            return panel.gameObject;
        }

        private GameObject BuildMissionBanner(RectTransform parent)
        {
            var panel = CreatePanel("MissionComplete", parent, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-310f, -310f), new Vector2(310f, -225f), new Color(0.28f, 0.12f, 0.60f, 0.94f));
            CreateText("Label", panel.transform, "MISSION COMPLETE  +", 31, TextAnchor.MiddleCenter, Color.white, Vector2.zero, new Vector2(590f, 85f), new Vector2(0.5f, 0.5f));
            panel.gameObject.SetActive(false);
            return panel.gameObject;
        }

        private void OnStateChanged(GameState state)
        {
            if (_startPanel == null)
            {
                return;
            }

            _startPanel.SetActive(state == GameState.Ready);
            _pausePanel.SetActive(state == GameState.Paused);
            _gameOverPanel.SetActive(state == GameState.GameOver);
            _countdownPanel.SetActive(state == GameState.Countdown);
            if (state == GameState.GameOver)
            {
                _gameOverScore.text = $"SCORE  {_game.Score:000000}";
                _highScore.text = $"BEST  {_game.HighScore:000000}";
                _gameOverDetails.text = $"DISTANCE  {_game.Distance:0} m   •   COINS  {_game.Coins}   •   {(_game.IsMissionComplete ? "MISSION COMPLETE" : "MISSION IN PROGRESS")}";
            }

            if (state == GameState.Paused)
            {
                RefreshSoundLabel();
                RefreshVibrationLabel();
            }
        }

        private void OnMissionCompleted()
        {
            _missionBannerRemaining = 2.2f;
        }

        private void ToggleSound()
        {
            _game.SetSoundEnabled(!_game.SoundEnabled);
            RefreshSoundLabel();
        }

        private void RefreshSoundLabel()
        {
            if (_soundLabel != null)
            {
                _soundLabel.text = _game.SoundEnabled ? "SOUND ON" : "SOUND OFF";
            }
        }

        private void ToggleVibration()
        {
            _game.SetVibrationEnabled(!_game.VibrationEnabled);
            RefreshVibrationLabel();
        }

        private void RefreshVibrationLabel()
        {
            if (_vibrationLabel != null)
            {
                _vibrationLabel.text = _game.VibrationEnabled ? "VIBRATION ON" : "VIBRATION OFF";
            }
        }

        private static void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }

        private static RectTransform CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return rect;
        }

        private static Image CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color color)
        {
            var rect = CreateRect(name, parent, anchorMin, anchorMax, offsetMin, offsetMax);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(string name, Transform parent, string value, int fontSize, TextAnchor alignment, Color color, Vector2 position, Vector2 size, Vector2 anchor)
        {
            var rect = CreateRect(name, parent, anchor, anchor, Vector2.zero, Vector2.zero);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var text = rect.gameObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label, UnityEngine.Events.UnityAction action, Vector2 position, Vector2 size, Vector2 anchor, Color color)
        {
            var rect = CreateRect(name, parent, anchor, anchor, Vector2.zero, Vector2.zero);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);
            var colors = button.colors;
            colors.highlightedColor = Color.Lerp(color, Color.white, 0.18f);
            colors.pressedColor = Color.Lerp(color, Color.black, 0.18f);
            button.colors = colors;

            var text = CreateText("Label", rect, label, 29, TextAnchor.MiddleCenter, Color.white, Vector2.zero, size, new Vector2(0.5f, 0.5f));
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 18;
            text.resizeTextMaxSize = 31;
            return button;
        }

        private static Image CreateProgressBar(Transform parent, Vector2 position, Vector2 size)
        {
            var backgroundRect = CreateRect("ProgressBackground", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            backgroundRect.anchoredPosition = position;
            backgroundRect.sizeDelta = size;
            var background = backgroundRect.gameObject.AddComponent<Image>();
            background.color = new Color(1f, 1f, 1f, 0.14f);

            var fillRect = CreateRect("Fill", backgroundRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var fill = fillRect.gameObject.AddComponent<Image>();
            fill.color = Cyan;
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 0f;
            return fill;
        }
    }
}
