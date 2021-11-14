using System;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.MainMenus
{
    [Serializable]
    public class StatisticsMenu : UserInterface, IUserInterface
    {
        [SerializeField] private MyEnum startingContentPanel;
        [SerializeField] private Button performanceButton;
        [SerializeField] private GameObject performanceContent;
        [SerializeField] private Button achievementsButton;
        [SerializeField] private GameObject achievementsContent;
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private GameObject leaderboardContent;

        public void Initialise()
        {
            SetButtonEvents();
        }

        private void SetButtonEvents()
        {
            achievementsButton.onClick.AddListener(EnableAchievementsPanel);
            performanceButton.onClick.AddListener(EnablePerformancePanel);
            leaderboardButton.onClick.AddListener(EnableLeaderBoardPanel);
        }
        
        public void Enable(bool state = true)
        {
            if (state)
            {
                EnableStartingContent();
            }
            display.enabled = state;
        }

        private void EnableStartingContent()
        {
            switch (startingContentPanel)
            {
                case MyEnum.PerformancePanel:
                    EnablePerformancePanel();
                    break;
                case MyEnum.AchievementsPanel:
                    EnableAchievementsPanel();
                    break;
                case MyEnum.LeaderboardsPanel:
                    EnableLeaderBoardPanel();
                    break;
                default:
                    Debug.LogError(startingContentPanel + " not accounted for in switch statement.");
                    break;
            }
        }

        private void EnableAchievementsPanel()
        {
            EnableAllPanels(false);
            achievementsContent.SetActive(true);
        }
        
        private void EnablePerformancePanel()
        {
            EnableAllPanels(false);
            performanceContent.SetActive(true);
        }
        
        private void EnableLeaderBoardPanel()
        {
            EnableAllPanels(false);
            leaderboardContent.SetActive(true);
        }

        private void EnableAllPanels(bool state = true)
        {
            achievementsContent.SetActive(state);
            performanceContent.SetActive(state);
            leaderboardContent.SetActive(state);
        }
        
        [Serializable]
        private enum MyEnum
        {
            PerformancePanel,
            AchievementsPanel,
            LeaderboardsPanel
        }
    }
}
