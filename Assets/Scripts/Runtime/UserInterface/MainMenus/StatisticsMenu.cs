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
    public class StatisticsMenu : MenuBase
    {
        [SerializeField] private StatisticsPanels startingStatisticsPanel;
        [Header("Tabs")]
        [SerializeField] private Color selectedTabColour = Color.white;
        [SerializeField] private Color selectedTabTextColour = new(0.08f, 0.09f, 0.11f);
        [SerializeField] private Color unselectedTabTextColour = new(0.36f, 0.39f, 0.44f);
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
            performanceTitle = performanceButton.GetComponentInChildren<TMP_Text>(true);
            achievementsTitle = achievementsButton.GetComponentInChildren<TMP_Text>(true);
            weeklyAchievementsTitle = weeklyAchievementsButton.GetComponentInChildren<TMP_Text>(true);
            permanentAchievementsTitle = permanentAchievementsButton.GetComponentInChildren<TMP_Text>(true);
            leaderboardTitle = leaderboardButton.GetComponentInChildren<TMP_Text>(true);

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

        /// <summary>Shows the performance tab with time played and times opened.</summary>
        private void ShowPerformancePanel()
        {
            ShowOnlyPanel(performanceContent);
            var playTime = engagement.TotalPlayTime;
            textBuilder.Clear().Append("TIME PLAYED: ").Append((int)playTime.TotalHours).Append(':');
            AppendTwoDigits(playTime.Minutes);
            textBuilder.Append(':');
            AppendTwoDigits(playTime.Seconds);
            timePlayed.SetText(textBuilder);
            textBuilder.Clear().Append("TIMES OPENED: ").Append(engagement.TimesOpened);
            timesOpened.SetText(textBuilder);
            longestSession.SetText("LONGEST SESSION: \u2013");
        }

        /// <summary>Appends a number from 0 to 99 as two digits, without allocating a string.</summary>
        private void AppendTwoDigits(int value)
        {
            if (value < 10) textBuilder.Append('0');
            textBuilder.Append(value);
        }

        /// <summary>Shows the achievements tab on its starting sub-tab.</summary>
        private void ShowAchievementsPanel()
        {
            if (startingAchievementPanel == AchievementPanels.Permanent) ShowPermanentAchievements();
            else ShowWeeklyAchievements();
            ShowOnlyPanel(achievementsContent);
        }

        /// <summary>Shows the weekly achievements sub-tab.</summary>
        private void ShowWeeklyAchievements()
        {
            SelectAchievementTab(weekly: true);
        }

        /// <summary>Shows the permanent achievements sub-tab.</summary>
        private void ShowPermanentAchievements()
        {
            SelectAchievementTab(weekly: false);
        }

        /// <summary>Shows one achievements sub-tab and marks it as selected.</summary>
        private void SelectAchievementTab(bool weekly)
        {
            weeklyAchievementsContent.SetActive(weekly);
            permanentAchievementsContent.SetActive(!weekly);
            SetTabSelected(weeklyAchievementsButton, weeklyAchievementsTitle, weekly);
            SetTabSelected(permanentAchievementsButton, permanentAchievementsTitle, !weekly);
        }

        /// <summary>Shows the leaderboard tab.</summary>
        private void ShowLeaderboardPanel()
        {
            ShowOnlyPanel(leaderboardContent);
        }

        /// <summary>Shows one tab's content and marks its tab as selected, hiding the others.</summary>
        private void ShowOnlyPanel(GameObject panel)
        {
            performanceContent.SetActive(panel == performanceContent);
            achievementsContent.SetActive(panel == achievementsContent);
            leaderboardContent.SetActive(panel == leaderboardContent);
            SetTabSelected(performanceButton, performanceTitle, panel == performanceContent);
            SetTabSelected(achievementsButton, achievementsTitle, panel == achievementsContent);
            SetTabSelected(leaderboardButton, leaderboardTitle, panel == leaderboardContent);
        }

        /// <summary>Shows a tab as selected (filled segment, dark label) or not (clear segment, grey label).</summary>
        private void SetTabSelected(Button tab, TMP_Text title, bool selected)
        {
            tab.image.color = selected ? selectedTabColour : Color.clear;
            title.color = selected ? selectedTabTextColour : unselectedTabTextColour;
        }

        /// <summary>The achievements sub-tabs; StatisticsMenu.prefab stores these as numbers, so keep their order.</summary>
        [Serializable]
        private enum AchievementPanels
        {
            Permanent,
            Weekly,
        }

        /// <summary>The statistics tabs; StatisticsMenu.prefab stores these as numbers, so keep their order.</summary>
        [Serializable]
        private enum StatisticsPanels
        {
            PerformancePanel,
            AchievementsPanel,
            LeaderboardsPanel,
        }
    }
}
