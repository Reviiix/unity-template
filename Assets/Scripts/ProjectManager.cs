using System.Linq;
using Achievements;
using Audio;
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
    [SerializeField]
    private AchievementManager achievementManager;
    
    private void Awake()
    {
        if (Instance != null) return;

        SetActiveCamera();
        
        IncrementOpenAmount();
        
        Instance = this;
        Initialise();
        
        Debug.Log(AchievementManager.Achievements.Last().Index);
    }

    private static void IncrementOpenAmount()
    {
        ProjectSettings.TimesGameHasBeenOpened++;
    }

    private static void SetActiveCamera()
    {
        //Camera.main is an expensive invocation, use sparingly.
        _activeCamera = Camera.main;
    }

    private void Initialise()
    {
        Transitions.Initialise(this);
        
        ScoreTracker.Initialise(userInterface.ReturnScoreText());
        TimeTracker.Initialise(userInterface.ReturnTimeText());
        
        globalObjectPools.Initialise();
        audioManager.Initialise();
        achievementManager.Initialise();
        
        //Cleanup after a large start up sequence.
        Debugging.ClearUnusedAssetsAndCollectGarbage();
    }

    [ContextMenu("Load Main Game")]
    public void LoadMainGame()
    {
        Debugging.DisplayDebugMessage( "Loading main game.");
        SceneTransitionManager.LoadGameScene(() =>
        {
            userInterface.EnableInGameUserInterface();
            SetActiveCamera();
        });
    }

    [ContextMenu("Load Main Menu")]
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
}

public struct ProjectSettings
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

    public static float CurrentVolume => VolumeControls.VolumeLevel;
    public static bool PersistantObjectsInitialisedPreviously = false;
}
