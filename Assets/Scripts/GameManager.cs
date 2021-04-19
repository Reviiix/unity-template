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
        StartGameTimer();
    }
    
    public void IncrementScore()
    {
        ScoreTracker.IncrementScore(0);
    }
    
    public void StartGameTimer()
    {
        TimeTracker.StartTimer(ProjectManager.Instance);
    }
    
    public void EndGame()
    {
        TimeTracker.StopTimer();
        HighScores.SetHighScore(ScoreTracker.Score);
        ProjectManager.Instance.userInterface.EnableGameOverMenu();
    }
}
