using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityTemplate.Achievements;
using UnityTemplate.Audio;
using UnityTemplate.Economy;
using UnityTemplate.Engagement;
using UnityTemplate.Progression;
using UnityTemplate.UserInterface;

namespace UnityTemplate.Core
{
    /// <summary>Single entry point of the game: creates and registers every service in a fixed order during Awake, then queues the start-up popups.</summary>
    [DefaultExecutionOrder(-1000)]
    public class GameInitialiser : MonoBehaviour
    {
        private const int TweenCapacity = 200;
        private const int SequenceCapacity = 20;

        [Header("Scene objects")]
        [FormerlySerializedAs("objectPool")] [SerializeField] private ObjectPooler objectPooler;
        [SerializeField] private AudioManager audioManager;
        [FormerlySerializedAs("userInterface")] [SerializeField] private UserInterfaceManager userInterfaceManager;

        [Header("Data")]
        [SerializeField] private StageCatalogue stageCatalogue;
        [SerializeField] private AchievementCatalogue achievementCatalogue;

        [Header("Features")]
        [SerializeField] private bool achievementsEnabled;
        [SerializeField] private bool dailyBonusEnabled = true;
        [SerializeField] private int targetFrameRate = 60;

        private SaveService saveService;
        private EngagementService engagementService;

        /// <summary>Creates, registers and initialises every service in order, then counts the app open.</summary>
        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
            DOTween.Init(recycleAllByDefault: false, useSafeMode: true, logBehaviour: LogBehaviour.ErrorsOnly).SetCapacity(TweenCapacity, SequenceCapacity);

            saveService = new SaveService(Application.persistentDataPath);
            var configuration = RemoteConfiguration.LoadCachedConfiguration();
            var credits = new CreditsService(saveService);
            var experience = new ExperienceService(saveService, configuration.AmountOfExperienceLevels);
            var progress = new ProgressService(saveService, stageCatalogue);
            engagementService = new EngagementService(saveService, () => Time.realtimeSinceStartupAsDouble);
            var achievements = new AchievementService(saveService, credits, achievementCatalogue, configuration.DynamicAchievements, achievementsEnabled);

            foreach (var saveable in new ISaveable[] { credits, experience, progress, engagementService, achievements, audioManager })
            {
                saveService.Register(saveable);
            }

            ServiceLocator.Register(saveService);
            ServiceLocator.Register(credits);
            ServiceLocator.Register(experience);
            ServiceLocator.Register(progress);
            ServiceLocator.Register(engagementService);
            ServiceLocator.Register(achievements);
            ServiceLocator.Register(objectPooler);
            ServiceLocator.Register(audioManager);
            ServiceLocator.Register(userInterfaceManager);
            ServiceLocator.Register(new StageLoader(stageCatalogue, userInterfaceManager));

            saveService.Load();
            objectPooler.Initialise();
            audioManager.Initialise();
            userInterfaceManager.Initialise();
            engagementService.RecordAppOpen(DateTime.Now);
            engagementService.ResumeSession();
        }

        /// <summary>Starts the background Remote Config fetch and shows the start-up popups.</summary>
        private async void Start()
        {
            RemoteConfiguration.FetchForNextLaunchAsync();
            await userInterfaceManager.ShowStartUpPopUpsAsync(dailyBonusEnabled);
        }

        /// <summary>Saves and pauses the play-time clock when the app goes to the background, and resumes it on return.</summary>
        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                engagementService.PauseSession();
                saveService.Save();
            }
            else
            {
                engagementService.ResumeSession();
            }
        }

        /// <summary>Saves on quit (desktop and Editor; mobile relies on the pause save).</summary>
        private void OnApplicationQuit()
        {
            engagementService.PauseSession();
            saveService.Save();
        }

        /// <summary>Clears the service registry when the boot objects are destroyed.</summary>
        private void OnDestroy()
        {
            ServiceLocator.Reset();
        }
    }
}
