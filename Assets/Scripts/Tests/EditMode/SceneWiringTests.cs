using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTemplate.Achievements;
using UnityTemplate.Core;
using UnityTemplate.Theming;
using UnityTemplate.UserInterface;

namespace UnityTemplate.Tests
{
    /// <summary>Opens Initialisation.unity additively and checks that every serialized reference the boot sequence needs is assigned and that menus and popups are variants of Menu.prefab and PopUp.prefab.</summary>
    public class SceneWiringTests
    {
        private const string InitialisationScenePath = "Assets/Scenes/Initialisation.unity";
        private const string MenuBasePath = "Assets/Prefabs/UserInterface/Menus/Menu.prefab";
        private const string PopUpBasePath = "Assets/Prefabs/UserInterface/PopUps/PopUp.prefab";
        private static readonly string[] InterFontPaths = { "Assets/Fonts/Inter/Inter-Regular SDF.asset", "Assets/Fonts/Inter/Inter-SemiBold SDF.asset" };
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
            foreach (var field in new[] { "objectPooler", "audioManager", "userInterfaceManager", "menuCamera", "stageCatalogue", "achievementCatalogue", "lightTheme", "darkTheme" })
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

        /// <summary>Checks that UserInterfaceManager has all five menus and that every reference on each menu is assigned.</summary>
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

        /// <summary>Checks that each menu is a scene instance of a variant of Menu.prefab with its script in the variant.</summary>
        [Test]
        public void EveryMenuIsAVariantOfTheMenuBase()
        {
            var userInterface = new SerializedObject(FindInScene<UserInterfaceManager>());
            foreach (var field in MenuFields)
            {
                var menu = userInterface.FindProperty(field).objectReferenceValue as MenuBase;
                Assert.IsNotNull(menu, $"UserInterfaceManager.{field} is not assigned.");
                AssertIsSceneInstanceOfVariant(menu, MenuBasePath);
            }
        }

        /// <summary>Checks that the confirmation popup and the three Addressable popups are variants of PopUp.prefab.</summary>
        [Test]
        public void EveryPopUpIsAVariantOfThePopUpBase()
        {
            var userInterface = new SerializedObject(FindInScene<UserInterfaceManager>());
            var confirmation = userInterface.FindProperty("confirmationScreen").objectReferenceValue as PopUpWindow;
            Assert.IsNotNull(confirmation, "UserInterfaceManager.confirmationScreen is not assigned.");
            AssertIsSceneInstanceOfVariant(confirmation, PopUpBasePath);
            foreach (var field in new[] { "firstOpenPopUp", "dailyLogInPopUp", "anniversaryPopUp" })
            {
                var guid = userInterface.FindProperty(field).FindPropertyRelative("m_AssetGUID").stringValue;
                var popUp = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                Assert.IsNotNull(popUp, $"UserInterfaceManager.{field} does not point at a prefab.");
                Assert.AreEqual(PrefabAssetType.Variant, PrefabUtility.GetPrefabAssetType(popUp), $"{popUp.name} is not a prefab variant.");
                var popUpBase = PrefabUtility.GetCorrespondingObjectFromOriginalSource(popUp);
                Assert.AreEqual(PopUpBasePath, AssetDatabase.GetAssetPath(popUpBase), $"{popUp.name} is not a variant of {PopUpBasePath}.");
            }
        }

        /// <summary>Asserts that a component sits on the root of a scene instance of a variant of the given base prefab, and comes from the variant rather than a scene override.</summary>
        private void AssertIsSceneInstanceOfVariant(Component component, string basePrefabPath)
        {
            var root = component.gameObject;
            Assert.AreEqual(scene, root.scene, $"{root.name} is not an object in {InitialisationScenePath}.");
            Assert.IsTrue(PrefabUtility.IsAnyPrefabInstanceRoot(root), $"{root.name} is not the root of a prefab instance.");
            Assert.AreEqual(PrefabAssetType.Variant, PrefabUtility.GetPrefabAssetType(root), $"{root.name} is not an instance of a prefab variant.");
            Assert.IsFalse(PrefabUtility.IsAddedComponentOverride(component), $"{component.GetType().Name} is added to {root.name} in the scene instead of in its prefab.");
            var basePrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(root);
            Assert.AreEqual(basePrefabPath, AssetDatabase.GetAssetPath(basePrefab), $"{root.name} is not a variant of {basePrefabPath}.");
        }

        /// <summary>Checks that every text in the user interface prefabs and the boot scene uses one of the two Inter font assets.</summary>
        [Test]
        public void EveryUserInterfaceTextUsesInter()
        {
            var inter = InterFontPaths.Select(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>).ToArray();
            Assert.IsTrue(inter.All(font => font != null), "The Inter font assets are missing.");
            var offenders = new List<string>();
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/UserInterface" }))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                offenders.AddRange(prefab.GetComponentsInChildren<TMP_Text>(true).Where(text => !inter.Contains(text.font)).Select(text => prefab.name + "/" + text.name));
            }
            foreach (var root in scene.GetRootGameObjects())
            {
                offenders.AddRange(root.GetComponentsInChildren<TMP_Text>(true).Where(text => !inter.Contains(text.font)).Select(text => root.name + "/" + text.name));
            }
            Assert.IsEmpty(offenders, "These texts do not use Inter: " + string.Join(", ", offenders));
        }

        /// <summary>Checks that the popup references and the stage selection menu's stage item point at Addressable assets.</summary>
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

        /// <summary>Checks that every themed graphic in the user interface prefabs and the boot scene already shows its light-theme colour, so the Editor shows what the game shows.</summary>
        [Test]
        public void ThemedGraphicsShowTheirLightThemeColour()
        {
            var light = (ThemePalette)new SerializedObject(FindInScene<GameInitialiser>()).FindProperty("lightTheme").objectReferenceValue;
            var roots = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/UserInterface" }).Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid))).Concat(scene.GetRootGameObjects());
            var offenders = new List<string>();
            foreach (var root in roots)
            {
                foreach (var themed in root.GetComponentsInChildren<ThemedGraphic>(true))
                {
                    var graphic = themed.GetComponent<UnityEngine.UI.Graphic>();
                    if (graphic == null) offenders.Add($"{root.name}/{themed.name} has no Graphic");
                    else if (graphic.color != light.Get(themed.Colour)) offenders.Add($"{root.name}/{themed.name} is {graphic.color}, not {themed.Colour}");
                }
            }
            Assert.IsEmpty(offenders, string.Join("\n", offenders));
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
