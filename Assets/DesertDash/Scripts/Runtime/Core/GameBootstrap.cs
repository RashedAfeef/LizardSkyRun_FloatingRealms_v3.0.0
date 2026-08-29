using DesertDash.Audio;
using DesertDash.Input;
using DesertDash.Player;
using DesertDash.UI;
using DesertDash.World;
using UnityEngine;

namespace DesertDash.Core
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private RunnerConfig config;

        private static GameBootstrap _active;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureBootstrapExists()
        {
            if (_active == null && UnityEngine.Object.FindFirstObjectByType<GameBootstrap>() == null)
            {
                new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
            }
        }

        private void Awake()
        {
            if (_active != null && _active != this)
            {
                Destroy(gameObject);
                return;
            }

            _active = this;
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<RunnerConfig>();
                config.name = "Runtime Runner Config";
            }

            BuildGame();
        }

        private void BuildGame()
        {
            ConfigureEnvironment();
            var materials = new RuntimeMaterialLibrary();

            var managerObject = new GameObject("GameManager");
            managerObject.transform.SetParent(transform, false);
            var game = managerObject.AddComponent<GameManager>();
            game.Initialize(config);

            var audioObject = new GameObject("GameAudio");
            audioObject.transform.SetParent(transform, false);
            var gameAudio = audioObject.AddComponent<GameAudio>();
            gameAudio.Initialize(game);

            var runner = BuildRunner(materials, game, gameAudio);
            BuildTrack(runner.transform, game, materials);
            BuildCamera(runner.transform, game);
            BuildHud(game, runner);
        }

        private RunnerController BuildRunner(RuntimeMaterialLibrary materials, GameManager game, GameAudio gameAudio)
        {
            var root = new GameObject("Runner");
            root.transform.SetParent(transform, false);
            root.transform.position = Vector3.zero;
            var controller = root.AddComponent<CharacterController>();
            controller.height = 2.44f;
            controller.radius = 0.44f;
            controller.center = new Vector3(0f, 1.22f, 0f);
            controller.skinWidth = 0.06f;
            controller.stepOffset = 0.22f;
            root.AddComponent<RunnerInput>();

            var visualObject = new GameObject("RunnerVisual");
            visualObject.transform.SetParent(root.transform, false);
            var visual = visualObject.AddComponent<RunnerCharacterVisual>();
            visual.Initialize(game, materials);

            var runner = root.AddComponent<RunnerController>();
            runner.Initialize(game, gameAudio, config, visual);
            return runner;
        }

        private void BuildTrack(Transform runner, GameManager game, RuntimeMaterialLibrary materials)
        {
            var trackObject = new GameObject("TrackSystem");
            trackObject.transform.SetParent(transform, false);
            var track = trackObject.AddComponent<TrackManager>();
            track.Initialize(runner, game, config, materials);
        }

        private void BuildCamera(Transform runner, GameManager game)
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(transform, false);
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.backgroundColor = new Color(0.37f, 0.62f, 0.82f);
            camera.nearClipPlane = 0.08f;
            camera.farClipPlane = 330f;
            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<CameraFollow>().Initialize(runner, game);
        }

        private void BuildHud(GameManager game, RunnerController runner)
        {
            var uiObject = new GameObject("UI");
            uiObject.transform.SetParent(transform, false);
            uiObject.AddComponent<RuntimeHud>().Initialize(game, runner);
        }

        private static void ConfigureEnvironment()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.48f, 0.68f, 0.86f);
            RenderSettings.ambientEquatorColor = new Color(0.30f, 0.38f, 0.58f);
            RenderSettings.ambientGroundColor = new Color(0.095f, 0.075f, 0.17f);
            RenderSettings.ambientIntensity = 0.92f;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.34f, 0.52f, 0.72f);
            RenderSettings.fogStartDistance = 120f;
            RenderSettings.fogEndDistance = 300f;

            var sunObject = new GameObject("FantasySun");
            var sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.91f, 0.74f);
            sun.intensity = 1.18f;
            sun.shadows = LightShadows.Soft;
            sunObject.transform.rotation = Quaternion.Euler(38f, -32f, 0f);
            RenderSettings.sun = sun;

            var panorama = Resources.Load<Texture2D>("Skybox/FantasyBlueSky");
            var panoramicShader = Resources.Load<Shader>("Shaders/FantasyPanoramicSky") ?? Shader.Find("Skybox/Panoramic");
            if (panorama != null && panoramicShader != null)
            {
                panorama.wrapMode = TextureWrapMode.Repeat;
                panorama.filterMode = FilterMode.Trilinear;
                var panoramicSky = new Material(panoramicShader) { name = "FantasyBlueSkyPanorama" };
                panoramicSky.SetTexture("_MainTex", panorama);
                panoramicSky.SetColor("_Tint", new Color(0.80f, 0.90f, 1.00f));
                panoramicSky.SetFloat("_Exposure", 1.08f);
                panoramicSky.SetFloat("_Rotation", 205f);
                if (panoramicSky.HasProperty("_Mapping")) panoramicSky.SetFloat("_Mapping", 1f);
                if (panoramicSky.HasProperty("_ImageType")) panoramicSky.SetFloat("_ImageType", 0f);
                if (panoramicSky.HasProperty("_MirrorOnBack")) panoramicSky.SetFloat("_MirrorOnBack", 0f);
                if (panoramicSky.HasProperty("_Layout")) panoramicSky.SetFloat("_Layout", 0f);
                RenderSettings.skybox = panoramicSky;
            }
            else
            {
                var fallbackShader = Resources.Load<Shader>("Shaders/AmmanSkybox") ?? Shader.Find("DesertDash/Amman Skybox");
                if (fallbackShader != null)
                {
                    var fallbackSky = new Material(fallbackShader) { name = "FantasyBlueSkyFallback" };
                    fallbackSky.SetColor("_ZenithColor", new Color(0.08f, 0.32f, 0.70f));
                    fallbackSky.SetColor("_HorizonColor", new Color(0.52f, 0.78f, 0.94f));
                    fallbackSky.SetColor("_GroundColor", new Color(0.16f, 0.12f, 0.28f));
                    fallbackSky.SetColor("_SunColor", new Color(1.00f, 0.90f, 0.66f));
                    fallbackSky.SetVector("_SunDirection", -sunObject.transform.forward);
                    fallbackSky.SetFloat("_SunSize", 0.012f);
                    fallbackSky.SetFloat("_CloudAmount", 0.26f);
                    RenderSettings.skybox = fallbackSky;
                }
            }

            var fillObject = new GameObject("AetherFill");
            var fill = fillObject.AddComponent<Light>();
            fill.type = LightType.Directional;
            fill.color = new Color(0.35f, 0.58f, 1.00f);
            fill.intensity = 0.38f;
            fill.shadows = LightShadows.None;
            fillObject.transform.rotation = Quaternion.Euler(58f, 150f, 0f);

            DynamicGI.UpdateEnvironment();
        }
    }
}
