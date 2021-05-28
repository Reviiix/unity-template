using System;
using TMPro;

namespace Statistics
{
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
            OnScoreChanged(originalValue, Score);
        }
    }
}
