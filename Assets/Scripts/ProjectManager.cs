using System.Collections;
using System.Linq;
using Achievements;
using Audio;
using Experience;
using PureFunctions;
using Statistics;
using UnityEngine;
using UserInterface;

public class ProjectManager : MonoBehaviour
{
    private static Camera _activeCamera;
    public static ProjectManager Instance;
    public BaseAudioManager audioManager;
    public UserInterfaceManager userInterface;
    public ObjectPooling globalObjectPools;

    private void Awake()
    {
        if (Instance != null) return;

        SetActiveCamera();
        
        IncrementOpenAmount();
        
        Initialise();
    }
    
    private void OnApplicationQuit()
    {
        SaveSystem.SavePlayer();
    }

    private static void IncrementOpenAmount()
    {
        ProjectStatistics.TimesGameHasBeenOpened++;
    }

    private static void SetActiveCamera()
    {
        //Camera.main is an expensive invocation, use sparingly.
        _activeCamera = Camera.main;
    }

    private void Initialise()
    {
        Instance = this;
        
        Transitions.Initialise(this);
        PlayerInformation.Initialise();
        ScoreTracker.Initialise(userInterface.ReturnScoreText());
        TimeTracker.Initialise(userInterface.ReturnTimeText());
        ExperienceManager.Initialise();
        
        globalObjectPools.Initialise();
        audioManager.Initialise();

        //Cleanup after a large start up sequence.
        //Debugging.ClearUnusedAssetsAndCollectGarbage();
    }
    
    public void LoadMainGame()
    {
        Debugging.DisplayDebugMessage( "Loading main game.");
        SceneTransitionManager.LoadGameScene(() =>
        {
            userInterface.EnableInGameUserInterface();
            SetActiveCamera();
        });
    }
    
    public void LoadMainMenu()
    {
        Debugging.DisplayDebugMessage("Loading main menu.");
        userInterface.EnableGameOverMenu(false);
        SceneTransitionManager.LoadInitialisationScene(() =>
        {
            userInterface.EnableStartMenu();
            SetActiveCamera();
        });
    }

    public static int ReturnScreenWidth()
    {
        return _activeCamera.pixelWidth;
    }
    
    public static void Wait(float time, System.Action callBack)
    {
        Instance.StartCoroutine(PureFunctions.Wait.WaitThenCallBack(time, callBack));
    }

    [ContextMenu("Reset Player Information")]
    public void ResetPlayerInformation()
    {
        Debugging.ResetPlayerInformation();
        SaveSystem.SavePlayer();
    }
}

public struct ProjectSettings
{
    public static float CurrentVolume => VolumeControls.VolumeLevel;
    public static bool PersistantObjectsInitialisedPreviously = false;
}

public struct ProjectStatistics
{
    #region TimesGameHasBeenOpened
    private const string TimesGameHasBeenOpenedKey = "TimesGameHasBeenOpened";
    private static int _timesGameHasBeenOpened;
    public static int TimesGameHasBeenOpened
    {
        get
        {
            var v = PlayerPrefs.HasKey(TimesGameHasBeenOpenedKey) ? PlayerPrefs.GetInt(TimesGameHasBeenOpenedKey) : 0;
            Debugging.DisplayDebugMessage("The game has been opened " + v + " times.");
            _timesGameHasBeenOpened = v;
            return v;
        } 
        // ReSharper disable once ValueParameterNotUsed
        set
        {
            _timesGameHasBeenOpened++;
            PlayerPrefs.SetInt(TimesGameHasBeenOpenedKey, _timesGameHasBeenOpened);
        }
    }
    #endregion TimesGameHasBeenOpened
}


