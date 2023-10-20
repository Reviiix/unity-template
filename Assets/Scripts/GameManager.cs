using Abstract;
using Statistics;
using UnityEngine;
using UserInterface;

/// <summary>
/// This class manages the flow of the game.
/// Puppet Master
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public TimeTracker GameTime { get; } = new();
    public ScoreTracker ScoreTracker { get; } = new();
    
    private void Start()
    {
        GameTime.StartTimer();
    }
    
    [ContextMenu(nameof(PauseUnpauseTimer))]
    public void PauseUnpauseTimer()
    {
        if (GameTime.Active)
        {
            GameTime.PauseTimer();
            return;
        }
        GameTime.ResumeTimer();
    }
    
    public void IncrementScore(int amount)
    {
        ScoreTracker.IncrementScore(amount);
    }

    [ContextMenu(nameof(EndGame))]
    public void EndGame()
    {
        GameTime.StopTimer();
        HighScores.SetHighScore(ScoreTracker.Score);
        UserInterfaceManager.EnableGameOverMenu();
    }
}
