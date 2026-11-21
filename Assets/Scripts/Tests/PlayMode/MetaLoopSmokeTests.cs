using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Economy;
using UnityTemplate.Engagement;
using UnityTemplate.Gameplay;
using UnityTemplate.Progression;
using UnityTemplate.Theming;
using UnityTemplate.UserInterface;
using UnityTemplate.UserInterface.ConditionalMenus;
using UnityTemplate.UserInterface.Kit;
using UnityTemplate.UserInterface.MainMenus;
using UnityTemplate.UserInterface.PopUpMenus;
using Object = UnityEngine.Object;

namespace UnityTemplate.Tests.PlayMode
{
    /// <summary>Boots the real game and walks the menu loop: first launch, the daily bonus wheel, the RESET ALL confirmation, starting the first stage and leaving it again; any logged error or exception fails the test.</summary>
    [Category("Smoke")]
    [PrebuildSetup(typeof(RealSaveBackup))]
    [PostBuildCleanup(typeof(RealSaveBackup))]
    public class MetaLoopSmokeTests
    {
        private const string InitialisationScene = "Initialisation";
        private const string TutorialStageScene = "0-0(Tutorial)";
        private const float TimeoutSeconds = 20;
        private static readonly string[] GameRoots = { "GlobalGameManagers", "UserInterface", "DontDestroyOnLoadMarker" };

        [SetUp]
        public void StartWithoutASave()
        {
            RealSaveBackup.DeleteSaves();
        }

        [UnityTearDown]
        public IEnumerator TearDownGameAndDeleteItsSave()
        {
            DOTween.KillAll();
            var marker = new GameObject("DontDestroyOnLoadMarker");
            Object.DontDestroyOnLoad(marker);
            foreach (var root in marker.scene.GetRootGameObjects())
            {
                if (GameRoots.Contains(root.name)) Object.Destroy(root);
            }
            yield return null;
            ServiceLocator.Reset();
            RealSaveBackup.DeleteSaves();
        }

        [UnityTest]
        public IEnumerator FirstLaunchShowsTheMainMenuAndTheWelcomePopUp()
        {
            yield return LoadInitialisation();

            Assert.AreEqual(1, ServiceLocator.Get<EngagementService>().TimesOpened);
            var welcome = default(FirstOpenPopUpMenu);
            yield return WaitUntil(() => (welcome = Object.FindFirstObjectByType<FirstOpenPopUpMenu>()) != null && welcome.State == PopUpState.Open);

            FindButtonByName(welcome.gameObject, "BTN_Continue").onClick.Invoke();
            yield return WaitUntil(() => welcome == null);
        }

        [UnityTest]
        public IEnumerator NextDayTheDailyWheelPaysOutPremiumCredits()
        {
            WriteSave(new SaveData { timesOpened = 1, firstOpenDateTicks = DateTime.Today.AddDays(-1).Ticks, lastDailyBonusDay = DayNumber(DateTime.Today.AddDays(-1)), dailyBonusStreak = 1 });
            yield return LoadInitialisation();

            var daily = default(DailyLogInPopUp);
            yield return WaitUntil(() => (daily = Object.FindFirstObjectByType<DailyLogInPopUp>()) != null && daily.State == PopUpState.Open);
            var proceed = FindButtonByName(daily.gameObject, "BTN_Proceed");
            proceed.onClick.Invoke();
            yield return WaitUntil(() => proceed.interactable, 15);
            proceed.onClick.Invoke();
            yield return WaitUntil(() => daily == null, 15);

            Assert.Greater(ServiceLocator.Get<CreditsService>().GetBalance(Currency.PremiumCredits), 20);
            Assert.IsFalse(ServiceLocator.Get<EngagementService>().IsDailyBonusAvailable(DateTime.Now));
        }

        /// <summary>Opens the RESET ALL confirmation and checks that a tap on BACK behind it lands on the popup, then reaches BACK again once the popup has closed.</summary>
        [UnityTest]
        public IEnumerator ResetAllConfirmationBlocksTapsOnTheMenuBehindIt()
        {
            WriteSave(new SaveData { timesOpened = 3, firstOpenDateTicks = DateTime.Today.AddDays(-2).Ticks, lastDailyBonusDay = DayNumber(DateTime.Today) });
            yield return LoadInitialisation();

            var back = FindLoadedButton("BTN_BackToMainMenu");
            FindLoadedButton("BTN_Settings").onClick.Invoke();
            FindLoadedButton("BTN_DeleteAllData").onClick.Invoke();
            var confirmation = default(ConfirmationPopUp);
            yield return WaitUntil(() => (confirmation = Object.FindFirstObjectByType<ConfirmationPopUp>()) != null && confirmation.State == PopUpState.Open);

            var tappedWhileOpen = TopObjectUnderTap(back.transform.position);
            Assert.IsNotNull(tappedWhileOpen, "Nothing took a tap on BACK while the RESET ALL confirmation was open.");
            Assert.IsTrue(tappedWhileOpen.transform.IsChildOf(confirmation.transform), $"A tap on BACK reached {tappedWhileOpen.name} behind the RESET ALL confirmation.");

            FindButtonByName(confirmation.gameObject, "BTN_No").onClick.Invoke();
            yield return WaitUntil(() => confirmation.State == PopUpState.Hidden);
            yield return null;
            var tappedAfterClosing = TopObjectUnderTap(back.transform.position);
            Assert.IsTrue(tappedAfterClosing != null && tappedAfterClosing.transform.IsChildOf(back.transform), $"After the confirmation closed, a tap on BACK reached {(tappedAfterClosing == null ? "nothing" : tappedAfterClosing.name)}.");
        }

