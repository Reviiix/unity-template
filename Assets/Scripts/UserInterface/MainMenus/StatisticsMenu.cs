using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.MainMenus
{
    [Serializable]
    public class StatisticsMenu : UserInterface, IUserInterface
    {
        private const FontStyles SelectedFontStyle = FontStyles.Underline | FontStyles.SmallCaps;
        private const FontStyles UnSelectedFontStyle = FontStyles.SmallCaps;
        private FontStyles ReturnSelectedFontStyle(bool state) => state ? SelectedFontStyle : UnSelectedFontStyle;
        [SerializeField] private StatisticsPanels startingStatisticsPanel;
        [Header("Performance Panel")]
        [SerializeField] private Button performanceButton;
        private TMP_Text _performanceText;
        [SerializeField] private GameObject performanceContent;
        [Header("Achievement Panel")]
        [SerializeField] private AchievementPanels startingAchievementPanel;
        [SerializeField] private Button achievementsButton;
        private TMP_Text _achievementsText;
        [SerializeField] private GameObject achievementsContent;
        [SerializeField] private Button weeklyAchievementsButton;
        private TMP_Text _weeklyAchievementsText;
        [SerializeField] private GameObject weeklyAchievementsContent;
        [SerializeField] private Button permanentAchievementsButton;
        private TMP_Text _permanentAchievementsText;
        [SerializeField] private GameObject permanentAchievementsContent;
        [Header("Leaderboard Panel")]
        [SerializeField] private Button leaderboardButton;
        private TMP_Text _leaderboardText;
        [SerializeField] private GameObject leaderboardContent;

        public void Initialise()
        {
            AssignVariables();
            SetButtonEvents();
        }

        private void AssignVariables()
        {
            _performanceText = performanceButton.GetComponent<TMP_Text>();
            _achievementsText = achievementsButton.GetComponent<TMP_Text>();
            _weeklyAchievementsText = weeklyAchievementsButton.GetComponent<TMP_Text>();
            _permanentAchievementsText = permanentAchievementsButton.GetComponent<TMP_Text>();
            _leaderboardText = leaderboardButton.GetComponent<TMP_Text>();
        }

        private void SetButtonEvents()
        {
            performanceButton.onClick.AddListener(()=>EnablePerformancePanel());
            achievementsButton.onClick.AddListener(()=>EnableAchievementsPanel());
            weeklyAchievementsButton.onClick.AddListener(()=>EnableWeeklyAchievementsPanel());
            permanentAchievementsButton.onClick.AddListener(()=>EnablePermanentAchievementsPanel());
            leaderboardButton.onClick.AddListener(()=>EnableLeaderBoardPanel());
        }
        
        public void Enable(bool state = true)
        {
            if (state)
            {
                EnableStartingPanel();
            }
            display.enabled = state;
        }

        private void EnableStartingPanel()
        {
            switch (startingStatisticsPanel)
            {
                case StatisticsPanels.PerformancePanel:
                    EnablePerformancePanel();
                    break;
                case StatisticsPanels.AchievementsPanel:
                    EnableAchievementsPanel();
                    break;
                case StatisticsPanels.LeaderboardsPanel:
                    EnableLeaderBoardPanel();
                    break;
                default:
                    Debug.LogError(startingStatisticsPanel + " not accounted for in switch statement.");
                    break;
            }
        }

        private void EnablePerformancePanel(bool state = true)
        {
            EnableAllPanels(!state);
            performanceContent.SetActive(state);
            _performanceText.fontStyle = ReturnSelectedFontStyle(state);
        }

        #region EnableAchievementsPanel
        private void EnableStartingAchievementPanel()
        {
            switch (startingAchievementPanel)
            {
                case AchievementPanels.Permanent:
                    EnablePermanentAchievementsPanel();
                    break;
                case AchievementPanels.Weekly:
                    EnableWeeklyAchievementsPanel();
                    break;
                default:
                    Debug.LogError(startingAchievementPanel + " not accounted for in switch statement.");
                    break;
            }
        }

        private void EnableAchievementsPanel(bool state = true)
        {
            EnableStartingAchievementPanel();
            EnableAllPanels(!state);
            achievementsContent.SetActive(state);
            _achievementsText.fontStyle = ReturnSelectedFontStyle(state);
        }
        
        private void EnableWeeklyAchievementsPanel(bool state = true)
        {
            permanentAchievementsContent.SetActive(!state);
            weeklyAchievementsContent.SetActive(state);
            _weeklyAchievementsText.fontStyle = ReturnSelectedFontStyle(state);
            _permanentAchievementsText.fontStyle = ReturnSelectedFontStyle(!state);
        }
        
        private void EnablePermanentAchievementsPanel(bool state = true)
        {
            permanentAchievementsContent.SetActive(state);
            weeklyAchievementsContent.SetActive(!state);
            _weeklyAchievementsText.fontStyle = ReturnSelectedFontStyle(!state);
            _permanentAchievementsText.fontStyle = ReturnSelectedFontStyle(state);
        }
        #endregion EnableAchievementsPanel

        private void EnableLeaderBoardPanel(bool state = true)
        {
            EnableAllPanels(!state);
            leaderboardContent.SetActive(state);
            _leaderboardText.fontStyle = ReturnSelectedFontStyle(state);
        }

        private void EnableAllPanels(bool state = true)
        {
            performanceContent.SetActive(state);
            achievementsContent.SetActive(state);
            leaderboardContent.SetActive(state);
            HighlightAllTitles(state);
        }

        private void HighlightAllTitles(bool state = true)
        {
            var fontStyle = ReturnSelectedFontStyle(state);
            _performanceText.fontStyle = fontStyle;
            _achievementsText.fontStyle = fontStyle;
            _leaderboardText.fontStyle = fontStyle;
        }
        
        [Serializable]
        private enum StatisticsPanels
        {
            PerformancePanel,
            AchievementsPanel,
            LeaderboardsPanel
        }
        
        [Serializable]
        private enum AchievementPanels
        {
            Permanent,
            Weekly
        }
    }
}
