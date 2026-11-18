using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTemplate.Utilities;

namespace UnityTemplate.EditorTools
{
    /// <summary>Turns a fresh copy of the template into a named game: product name, application identifiers, a new product GUID and the version-text link.</summary>
    public static class NewGameSetup
    {
        public const string CompanyName = "LeonHiggin";
        private const string InitialisationScenePath = "Assets/Scenes/Initialisation.unity";
        private const string GameNameArgument = "-gameName";
        private const string WebsiteUrlArgument = "-websiteUrl";
        private static readonly NamedBuildTarget[] IdentifiedTargets = { NamedBuildTarget.Android, NamedBuildTarget.iOS, NamedBuildTarget.Standalone };

        /// <summary>Applies the set-up for a game called gameName, optionally pointing the version text at websiteUrl (empty clears it).</summary>
        public static void Apply(string gameName, string websiteUrl = "")
        {
            var identifier = IdentifierFor(gameName);
            PlayerSettings.productName = gameName.Trim();
            PlayerSettings.companyName = CompanyName;
            foreach (var target in IdentifiedTargets) PlayerSettings.SetApplicationIdentifier(target, identifier);
            GiveNewProductGuid();
            SetVersionTextUrl(websiteUrl ?? string.Empty);
            AssetDatabase.SaveAssets();
            Debug.Log($"New Game Setup: '{PlayerSettings.productName}' is {identifier}.");
        }

        /// <summary>Entry point for -executeMethod: reads -gameName (required) and -websiteUrl (optional) from the command line.</summary>
        public static void ApplyFromCommandLine()
        {
            var gameName = CommandLineValue(GameNameArgument);
            if (string.IsNullOrWhiteSpace(gameName)) throw new ArgumentException($"Pass the game's name with {GameNameArgument} \"My Game\".");
            Apply(gameName, CommandLineValue(WebsiteUrlArgument));
        }

        /// <summary>Builds com.LeonHiggin.&lt;GameName&gt; from a display name, keeping letters and digits in PascalCase.</summary>
        public static string IdentifierFor(string gameName)
        {
            var words = Regex.Split(gameName ?? string.Empty, "[^A-Za-z0-9]+");
            var builder = new StringBuilder();
            foreach (var word in words.Where(word => word.Length > 0))
            {
                builder.Append(char.ToUpperInvariant(word[0])).Append(word, 1, word.Length - 1);
            }
            if (builder.Length == 0) throw new ArgumentException($"'{gameName}' has no letters or digits to build an application identifier from.");
            if (char.IsDigit(builder[0])) builder.Insert(0, "Game");
            return $"com.{CompanyName}.{builder}";
        }

        /// <summary>Replaces the product GUID copied from the template with a new random one.</summary>
        private static void GiveNewProductGuid()
        {
            var playerSettings = new SerializedObject(Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings"));
            var guid = playerSettings.FindProperty("productGUID");
            var bytes = Guid.NewGuid().ToByteArray();
            for (var i = 0; i < 4; i++)
            {
                guid.FindPropertyRelative($"data[{i}]").uintValue = BitConverter.ToUInt32(bytes, i * 4);
            }
            playerSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>Sets the link the version text in the boot scene opens (the OpenURLButton beside SetVersionNumberText), saving the scene.</summary>
        private static void SetVersionTextUrl(string url)
        {
            var scene = EditorSceneManager.GetSceneByPath(InitialisationScenePath);
            var openedHere = !scene.isLoaded;
            if (openedHere) scene = EditorSceneManager.OpenScene(InitialisationScenePath, OpenSceneMode.Additive);

            foreach (var button in VersionTextButtons(scene))
            {
                var serialized = new SerializedObject(button);
                serialized.FindProperty("url").stringValue = url;
                serialized.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            if (openedHere) EditorSceneManager.CloseScene(scene, true);
        }

        /// <summary>Returns the link buttons on the version text in a scene.</summary>
        public static IEnumerable<OpenURLButton> VersionTextButtons(Scene scene)
        {
            return scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<SetVersionNumberText>(true))
                .Select(versionText => versionText.GetComponent<OpenURLButton>())
                .Where(button => button != null);
        }

        /// <summary>Returns the value after a command-line flag, or an empty string if the flag is missing.</summary>
        private static string CommandLineValue(string flag)
        {
            var arguments = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(arguments, flag);
            return index >= 0 && index + 1 < arguments.Length ? arguments[index + 1] : string.Empty;
        }
    }
}
