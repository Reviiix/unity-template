using System;
using System.ComponentModel.Design;
using Statistics;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine.SceneManagement;

namespace UserInterface.InGameMenus
{
    [Serializable]
    public class InGameUserInterface : UserInterface, IUserInterface
    {
        private const string ScoreDisplayPrefix = ScoreTracker.ScoreDisplayPrefix;
        public TMP_Text timeText;
        public TMP_Text scoreText;

        public void Initialise()
        {
            SceneManager.sceneLoaded += AssignEvents;
        }

        private void AssignEvents(Scene scene, LoadSceneMode loadSceneMode)
        {
            GameManager.Instance.ScoreTracker.OnScoreChanged += UpdateScoreDisplay;
        }

        private void UpdateScoreDisplay(int oldValue, int newValue)
        {
            scoreText.text = ScoreDisplayPrefix + newValue;
        }

        public void Enable(bool state = true)
        {
            display.enabled = state;
        }
    }
}
