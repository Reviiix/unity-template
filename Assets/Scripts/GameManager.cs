using Statistics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public BaseAudioManager baseAudioManager;
    public UserInterfaceManager userInterface;
    public ObjectPooling objectPooling;
    
    private void Awake()
    {
        instance = this;
        Initialise();
    }

    private static void Initialise()
    {
        ScoreTracker.Initialise();
        TimeTracker.Initialise();
    }

    public void StartGamePlay()
    {
        Debugging.DisplayDebugMessage("Game play started.");
        SceneTransitionManager.LoadMainScene(() =>
        {
            userInterface.EnableInGameCanvas(true);
            TimeTracker.StartTimer();
        });
    }

    public static void ReloadGame()
    {
        Debugging.DisplayDebugMessage("Game reloading.");
    }
}
