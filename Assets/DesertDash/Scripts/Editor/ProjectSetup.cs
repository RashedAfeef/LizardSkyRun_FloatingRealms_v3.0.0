using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DesertDash.Editor
{
    public static class ProjectSetup
    {
        private const string BootstrapScene = "Assets/DesertDash/Scenes/Bootstrap.unity";
        private const string FallbackHeroBuilderScript = "Assets/DesertDash/Scripts/Runtime/Player/JordanianHeroBuilder.cs";
        private const string FantasyThemeBuilderScript = "Assets/DesertDash/Scripts/Runtime/World/FantasyThemeBuilder.cs";
        private const string FantasySkyboxTexture = "Assets/DesertDash/Resources/Skybox/FantasyBlueSky.jpg";
        private const string FantasySkyboxShader = "Assets/DesertDash/Resources/Shaders/FantasyPanoramicSky.shader";
        private const string HeroToonShader = "Assets/DesertDash/Resources/Shaders/JordanianHeroToon.shader";

        [MenuItem("Lizard Sky Run/Open Bootstrap Scene", priority = 1)]
        public static void OpenBootstrapScene()
        {
            EditorSceneManager.OpenScene(BootstrapScene);
        }

        [MenuItem("Lizard Sky Run/Apply Recommended Settings", priority = 20)]
        public static void ApplyRecommendedSettings()
        {
            PlayerSettings.companyName = "Rashed Games";
            PlayerSettings.productName = "Lizard Sky Run: The Floating Realms";
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.runInBackground = false;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.rashedgames.lizardskyrun");
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.rashedgames.lizardskyrun");
            QualitySettings.antiAliasing = 4;
            QualitySettings.shadowDistance = 80f;
            QualitySettings.shadows = ShadowQuality.All;

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootstrapScene, true)
            };

            AssetDatabase.SaveAssets();
            Debug.Log("Lizard Sky Run: recommended settings applied. Replace the company name and application identifiers before publishing.");
        }

        [MenuItem("Lizard Sky Run/Validate Project", priority = 21)]
        public static void ValidateProject()
        {
            var issues = 0;
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapScene) == null)
            {
                Debug.LogError($"Missing bootstrap scene: {BootstrapScene}");
                issues++;
            }

            if (!EditorBuildSettings.scenes.Any(scene => scene.enabled && scene.path == BootstrapScene))
            {
                Debug.LogError("Bootstrap scene is not enabled in Build Settings.");
                issues++;
            }

            if (AssetDatabase.LoadAssetAtPath<MonoScript>(FallbackHeroBuilderScript) == null)
            {
                Debug.LogError($"Missing procedural hero fallback: {FallbackHeroBuilderScript}");
                issues++;
            }

            if (AssetDatabase.LoadAssetAtPath<MonoScript>(FantasyThemeBuilderScript) == null)
            {
                Debug.LogError($"Missing fantasy environment builder: {FantasyThemeBuilderScript}");
                issues++;
            }

            if (AssetDatabase.LoadAssetAtPath<Texture2D>(FantasySkyboxTexture) == null)
            {
                Debug.LogError($"Missing panoramic fantasy Skybox: {FantasySkyboxTexture}");
                issues++;
            }

            if (AssetDatabase.LoadAssetAtPath<Shader>(FantasySkyboxShader) == null)
            {
                Debug.LogError($"Missing panoramic fantasy Skybox shader: {FantasySkyboxShader}");
                issues++;
            }

            if (AssetDatabase.LoadAssetAtPath<Shader>(HeroToonShader) == null)
            {
                Debug.LogError($"Missing cartoon hero toon shader: {HeroToonShader}");
                issues++;
            }

            var heroModel = AssetDatabase.LoadAssetAtPath<GameObject>(MeshyHeroImporter.ModelPath);
            if (heroModel == null)
            {
                Debug.LogError($"Missing active Meshy 3D character: {MeshyHeroImporter.ModelPath}");
                issues++;
            }

            if (AssetDatabase.LoadAssetAtPath<Texture2D>(MeshyHeroImporter.TexturePath) == null)
            {
                Debug.LogError($"Missing active Meshy character texture: {MeshyHeroImporter.TexturePath}");
                issues++;
            }

            var heroAssets = AssetDatabase.LoadAllAssetsAtPath(MeshyHeroImporter.ModelPath);
            var clipNames = heroAssets.OfType<AnimationClip>().Select(clip => clip.name).ToArray();
            foreach (var expectedClip in new[] { "Running", "Run_03", "Walking" })
            {
                if (!clipNames.Contains(expectedClip))
                {
                    Debug.LogError($"Meshy character animation clip was not imported: {expectedClip}");
                    issues++;
                }
            }

            var avatar = heroAssets.OfType<Avatar>().FirstOrDefault();
            if (avatar == null || !avatar.isValid || !avatar.isHuman)
            {
                Debug.LogError("The active Meshy character Humanoid Avatar is missing or invalid. Select the FBX and verify Rig > Humanoid.");
                issues++;
            }

            var scripts = MonoImporter.GetAllRuntimeMonoScripts();
            foreach (var script in scripts)
            {
                if (script.GetClass() == null && AssetDatabase.GetAssetPath(script).Contains("Assets/DesertDash"))
                {
                    Debug.LogError($"Script did not compile: {AssetDatabase.GetAssetPath(script)}");
                    issues++;
                }
            }

            if (issues == 0)
            {
                Debug.Log("Lizard Sky Run validation passed.");
            }
            else
            {
                Debug.LogError($"Lizard Sky Run validation found {issues} issue(s).");
            }
        }
    }
}
