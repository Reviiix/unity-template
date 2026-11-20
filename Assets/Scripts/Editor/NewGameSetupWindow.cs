using System;
using UnityEditor;
using UnityEngine;

namespace UnityTemplate.EditorTools
{
    /// <summary>Tools &gt; New Game Setup window: asks for the game's name and runs NewGameSetup.Apply.</summary>
    public class NewGameSetupWindow : EditorWindow
    {
        private string gameName = string.Empty;
        private string websiteUrl = string.Empty;

        /// <summary>Opens the window.</summary>
        [MenuItem("Tools/New Game Setup...", priority = 0)]
        public static void Open()
        {
            var window = GetWindow<NewGameSetupWindow>(true, "New Game Setup");
            window.minSize = window.maxSize = new Vector2(420, 150);
        }

        /// <summary>Draws the name and link fields, the identifier preview and the Apply button.</summary>
        private void OnGUI()
        {
            gameName = EditorGUILayout.TextField("Game name", gameName);
            websiteUrl = EditorGUILayout.TextField("Website (optional)", websiteUrl);
            EditorGUILayout.LabelField("Identifier", Preview());
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Sets the product name and the Android, iOS and Standalone identifiers, and gives the game a new product GUID.", MessageType.None);

            using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(gameName) || Preview().Length == 0))
            {
                if (!GUILayout.Button("Apply")) return;
            }
            var name = gameName;
            var url = websiteUrl;
            EditorApplication.delayCall += () => NewGameSetup.Apply(name, url);
            Close();
        }

        /// <summary>Returns the identifier the current name would give, or an empty string if it has no usable characters.</summary>
        private string Preview()
        {
            try { return NewGameSetup.IdentifierFor(gameName); }
            catch (ArgumentException) { return string.Empty; }
        }
    }
}
