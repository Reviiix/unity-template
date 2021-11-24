using System;
using System.Collections;
using Abstract;
using Achievements;
using Audio;
using Credits;
using Player;
using Statistics;
using Statistics.Experience;
using UnityEngine;
using UserInterface;

public class ProjectManager : Singleton<ProjectManager>
{
    [SerializeField] private BaseAudioManager audioManager;
    [SerializeField] private UserInterfaceManager userInterface;
    [SerializeField] private ObjectPooling objectPool;
    public static Action OnApplicationOpen;

    protected override void OnEnable()
    {
        base.OnEnable();
        ProjectInitializer.Initialise(()=>OnApplicationOpen?.Invoke());
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save();
        DebuggingAid.Debugging.DisplayDebugMessage("Current Session Time in seconds: " + Time.deltaTime + ", Total Play Time: " + PlayerEngagementManager.TotalPlayTime);
    }
    
    public static IEnumerator WaitForAnyAsynchronousInitialisationToComplete(Action callBack)
    {
        yield return ProjectInitializer.WaitForAnyAsynchronousInitialisation(callBack);
    }

    private static class ProjectInitializer
    {
        private static bool _initialised;
        public static bool Initialised
        {
            get => _initialised;
            // ReSharper disable once ValueParameterNotUsed
            private set => _initialised = true;
        }

        public static void Initialise(Action callBack)
        {
            if (Initialised) return;

            InitialiseStaticManagers(() =>
            {
                InitialiseSingletons(() =>
                {
                    RemoteConfigurationManager.UpdateConfiguration();
                    SaveSystem.Load();
                    Initialised = true;
                    callBack();
                });
            });
        }

        private static void InitialiseStaticManagers(Action callBack)
        {
            RemoteConfigurationManager.Initialise();
            GameStatistics.Initialise();
            HolidayManager.Initialise();
            ExperienceManager.Initialise();
            CreditsManager.Initialise();
            CameraManager.Initialise();
            PlayerEngagementManager.Initialise();
            PermanentAchievementManager.Initialise();
            DynamicAchievementManager.Initialise();
            callBack();
        }
    
        private static void InitialiseSingletons(Action callBack)
        {
            var instance = Instance;
            instance.objectPool.Initialise(() => //Some classes are dependant on objects being created by the ObjectPooler class which uses the asynchronous addressable system so we need to wait for that to be done before initialising those classes.
            {
                instance.audioManager.Initialise();
                instance.userInterface.Initialise();
                callBack();
            });
        }
        
        public static IEnumerator WaitForAnyAsynchronousInitialisation(Action callBack)
        {
            yield return new WaitUntil(() => Initialised);
            callBack();
        }
    }

    public struct ProjectInformation
    {
        private const string GitHubURL = "https://github.com/Reviiix/unity-template";
        private const string DiscordServer = "https://github.com/Reviiix/unity-template";
    }
}


