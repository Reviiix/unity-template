using System;
using Audio;
using PureFunctions;
using Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    [Serializable]
    public class GameOverMenu : Menu, IMenu
    {
        private static Color[] _restartButtonColorSwaps;
        private const string FinalScorePrefix = "FINAL " + ScoreTracker.ScoreDisplayPrefix;
        private const string HighScorePrefix = "HIGH" + ScoreTracker.ScoreDisplayPrefix;
        private const string FinalTimePrefix = "FINAL " + TimeTracker.TimeDisplayPrefix;
        private static readonly Color NewHighScoreTextColour = Color.green;
        private static Color _noHighScoreColor = Color.white;
        [SerializeField]
        private Button restartButton;
        [SerializeField]
        private TMP_Text finalTimeText;
        [SerializeField]
        private TMP_Text finalScoreText;
        [SerializeField]
        private TMP_Text highScoreText;

        public void Initialise(Color[] restartButtonColorSwaps)
        {
            SetNoHighScoreColor(restartButtonColorSwaps);
            AddButtonEvents();
        }

        private void SetNoHighScoreColor(Color[] restartButtonColorSwaps)
        {
            _restartButtonColorSwaps = restartButtonColorSwaps;
            _noHighScoreColor = highScoreText.color;
        }

        private void AddButtonEvents()
        {
            restartButton.onClick.AddListener(ProjectManager.Instance.LoadMainMenu);
            restartButton.onClick.AddListener(BaseAudioManager.PlayButtonClickSound);
            restartButton.onClick.AddListener(ChangeTextColour.StopChangeTextColorSequence);
        }

        public void Enable(bool state = true)
        {
            DisableAllCanvases();
            ProjectManager.Instance.userInterface.EnableInGameUserInterface(false);
            display.enabled = state;

            if (!state) return;
        
            ChangeTextColour.Change(restartButton.GetComponent<TMP_Text>(), _restartButtonColorSwaps[0], _restartButtonColorSwaps[1], ProjectManager.Instance);
            ChangeTextColour.Change(restartButton.GetComponent<TMP_Text>(), _restartButtonColorSwaps[0], _restartButtonColorSwaps[1], ProjectManager.Instance);
        
            ProjectManager.Instance.userInterface.EnablePauseButton(false);
            SetFinalStatisticsDisplay();
        }

        private void SetFinalStatisticsDisplay()
        {
            var score = ScoreTracker.Score;
            var highScore = HighScores.ReturnHighScore();
            var finalTime = TimeTracker.ReturnCurrentTimeAsFormattedString();
        
            finalScoreText.text = FinalScorePrefix + score;
            finalTimeText.text = FinalTimePrefix + finalTime;
            highScoreText.text = HighScorePrefix + highScore;

            if (score >= highScore)
            {
                SetHighScoreColours();
                return;
            }
            SetHighScoreColours(false);
        }

        private void SetHighScoreColours(bool highScoreAchieved = true)
        {
            if (highScoreAchieved)
            {
                ChangeTextColour.Change(new [] {highScoreText, finalScoreText}, NewHighScoreTextColour);
                return;
            }
            ChangeTextColour.Change(new [] {highScoreText, finalScoreText}, _noHighScoreColor);
        }
    }
}
