using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Engagement;

namespace UnityTemplate.UserInterface.MainMenus
{
    /// <summary>The statistics menu with its performance, achievements (permanent and weekly) and leaderboard tabs.</summary>
    [Serializable]
    public class StatisticsMenu : MenuBase
    {
        private const FontStyles HighlightedFontStyle = FontStyles.Underline | FontStyles.SmallCaps;
        private const FontStyles NormalFontStyle = FontStyles.SmallCaps;
        [SerializeField] private StatisticsPanels startingStatisticsPanel;
        [Header("Performance Panel")]
        [SerializeField] private Button performanceButton;
        [SerializeField] private TMP_Text timePlayed;
        [SerializeField] private TMP_Text timesOpened;
        [SerializeField] private TMP_Text longestSession;
        [SerializeField] private GameObject performanceContent;
        [Header("Achievement Panel")]
        [SerializeField] private AchievementPanels startingAchievementPanel;
        [SerializeField] private Button achievementsButton;
        [SerializeField] private GameObject achievementsContent;
        [SerializeField] private Button weeklyAchievementsButton;
        [SerializeField] private GameObject weeklyAchievementsContent;
        [SerializeField] private Button permanentAchievementsButton;
        [SerializeField] private GameObject permanentAchievementsContent;
        [Header("Leaderboard Panel")]
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private GameObject leaderboardContent;
        private readonly StringBuilder textBuilder = new();
        private TMP_Text performanceTitle;
        private TMP_Text achievementsTitle;
        private TMP_Text weeklyAchievementsTitle;
        private TMP_Text permanentAchievementsTitle;
        private TMP_Text leaderboardTitle;
        private EngagementService engagement;

        /// <summary>Caches the tab titles and wires the tab buttons.</summary>
        public void Initialise()
        {
            engagement = ServiceLocator.Get<EngagementService>();
            performanceTitle = performanceButton.GetComponent<TMP_Text>();
            achievementsTitle = achievementsButton.GetComponent<TMP_Text>();
            weeklyAchievementsTitle = weeklyAchievementsButton.GetComponent<TMP_Text>();
            permanentAchievementsTitle = permanentAchievementsButton.GetComponent<TMP_Text>();
            leaderboardTitle = leaderboardButton.GetComponent<TMP_Text>();

            performanceButton.onClick.AddListener(ShowPerformancePanel);
            achievementsButton.onClick.AddListener(ShowAchievementsPanel);
            weeklyAchievementsButton.onClick.AddListener(ShowWeeklyAchievements);
            permanentAchievementsButton.onClick.AddListener(ShowPermanentAchievements);
            leaderboardButton.onClick.AddListener(ShowLeaderboardPanel);
        }

        /// <summary>Shows the menu on its starting tab.</summary>
        public override void Show()
        {
            switch (startingStatisticsPanel)
            {
                case StatisticsPanels.PerformancePanel:
                    ShowPerformancePanel();
                    break;
                case StatisticsPanels.AchievementsPanel:
                    ShowAchievementsPanel();
                    break;
                case StatisticsPanels.LeaderboardsPanel:
                    ShowLeaderboardPanel();
                    break;
            }
            base.Show();
        }

        private void ShowPerformancePanel()
        {
            ShowOnlyPanel(performanceContent, performanceTitle);
            var playTime = engagement.TotalPlayTime;
            textBuilder.Clear().Append("TIME PLAYED: ").Append(playTime.ToString());
            timePlayed.SetText(textBuilder);
            textBuilder.Clear().Append("TIMES OPENED: ").Append(engagement.TimesOpened);
            timesOpened.SetText(textBuilder);
            longestSession.SetText("LONGEST SESSION: ");
        }

        private void ShowAchievementsPanel()
        {
            if (startingAchievementPanel == AchievementPanels.Permanent) ShowPermanentAchievements();
            else ShowWeeklyAchievements();
            ShowOnlyPanel(achievementsContent, achievementsTitle);
        }

        private void ShowWeeklyAchievements()
        {
            SelectAchievementTab(weekly: true);
        }

        private void ShowPermanentAchievements()
        {
            SelectAchievementTab(weekly: false);
        }

        private void SelectAchievementTab(bool weekly)
        {
            weeklyAchievementsContent.SetActive(weekly);
            permanentAchievementsContent.SetActive(!weekly);
            weeklyAchievementsTitle.fontStyle = weekly ? HighlightedFontStyle : NormalFontStyle;
            permanentAchievementsTitle.fontStyle = weekly ? NormalFontStyle : HighlightedFontStyle;
        }

        private void ShowLeaderboardPanel()
        {
            ShowOnlyPanel(leaderboardContent, leaderboardTitle);
        }

        private void ShowOnlyPanel(GameObject panel, TMP_Text title)
        {
            performanceContent.SetActive(panel == performanceContent);
            achievementsContent.SetActive(panel == achievementsContent);
            leaderboardContent.SetActive(panel == leaderboardContent);
            performanceTitle.fontStyle = title == performanceTitle ? HighlightedFontStyle : NormalFontStyle;
            achievementsTitle.fontStyle = title == achievementsTitle ? HighlightedFontStyle : NormalFontStyle;
            leaderboardTitle.fontStyle = title == leaderboardTitle ? HighlightedFontStyle : NormalFontStyle;
        }

        [Serializable]
        private enum AchievementPanels
        {
            Permanent,
            Weekly,
        }

        [Serializable]
        private enum StatisticsPanels
        {
            PerformancePanel,
            AchievementsPanel,
            LeaderboardsPanel,
        }
    }
}
