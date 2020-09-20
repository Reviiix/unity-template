using System;
using Audio;
using PureFunctions;
using Statistics;
using UnityEngine;

public class ProjectManager : MonoBehaviour
{
    public static ProjectManager Instance;
    public BaseAudioManager audioManager;
    public UserInterfaceManager userInterface;
    public ObjectPooling globalObjectPools;
    
    private void Awake()
    {
        if (Instance != null) return;
        
        ProjectSettings.TimesGameHasBeenOpened++;
        Instance = this;
        Initialise();
    }

    private void Initialise()
    {
        Transitions.Initialise(this);
        ScoreTracker.Initialise(userInterface.ReturnScoreText());
        TimeTracker.Initialise(userInterface.ReturnTimeText());
        
        globalObjectPools.Initialise();
        audioManager.Initialise();
        
        //Cleanup after a large start up sequence.
        ClearUnusedAssetsAndCollectGarbage();
    }

    [ContextMenu("Load Main Game")]
    public void LoadMainGame()
    {
        Debugging.DisplayDebugMessage( "Loading main game..");
        SceneTransitionManager.LoadGameScene(() =>
        {
            userInterface.EnableInGameUserInterface();
            GameManager.Instance.StartGame();
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
        });
    }

    private static void ClearUnusedAssetsAndCollectGarbage()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
        Debugging.DisplayDebugMessage("Unused assets have been forcefully unloaded and garbage has been collected manually.");
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
