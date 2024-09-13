using System;
using System.Collections;
using Abstract;
using Achievements;
using Achievements.Dynamic;
using Achievements.Permanent;
using Audio;
using Credits;
using Player;
using PureFunctions.UnitySpecific;
using Statistics;
using Statistics.Experience;
using UnityEngine;
using UserInterface;

/// <summary>
/// This class manages features on a whole project scope such as initialisation, enabled features and..
/// </summary>
public class ProjectManager : Singleton<ProjectManager>
{
    [SerializeField] private BaseAudioManager audioManager;
    [SerializeField] private UserInterfaceManager userInterface;
    [SerializeField] private ObjectPooler objectPool;
    public static Action OnApplicationOpen;
    public static bool Initialised => ProjectInitializer.Complete;

    protected override void OnEnable() //OnEnable instead of Start/Awake to ensure instance is set by base class.
    {
        base.OnEnable();
        ProjectInitializer.Initialise(()=>OnApplicationOpen?.Invoke());
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save();
        DebuggingAid.DebugLogManager.Log("Current Session Time in seconds: " + Time.deltaTime + ".\nTotal Play Time: " + PlayerEngagement.TotalPlayTime);
    }

    /// <summary>
    /// Initialising like this seems cumbersome but gives better control over the order of events and helps prevent race conditions
    /// </summary>
    private static class ProjectInitializer
    {
        public static bool Complete { get; private set; }

        public static void Initialise(Action callBack)
        {
            if (Complete) return;
            
            InitialiseStaticManagers(() =>
            {
                InitialiseSingletons(() =>
                {
                    RemoteConfigurationManager.UpdateConfiguration();
                    SaveSystem.Load();
                    Complete = true;
                    callBack();
                });
            });
        }

        /// <summary>
        /// Initialising like this instead of through the static constructor gives greater control over the order of execution.
        /// </summary>
        /// <param name="callBack">
        /// /// Action is intended to start any other initialisation that is dependent on the static classes.
        /// </param>
        private static void InitialiseStaticManagers(Action callBack)
        {
            RemoteConfigurationManager.Initialise();
            PlayerStatistics.Initialise();
            HolidayManager.Initialise();
            ExperienceManager.Initialise();
            CreditsManager.Initialise();
            CameraManager.Initialise();
            PlayerEngagement.Initialise();
            AchievementManager.Initialise();
            callBack();
        }
    
        /// <summary>
        /// Some classes are dependant on objects being created by the ObjectPooler class which uses the asynchronous addressable system so we need to wait for that to be done before initialising those classes.
        /// </summary>
        /// <param name="callBack">
        /// /// Action is intended to start any other initialisation that is dependent on the singleton classes.
        /// </param>
        private static void InitialiseSingletons(Action callBack)
        {
            Instance.objectPool.Initialise(() =>
            {
                Instance.audioManager.Initialise();
                Instance.userInterface.Initialise();
                callBack();
            });
        }
    }

    public struct ProjectInformation
    {
        private const string GitHubURL = "https://github.com/Reviiix/unity-template";
        private const string DiscordServer = "https://github.com/Reviiix/unity-template";
        private const string SupportEmail = "fuckoff@live.co.uk";
    }
    
    public struct EnabledFeatures
    {
        public const bool DebugMessages = true;
        public const bool Achievements = false;
        public const bool DailyBonus = true;
        public const bool SaveWithBinaryFormatter = true; //Alternative is JSON.
    }
}


