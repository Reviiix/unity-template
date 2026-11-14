using NUnit.Framework;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTemplate.Achievements;
using UnityTemplate.Core;
using UnityTemplate.UserInterface;

namespace UnityTemplate.Tests
{
    /// <summary>Opens Initialisation.unity additively and checks that every serialized reference the boot sequence needs is assigned.</summary>
    public class SceneWiringTests
    {
        private const string InitialisationScenePath = "Assets/Scenes/Initialisation.unity";
        private Scene scene;
        private bool openedByTest;

        [SetUp]
        public void OpenScene()
        {
            scene = SceneManager.GetSceneByPath(InitialisationScenePath);
            openedByTest = !scene.isLoaded;
            if (openedByTest) scene = EditorSceneManager.OpenScene(InitialisationScenePath, OpenSceneMode.Additive);
        }

        [TearDown]
        public void CloseScene()
        {
            if (openedByTest) EditorSceneManager.CloseScene(scene, true);
        }

        [Test]
        public void GameInitialiserReferencesAreAssigned()
        {
            var initialiser = new SerializedObject(FindInScene<GameInitialiser>());
            foreach (var field in new[] { "objectPooler", "audioManager", "userInterfaceManager", "stageCatalogue", "achievementCatalogue" })
            {
                Assert.IsNotNull(initialiser.FindProperty(field).objectReferenceValue, $"GameInitialiser.{field} is not assigned.");
            }
        }

        [Test]
        public void EveryPoolHasAPrefab()
        {
            var pools = new SerializedObject(FindInScene<ObjectPooler>()).FindProperty("pools");
            Assert.Greater(pools.arraySize, 0);
            for (var i = 0; i < pools.arraySize; i++)
            {
                Assert.IsNotNull(pools.GetArrayElementAtIndex(i).FindPropertyRelative("prefab").objectReferenceValue, $"Pool {i} has no prefab.");
            }
        }

        [Test]
        public void PopUpAndStageItemReferencesPointAtAddressableAssets()
        {
            var userInterface = new SerializedObject(FindInScene<UserInterfaceManager>());
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            foreach (var property in new[]
                     {
                         userInterface.FindProperty("firstOpenPopUp"),
                         userInterface.FindProperty("dailyLogInPopUp"),
                         userInterface.FindProperty("anniversaryPopUp"),
                         userInterface.FindProperty("stageSelectionMenu").FindPropertyRelative("stageItemPrefab"),
                     })
            {
                var guid = property.FindPropertyRelative("m_AssetGUID").stringValue;
                Assert.IsFalse(string.IsNullOrEmpty(guid), $"{property.propertyPath} is not assigned.");
                Assert.IsNotNull(settings.FindAssetEntry(guid), $"{property.propertyPath} points at an asset that is not Addressable.");
            }
        }

        [Test]
        public void AchievementListsHaveAnItemPrefab()
        {
            foreach (var list in scene.GetRootGameObjects())
            {
                foreach (var display in list.GetComponentsInChildren<AchievementListDisplay>(true))
                {
                    Assert.IsNotNull(new SerializedObject(display).FindProperty("listItemPrefab").objectReferenceValue, $"{display.name} has no list item prefab.");
                }
            }
        }

        private T FindInScene<T>() where T : Component
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var component = root.GetComponentInChildren<T>(true);
                if (component != null) return component;
            }
            Assert.Fail($"{typeof(T).Name} is missing from {InitialisationScenePath}.");
            return null;
        }
    }
}
