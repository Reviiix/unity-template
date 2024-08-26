using System;

namespace Statistics
{
    /// <summary>
    /// This class tracks the player score.
    /// </summary>
    public class ScoreTracker
    {
        public const string ScoreDisplayPrefix = "SCORE: ";
        public Action<int, int> OnScoreChanged;
        
        public int Score
        {
            get;
            private set;
        }

        public void IncrementScore(int amount)
        {
            var originalValue = Score;
            Score += amount;
            OnScoreChanged?.Invoke(originalValue, Score);
        }
    }
}
