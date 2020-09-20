using TMPro;

namespace Statistics
{
    public static class ScoreTracker
    {
        private static TMP_Text _scoreDisplay;
        private static readonly int[] IncrementAmounts = {1, 2, 3, 5, 10, 20};
        public const string ScoreDisplayPrefix = "SCORE: ";
        public static int Score
        {
            get;
            private set;
        }

        public static void Initialise(TMP_Text scoreDisplay)
        {
            ResolveDependencies(scoreDisplay);
            Reset();
        }

        private static void ResolveDependencies(TMP_Text scoreDisplay)
        {
            _scoreDisplay = scoreDisplay;
        }

        public static void IncrementScore(int amountIndex)
        {
            Score += IncrementAmounts[amountIndex];
            UpdateScoreDisplay();
        }

        private static void UpdateScoreDisplay()
        {
            _scoreDisplay.text = ScoreDisplayPrefix + Score;
        }
        
        private static void Reset()
        {
            Score = 0;
            UpdateScoreDisplay();
        }
    }
}
