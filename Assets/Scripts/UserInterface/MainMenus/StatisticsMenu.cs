using System;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.MainMenus
{
    /// <summary>
    /// This is the base class for the statistics menu
    /// </summary>
    [Serializable]
    public class StatisticsMenu : UserInterface, IUserInterface
    {
        private const FontStyles HighlightedFontStyle = FontStyles.Underline | FontStyles.SmallCaps;
        private const FontStyles NormalFontStyle = FontStyles.SmallCaps;
        private FontStyles GetHighlightedFontStyle(bool state) => state ? HighlightedFontStyle : NormalFontStyle;
        [SerializeField] private StatisticsPanels startingStatisticsPanel;
        [Header("Performance Panel")]
        [SerializeField] private Button performanceButton;
        private TMP_Text performanceTitle;
        [SerializeField] private TMP_Text timePlayed;
        [SerializeField] private TMP_Text timesOpened;
        [SerializeField] private TMP_Text longestSession;
        [SerializeField] private GameObject performanceContent;
        [Header("Achievement Panel")]
        [SerializeField] private AchievementPanels startingAchievementPanel;
        [SerializeField] private Button achievementsButton;
        private TMP_Text achievementsTitle;
        [SerializeField] private GameObject achievementsContent;
        [SerializeField] private Button weeklyAchievementsButton;
        private TMP_Text weeklyAchievementsTitle;
        [SerializeField] private GameObject weeklyAchievementsContent;
        [SerializeField] private Button permanentAchievementsButton;
        private TMP_Text permanentAchievementsTitle;
        [SerializeField] private GameObject permanentAchievementsContent;
        [Header("Leaderboard Panel")]
        [SerializeField] private Button leaderboardButton;
        private TMP_Text leaderboardTitle;
        [SerializeField] private GameObject leaderboardContent;

        public void Initialise()
        {
            AssignVariables();
            SetButtonEvents();
        }

        private void AssignVariables()
        {
            performanceTitle = performanceButton.GetComponent<TMP_Text>();
            achievementsTitle = achievementsButton.GetComponent<TMP_Text>();
            weeklyAchievementsTitle = weeklyAchievementsButton.GetComponent<TMP_Text>();
            permanentAchievementsTitle = permanentAchievementsButton.GetComponent<TMP_Text>();
            leaderboardTitle = leaderboardButton.GetComponent<TMP_Text>();
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
        #region Performance Panel
        private void EnablePerformancePanel(bool state = true)
        {
            EnableAllPanels(!state);
            performanceContent.SetActive(state);
            performanceTitle.fontStyle = GetHighlightedFontStyle(state);
            SetTimePlayedDisplay();
        }

        private void SetTimePlayedDisplay()
        {
            const string prefix = "TIME PLAYED: ";
            timePlayed.text = prefix + PlayerEngagement.TotalPlayTime;
        }
        
        private void SetTimesOpenedDisplay()
        {
            const string prefix = "TIMES OPENED: ";
            timesOpened.text = prefix + PlayerEngagement.TimesGameHasBeenOpened;
        }
        
        private void SetLongestSessionDisplay()
        {
            const string prefix = "LONGEST SESSION: ERROR";
            longestSession.text = prefix;
        }
        #endregion Performance Panel

        #region Achievements Panel
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
            achievementsTitle.fontStyle = GetHighlightedFontStyle(state);
        }
        
        private void EnableWeeklyAchievementsPanel(bool state = true)
        {
            permanentAchievementsContent.SetActive(!state);
            weeklyAchievementsContent.SetActive(state);
            weeklyAchievementsTitle.fontStyle = GetHighlightedFontStyle(state);
            permanentAchievementsTitle.fontStyle = GetHighlightedFontStyle(!state);
        }
        
        private void EnablePermanentAchievementsPanel(bool state = true)
        {
            permanentAchievementsContent.SetActive(state);
            weeklyAchievementsContent.SetActive(!state);
            weeklyAchievementsTitle.fontStyle = GetHighlightedFontStyle(!state);
            permanentAchievementsTitle.fontStyle = GetHighlightedFontStyle(state);
        }
        
        [Serializable]
        private enum AchievementPanels
        {
            Permanent,
            Weekly
        }
        #endregion Achievements Panel

        #region Leaderboard Panel
        private void EnableLeaderBoardPanel(bool state = true)
        {
            EnableAllPanels(!state);
            leaderboardContent.SetActive(state);
            leaderboardTitle.fontStyle = GetHighlightedFontStyle(state);
        }

        private void EnableAllPanels(bool state = true)
        {
            performanceContent.SetActive(state);
            achievementsContent.SetActive(state);
            leaderboardContent.SetActive(state);
            HighlightAllTitles(state);
        }
        #endregion Leaderboard Panel

        private void HighlightAllTitles(bool state = true)
        {
            var fontStyle = GetHighlightedFontStyle(state);
            performanceTitle.fontStyle = fontStyle;
            achievementsTitle.fontStyle = fontStyle;
            leaderboardTitle.fontStyle = fontStyle;
        }
        
        [Serializable]
        private enum StatisticsPanels
        {
            PerformancePanel,
            AchievementsPanel,
            LeaderboardsPanel
        }
    }
}
