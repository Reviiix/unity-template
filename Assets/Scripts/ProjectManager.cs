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

/// <summary>
/// This class manages features on a whole project scope such as initialsation and..
/// </summary>
public class ProjectManager : Singleton<ProjectManager>
{
    [SerializeField] private BaseAudioManager audioManager;
    [SerializeField] private UserInterfaceManager userInterface;
    [SerializeField] private ObjectPooling objectPool;
    public static Action OnApplicationOpen;
    public static readonly WaitUntil WaitForInitialisation = ProjectInitializer.WaitForAsynchronousInitialisationToComplete;

    protected override void OnEnable() //OnEnable instead of strt to ensure instance is set by 
    {
        base.OnEnable();
        ProjectInitializer.Initialise(()=>OnApplicationOpen?.Invoke());
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save();
        DebuggingAid.Debugging.DisplayDebugMessage("Current Session Time in seconds: " + Time.deltaTime + ", Total Play Time: " + PlayerEngagement.TotalPlayTime);
    }
    
    public static IEnumerator WaitForInitialisationToComplete(Action callBack)
    {
        yield return WaitForInitialisation;
        callBack();
    }

    //Initialising like this seems cumbersome but gives better control over the order of events and helps prevent race conditions
    private static class ProjectInitializer
    {
        public static readonly WaitUntil WaitForAsynchronousInitialisationToComplete = new WaitUntil(() => Initialised);
        private static bool _initialised;
        private static bool Initialised
        {
            get => _initialised;
            // ReSharper disable once ValueParameterNotUsed
            set => _initialised = true;
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
            PlayerEngagement.Initialise();
            PermanentAchievementManager.Initialise();
            DynamicAchievementManager.Initialise();
            callBack();
        }
    
        private static void InitialiseSingletons(Action callBack)
        {
            Instance.objectPool.Initialise(() => //Some classes are dependant on objects being created by the ObjectPooler class which uses the asynchronous addressable system so we need to wait for that to be done before initialising those classes.
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
    }
}


