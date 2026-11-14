using System.Collections.Generic;
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
        private const string MenuBasePath = "Assets/Prefabs/UserInterface/Menus/Menu.prefab";
        private const string PopUpBasePath = "Assets/Prefabs/UserInterface/PopUps/PopUp.prefab";
        private static readonly string[] MenuFields = { "mainMenu", "stageSelectionMenu", "statisticsMenu", "settingsMenu", "storeMenu" };
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
        public void EveryMenuAndItsReferencesAreAssigned()
        {
            var userInterface = new SerializedObject(FindInScene<UserInterfaceManager>());
            foreach (var field in MenuFields)
            {
                var menu = userInterface.FindProperty(field).objectReferenceValue as MenuBase;
                Assert.IsNotNull(menu, $"UserInterfaceManager.{field} is not assigned.");
                var property = new SerializedObject(menu).GetIterator();
                while (property.NextVisible(true))
                {
                    if (property.propertyType != SerializedPropertyType.ObjectReference || property.name == "m_Script") continue;
                    Assert.IsNotNull(property.objectReferenceValue, $"{menu.name}.{property.propertyPath} is not assigned.");
                }
            }
        }

        [Test]
        public void EveryMenuIsAVariantOfTheMenuBase()
        {
            var userInterface = new SerializedObject(FindInScene<UserInterfaceManager>());
            foreach (var field in MenuFields)
            {
                var menu = userInterface.FindProperty(field).objectReferenceValue as MenuBase;
                Assert.IsNotNull(menu, $"UserInterfaceManager.{field} is not assigned.");
                Assert.IsTrue(PrefabUtility.IsOutermostPrefabInstanceRoot(menu.gameObject), $"{menu.name} is not an instance of a menu prefab.");
                var menuBase = PrefabUtility.GetCorrespondingObjectFromOriginalSource(menu.gameObject);
                Assert.AreEqual(MenuBasePath, AssetDatabase.GetAssetPath(menuBase), $"{menu.name} is not a variant of {MenuBasePath}.");
            }
        }

        [Test]
        public void EveryPopUpIsAVariantOfThePopUpBase()
        {
            var userInterface = new SerializedObject(FindInScene<UserInterfaceManager>());
            var confirmation = userInterface.FindProperty("confirmationScreen").objectReferenceValue as Component;
            Assert.IsNotNull(confirmation, "UserInterfaceManager.confirmationScreen is not assigned.");
            var popUps = new List<GameObject> { confirmation.gameObject };
            foreach (var field in new[] { "firstOpenPopUp", "dailyLogInPopUp", "anniversaryPopUp" })
            {
                var guid = userInterface.FindProperty(field).FindPropertyRelative("m_AssetGUID").stringValue;
                var popUp = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                Assert.IsNotNull(popUp, $"UserInterfaceManager.{field} does not point at a prefab.");
                popUps.Add(popUp);
            }
            foreach (var popUp in popUps)
            {
                var popUpBase = PrefabUtility.GetCorrespondingObjectFromOriginalSource(popUp);
                Assert.AreEqual(PopUpBasePath, AssetDatabase.GetAssetPath(popUpBase), $"{popUp.name} is not a variant of {PopUpBasePath}.");
            }
        }

        [Test]
        public void PopUpAndStageItemReferencesPointAtAddressableAssets()
        {
            var userInterface = new SerializedObject(FindInScene<UserInterfaceManager>());
            var stageSelectionMenu = userInterface.FindProperty("stageSelectionMenu").objectReferenceValue;
            Assert.IsNotNull(stageSelectionMenu, "UserInterfaceManager.stageSelectionMenu is not assigned.");
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            foreach (var property in new[]
                     {
                         userInterface.FindProperty("firstOpenPopUp"),
                         userInterface.FindProperty("dailyLogInPopUp"),
                         userInterface.FindProperty("anniversaryPopUp"),
                         new SerializedObject(stageSelectionMenu).FindProperty("stageItemPrefab"),
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
