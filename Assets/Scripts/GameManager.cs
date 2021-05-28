using Abstract;
using Statistics;
using UnityEngine;
using UserInterface;

public class GameManager : Singleton<GameManager>
{
    public TimeTracker GameTime { get; } = new TimeTracker();
    public ScoreTracker ScoreTracker { get; } = new ScoreTracker();
    
    private void Start()
    {
        GameTime.StartTimer(this);
    }
    
    public void IncrementScore(int amount)
    {
        ScoreTracker.IncrementScore(amount);
    }

    [ContextMenu("End Game")]
    public void EndGame()
    {
        GameTime.StopTimer();
        HighScores.SetHighScore(ScoreTracker.Score);
        UserInterfaceManager.EnableGameOverMenu();
    }
}
