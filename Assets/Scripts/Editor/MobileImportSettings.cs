using UnityEditor;
using UnityEngine;

namespace UnityTemplate.EditorTools
{
    /// <summary>Applies the project's iOS/Android import settings to every texture and audio clip; re-run it after adding or replacing assets.</summary>
    public static class MobileImportSettings
    {
        private const string MenuPath = "Tools/Apply Mobile Import Settings";
        private const string TexturesFolder = "Assets/Graphics";
        private const string AudioFolder = "Assets/Audio";
        private const int UncompressedSizeLimit = 32;
        private const int MaximumTextureSize = 2048;
        private static readonly string[] MobilePlatforms = { "Android", "iPhone" };

        /// <summary>Re-applies the texture and audio settings to everything under the graphics and audio folders.</summary>
        [MenuItem(MenuPath)]
        public static void ApplyToAllAssets()
        {
            var changedTextures = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { TexturesFolder }))
            {
                if (AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) is TextureImporter importer && ApplyTextureSettings(importer)) changedTextures++;
            }

            var changedClips = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:AudioClip", new[] { AudioFolder }))
            {
                if (AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) is AudioImporter importer && ApplyAudioSettings(importer)) changedClips++;
            }

            Debug.Log($"Mobile import settings applied: {changedTextures} textures and {changedClips} audio clips re-imported.");
        }

        /// <summary>Uses ASTC 4x4 (sharp edges, no blur) at a max size that never shrinks the source; tiny textures stay uncompressed.</summary>
        private static bool ApplyTextureSettings(TextureImporter importer)
        {
            importer.GetSourceTextureWidthAndHeight(out var width, out var height);
            var largestSide = Mathf.Max(width, height);
            var maxSize = Mathf.Clamp(Mathf.NextPowerOfTwo(largestSide), UncompressedSizeLimit, MaximumTextureSize);
            var format = largestSide <= UncompressedSizeLimit ? TextureImporterFormat.RGBA32 : TextureImporterFormat.ASTC_4x4;
            var changed = false;

            if (importer.mipmapEnabled || importer.maxTextureSize != maxSize)
            {
                importer.mipmapEnabled = false;
                importer.maxTextureSize = maxSize;
                changed = true;
            }

            foreach (var platform in MobilePlatforms)
            {
                var settings = importer.GetPlatformTextureSettings(platform);
                if (settings.overridden && settings.format == format && settings.maxTextureSize == maxSize) continue;

                settings.overridden = true;
                settings.format = format;
                settings.maxTextureSize = maxSize;
                settings.compressionQuality = 100;
                importer.SetPlatformTextureSettings(settings);
                changed = true;
            }

            if (changed) importer.SaveAndReimport();
            return changed;
        }

        /// <summary>Makes short UI sounds ADPCM-compressed and decompressed on load: small, instant to play and cheap to decode.</summary>
        private static bool ApplyAudioSettings(AudioImporter importer)
        {
            var changed = false;
            foreach (var platform in MobilePlatforms)
            {
                var settings = importer.GetOverrideSampleSettings(platform);
                if (importer.ContainsSampleSettingsOverride(platform) && settings.loadType == AudioClipLoadType.DecompressOnLoad && settings.compressionFormat == AudioCompressionFormat.ADPCM) continue;

                settings.loadType = AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat = AudioCompressionFormat.ADPCM;
                settings.preloadAudioData = true;
                importer.SetOverrideSampleSettings(platform, settings);
                changed = true;
            }

            if (changed) importer.SaveAndReimport();
            return changed;
        }
    }
}
