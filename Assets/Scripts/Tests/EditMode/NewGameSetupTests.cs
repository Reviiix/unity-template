using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityTemplate.EditorTools;

namespace UnityTemplate.Tests
{
    /// <summary>Checks that the template ships with no cloud project, Remote Config environments or repository link, and how New Game Setup builds identifiers.</summary>
    public class NewGameSetupTests
    {
        private const string TemplateProductName = "Game Template";
        private const string InitialisationScenePath = "Assets/Scenes/Initialisation.unity";
        private const string RemoteConfigDataPath = "Assets/Editor/RemoteConfig/Data/RemoteConfigDataStoreAsset.asset";

        [TestCase("Space Race", "com.LeonHiggin.SpaceRace")]
        [TestCase("space-race!", "com.LeonHiggin.SpaceRace")]
        [TestCase("  Tiny   Tower 2 ", "com.LeonHiggin.TinyTower2")]
        [TestCase("3 in a row", "com.LeonHiggin.Game3InARow")]
        public void IdentifierIsCompanyAndPascalCaseName(string gameName, string expected)
        {
            Assert.AreEqual(expected, NewGameSetup.IdentifierFor(gameName));
        }

        [Test]
        public void IdentifierNeedsALetterOrDigit()
        {
            Assert.Throws<ArgumentException>(() => NewGameSetup.IdentifierFor("!!"));
        }

        [Test]
        public void TemplateShipsUnlinked()
        {
            if (PlayerSettings.productName != TemplateProductName) Assert.Ignore("New Game Setup has run: this is a game, not the template.");

            var playerSettings = new SerializedObject(Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings"));
            foreach (var field in new[] { "cloudProjectId", "organizationId", "projectName" })
            {
                Assert.IsEmpty(playerSettings.FindProperty(field).stringValue, $"PlayerSettings.{field} still links the template to a cloud project.");
            }
            StringAssert.DoesNotContain("projectId", File.ReadAllText(RemoteConfigDataPath), "Remote Config still lists the template's environments.");

            var scene = SceneManager.GetSceneByPath(InitialisationScenePath);
            var openedHere = !scene.isLoaded;
            if (openedHere) scene = EditorSceneManager.OpenScene(InitialisationScenePath, OpenSceneMode.Additive);
            try
            {
                var buttons = NewGameSetup.VersionTextButtons(scene).ToList();
                Assert.IsNotEmpty(buttons, "The version text has no OpenURLButton.");
                foreach (var button in buttons)
                {
                    Assert.IsEmpty(new SerializedObject(button).FindProperty("url").stringValue, $"{button.name} still opens the template's link.");
                }
            }
            finally
            {
                if (openedHere) EditorSceneManager.CloseScene(scene, true);
            }
        }
    }
}