        [UnityTest]
        public IEnumerator StartFadesOutAndLoadsTheTutorialStage()
        {
            WriteSave(new SaveData { timesOpened = 3, firstOpenDateTicks = DateTime.Today.AddDays(-2).Ticks, lastDailyBonusDay = DayNumber(DateTime.Today) });
            yield return LoadInitialisation();

            var start = FindLoadedButton("BTN_PlayButton");
            start.onClick.Invoke();
            var stageLoader = ServiceLocator.Get<StageLoader>();
            yield return WaitUntil(() => stageLoader.IsInStage && !stageLoader.IsLoading, 15);

            Assert.IsTrue(ServiceLocator.TryGet<GameManager>(out _));
            Assert.AreEqual(TutorialStageScene, SceneManager.GetActiveScene().name);
        }

        /// <summary>Starts the tutorial stage, presses BACK and checks that the stage unloads and the main menu shows again.</summary>
        [UnityTest]
        public IEnumerator BackLeavesTheStageForTheMainMenu()
        {
            WriteSave(new SaveData { timesOpened = 3, firstOpenDateTicks = DateTime.Today.AddDays(-2).Ticks, lastDailyBonusDay = DayNumber(DateTime.Today) });
            yield return LoadInitialisation();
            var stageLoader = ServiceLocator.Get<StageLoader>();
            FindLoadedButton("BTN_PlayButton").onClick.Invoke();
            yield return WaitUntil(() => stageLoader.IsInStage && !stageLoader.IsLoading, 15);

            var back = FindLoadedButton("BTN_BackToMainMenu");
            Assert.IsTrue(back.interactable && back.gameObject.activeInHierarchy, "BACK isn't available in the stage.");
            back.onClick.Invoke();
            yield return WaitUntil(() => !stageLoader.IsInStage && !stageLoader.IsLoading, 15);

            Assert.IsFalse(ServiceLocator.TryGet<GameManager>(out _), "The stage's GameManager is still registered.");
            Assert.IsFalse(SceneManager.GetSceneByName(TutorialStageScene).isLoaded);
            Assert.AreEqual(InitialisationScene, SceneManager.GetActiveScene().name);
            Assert.IsTrue(Object.FindFirstObjectByType<MainMenu>().IsVisible);
            Assert.IsFalse(back.interactable, "BACK should be inactive on the main menu.");
        }

        /// <summary>Flips the dark mode switch in Settings and checks that the UI recolours, the choice is saved, and flipping it back returns to light.</summary>
        [UnityTest]
        public IEnumerator DarkModeSwitchRecoloursTheUiAndIsSaved()
        {
            WriteSave(new SaveData { timesOpened = 3, firstOpenDateTicks = DateTime.Today.AddDays(-2).Ticks, lastDailyBonusDay = DayNumber(DateTime.Today) });
            yield return LoadInitialisation();
            var theme = ServiceLocator.Get<ThemeService>();
            var backdrop = Object.FindObjectsByType<Image>(FindObjectsInactive.Include, FindObjectsSortMode.None).Single(image => image.name == "IMG_Backdrop");
            var lightBackground = theme.Get(ThemeColour.Background);
            Assert.AreEqual(ThemeMode.Light, theme.Mode);
            Assert.AreEqual(lightBackground, backdrop.color);

            var darkModeSwitch = Object.FindFirstObjectByType<SettingsMenu>().GetComponentInChildren<ToggleSwitch>(true);
            darkModeSwitch.Toggle.isOn = true;
            yield return null;

            Assert.AreEqual(ThemeMode.Dark, theme.Mode);
            Assert.AreEqual(theme.Get(ThemeColour.Background), backdrop.color);
            Assert.AreNotEqual(lightBackground, backdrop.color);
            StringAssert.Contains("\"darkMode\":true", File.ReadAllText(Path.Combine(Application.persistentDataPath, RealSaveBackup.SaveFileName)));

            darkModeSwitch.Toggle.isOn = false;
            yield return null;
            Assert.AreEqual(ThemeMode.Light, theme.Mode);
            Assert.AreEqual(lightBackground, backdrop.color);
        }

        private static IEnumerator LoadInitialisation()
        {
            yield return SceneManager.LoadSceneAsync(InitialisationScene);
            yield return null;
            Assert.IsTrue(ServiceLocator.TryGet<SaveService>(out _), "GameInitialiser did not register its services.");
        }

        private static IEnumerator WaitUntil(Func<bool> condition, float timeoutSeconds = TimeoutSeconds)
        {
            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (!condition())
            {
                if (Time.realtimeSinceStartup > deadline) Assert.Fail("Timed out waiting for the game.");
                yield return null;
            }
        }

        private static Button FindButtonByName(GameObject root, string name)
        {
            foreach (var button in root.GetComponentsInChildren<Button>(true))
            {
                if (button.name == name) return button;
            }
            Assert.Fail($"No button named {name} under {root.name}.");
            return null;
        }

        /// <summary>Returns the object a tap at this overlay-canvas position would reach first, or null if nothing takes it.</summary>
        private static GameObject TopObjectUnderTap(Vector3 overlayPosition)
        {
            var tap = new PointerEventData(EventSystem.current) { position = RectTransformUtility.WorldToScreenPoint(null, overlayPosition) };
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(tap, hits);
            return hits.Count > 0 ? hits[0].gameObject : null;
        }

        private static Button FindLoadedButton(string name)
        {
            foreach (var button in Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (button.name == name) return button;
            }
            Assert.Fail($"No loaded button is named {name}.");
            return null;
        }

        private static void WriteSave(SaveData data)
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, RealSaveBackup.SaveFileName), JsonUtility.ToJson(data));
        }

        private static int DayNumber(DateTime date)
        {
            return (int)(date.Date.Ticks / TimeSpan.TicksPerDay);
        }
    }
}
