using System;
using UnityEditor;
using UnityEngine;

namespace DesertDash.Editor
{
    public sealed class MeshyHeroImporter : AssetPostprocessor
    {
        public const string ModelPath = "Assets/DesertDash/Resources/Characters/JordanianHero/JordanianHero_Meshy_Animated_Full.fbx";
        public const string TexturePath = "Assets/DesertDash/Resources/Characters/JordanianHero/JordanianHero_BaseColor.png";
        public const string MotionFolder = "Assets/DesertDash/Resources/Characters/JordanianHero/Motions/";

        private bool IsHeroModel => string.Equals(assetPath, ModelPath, StringComparison.OrdinalIgnoreCase);
        private bool IsHeroTexture => string.Equals(assetPath, TexturePath, StringComparison.OrdinalIgnoreCase);
        private bool IsHeroMotion => assetPath.StartsWith(MotionFolder, StringComparison.OrdinalIgnoreCase) && assetPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase);

        private void OnPreprocessModel()
        {
            if (!IsHeroModel && !IsHeroMotion)
            {
                return;
            }

            var importer = (ModelImporter)assetImporter;
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.importAnimation = true;
            importer.importAnimatedCustomProperties = false;
            importer.importBlendShapes = IsHeroMotion ? false : importer.importBlendShapes;
            importer.importCameras = false;
            importer.importConstraints = false;
            importer.importLights = false;
            importer.isReadable = false;
            importer.meshCompression = ModelImporterMeshCompression.High;
            importer.optimizeGameObjects = false;
            importer.optimizeMeshPolygons = true;
            importer.optimizeMeshVertices = true;
            importer.preserveHierarchy = true;
            importer.resampleCurves = true;
            importer.useFileScale = true;
            importer.weldVertices = true;
        }

        private void OnPreprocessAnimation()
        {
            if (!IsHeroModel && !IsHeroMotion)
            {
                return;
            }

            var importer = (ModelImporter)assetImporter;
            var clips = importer.defaultClipAnimations;
            for (var index = 0; index < clips.Length; index++)
            {
                var clip = clips[index];
                clip.loopTime = clip.name.IndexOf("Run", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                clip.name.IndexOf("Walk", StringComparison.OrdinalIgnoreCase) >= 0;
                clip.loopPose = clip.loopTime;
                clip.keepOriginalOrientation = true;
                clip.keepOriginalPositionY = true;
                clip.keepOriginalPositionXZ = true;
                clip.lockRootRotation = true;
                clip.lockRootHeightY = true;
                clip.lockRootPositionXZ = true;
                clips[index] = clip;
            }

            importer.clipAnimations = clips;
        }

        private void OnPreprocessTexture()
        {
            if (!IsHeroTexture)
            {
                return;
            }

            var importer = (TextureImporter)assetImporter;
            importer.alphaSource = TextureImporterAlphaSource.None;
            importer.maxTextureSize = 2048;
            importer.mipmapEnabled = true;
            importer.sRGBTexture = true;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
        }
    }
}
