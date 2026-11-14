using System;

namespace UnityTemplate.Gameplay
{
    /// <summary>Tracks the score of the current stage and announces each change.</summary>
    public class ScoreTracker
    {
        public int Score { get; private set; }
        public event Action<int, int> ScoreChanged;

        /// <summary>Adds points and raises ScoreChanged with the old and new score.</summary>
        public void Increment(int amount)
        {
            var oldScore = Score;
            Score += amount;
            ScoreChanged?.Invoke(oldScore, Score);
        }
    }
}
