using TMPro;

namespace Statistics
{
    public static class ScoreTracker
    {
        private static TMP_Text _scoreDisplay;
        private const string ScoreDisplayPrefix = "SCORE: ";
        private static readonly int[] IncrementAmounts = {1, 3, 5, 10, 20};
        public static int Score
        {
            get;
            private set;
        }

        public static void Initialise()
        {
            _scoreDisplay = GameManager.instance.userInterface.ReturnScoreText();
            Reset();
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
