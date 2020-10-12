using Statistics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        
        //Cleanup after a large start up sequence.
        Debugging.ClearUnusedAssetsAndCollectGarbage();
    }

    private void Start()
    {
        StartGame();
    }
    
    [ContextMenu("Increment Score")]
    public void IncrementScore()
    {
        ScoreTracker.IncrementScore(0);
    }
    
    [ContextMenu("Start Game")]
    public void StartGame()
    {
        TimeTracker.StartTimer(ProjectManager.Instance);
    }

    [ContextMenu("End Game")]
    public void EndGame()
    {
        TimeTracker.StopTimer();
        HighScores.SetHighScore(ScoreTracker.Score);
        ProjectManager.Instance.userInterface.EnableGameOverMenu();
    }
}
