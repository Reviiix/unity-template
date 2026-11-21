using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityTemplate.EditorTools;

namespace UnityTemplate.Tests
{
    /// <summary>Checks the project's identity (the template ships unlinked; a game no longer uses the template's identifiers or product GUID) and how New Game Setup builds identifiers.</summary>
    public class NewGameSetupTests
    {
        private const string TemplateProductName = "Game Template";
        private const string InitialisationScenePath = "Assets/Scenes/Initialisation.unity";
        private const string RemoteConfigDataPath = "Assets/Editor/RemoteConfig/Data/RemoteConfigDataStoreAsset.asset";
        private const string ProjectSettingsPath = "ProjectSettings/ProjectSettings.asset";
        private const string TemplateProductGuid = "3fb6a1b4701e0400b9937be44863ddb0";
        private static readonly string[] TemplateIdentifiers = { "com.LeonHiggin.GameTemplate", "com.Company.ProductName" };

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
        public void ProjectUsesItsOwnIdentity()
        {
            if (PlayerSettings.productName == TemplateProductName) AssertTemplateShipsUnlinked();
            else AssertGameReplacedTheTemplateIdentity();
        }

        /// <summary>In a game (New Game Setup has run): no platform keeps the template's identifier and the product GUID is new.</summary>
        private static void AssertGameReplacedTheTemplateIdentity()
        {
            foreach (var target in new[] { NamedBuildTarget.Android, NamedBuildTarget.iOS, NamedBuildTarget.Standalone })
            {
                var identifier = PlayerSettings.GetApplicationIdentifier(target);
                Assert.IsNotEmpty(identifier, $"{target.TargetName} has no application identifier; run Tools > New Game Setup.");
                CollectionAssert.DoesNotContain(TemplateIdentifiers, identifier, $"{target.TargetName} still uses the template's identifier; run Tools > New Game Setup.");
            }
            var productGuid = Regex.Match(File.ReadAllText(ProjectSettingsPath), @"productGUID: (\w+)").Groups[1].Value;
            Assert.AreNotEqual(TemplateProductGuid, productGuid, "The product GUID is still the template's; run Tools > New Game Setup.");
        }

        /// <summary>In the template itself: no cloud project, Remote Config environments or version-text link.</summary>
        private static void AssertTemplateShipsUnlinked()
        {
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
