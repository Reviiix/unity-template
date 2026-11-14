using System;
using System.Collections;
using System.Linq;
using System.IO;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Economy;
using UnityTemplate.Engagement;
using UnityTemplate.Gameplay;
using UnityTemplate.UserInterface;
using UnityTemplate.UserInterface.ConditionalMenus;
using Object = UnityEngine.Object;

namespace UnityTemplate.Tests.PlayMode
{
    /// <summary>Boots the real game and walks the menu loop: first launch, the daily bonus wheel and starting the first stage; any logged error or exception fails the test.</summary>
    public class MetaLoopSmokeTests
    {
        private const string InitialisationScene = "Initialisation";
        private const string SaveFileName = "save.json";
        private const float TimeoutSeconds = 20;
        private static readonly string[] GameRoots = { "GlobalGameManagers", "UserInterface", "DontDestroyOnLoadMarker" };
        private string backupFolder;

        [SetUp]
        public void BackUpRealSave()
        {
            backupFolder = Path.Combine(Application.temporaryCachePath, "SaveBackup" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(backupFolder);
            foreach (var path in Directory.GetFiles(Application.persistentDataPath, SaveFileName + "*"))
            {
                File.Move(path, Path.Combine(backupFolder, Path.GetFileName(path)));
            }
        }

        [UnityTearDown]
        public IEnumerator TearDownGameAndRestoreSave()
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

            foreach (var path in Directory.GetFiles(Application.persistentDataPath, SaveFileName + "*")) File.Delete(path);
            foreach (var path in Directory.GetFiles(backupFolder)) File.Move(path, Path.Combine(Application.persistentDataPath, Path.GetFileName(path)));
            Directory.Delete(backupFolder, true);
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

        [UnityTest]
        public IEnumerator StartFadesOutAndLoadsTheTutorialStage()
        {
            WriteSave(new SaveData { timesOpened = 3, firstOpenDateTicks = DateTime.Today.AddDays(-2).Ticks, lastDailyBonusDay = DayNumber(DateTime.Today) });
            yield return LoadInitialisation();

            var start = FindLoadedButton("BTN_PlayButton");
            start.onClick.Invoke();
            yield return WaitUntil(() => ServiceLocator.TryGet<GameManager>(out _), 15);

            Assert.AreEqual("0-0(Tutorial)", SceneManager.GetActiveScene().name);
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
            File.WriteAllText(Path.Combine(Application.persistentDataPath, SaveFileName), JsonUtility.ToJson(data));
        }

        private static int DayNumber(DateTime date)
        {
            return (int)(date.Date.Ticks / TimeSpan.TicksPerDay);
        }
    }
}
