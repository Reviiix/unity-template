using UnityEditor;
using UnityEngine;

namespace Statistics
{
    public static class HighScores
    {
        private static readonly string Key = PlayerSettings.productName + "HighScore";
    
        public static int ReturnHighScore()
        {
            if (PlayerPrefs.HasKey(Key))
            {
                return PlayerPrefs.GetInt(Key);
            }
            Debug.LogWarning("No previous high score found with key: " + Key + ". This may be the first time the game is played.");
            return 0;
        }

        public static void SetHighScore(int currentScore)
        {
            if (CurrentScoreBeatsHighScore(currentScore))
            {
                PlayerPrefs.SetInt(Key, currentScore);
            }
        }
    
        private static bool CurrentScoreBeatsHighScore(int currentScore)
        {
            return currentScore >= ReturnHighScore();
        }
    }
}
