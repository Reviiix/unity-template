using System;
using Audio;
using Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.MainMenus
{
    [Serializable]
    public class MainMenu : UserInterface, IUserInterface
    {
        [SerializeField] private Button startGame;
        [SerializeField] private Button openStageSelectionMenuButton;
        [SerializeField] private Button openStatisticsMenuButton;
        [SerializeField] private Button openSettingsMenuButton;
        [SerializeField] private Button openStoreMenuButton;

        public void Initialise(Action openStageSelectionMenu, Action openStatisticsMenu, Action openSettingsPage, Action openStorePage)
        {
            AddButtonEvents(openStageSelectionMenu, openStatisticsMenu, openSettingsPage, openStorePage);
            SetStartGameText();
        }

        private void AddButtonEvents(Action openStageSelectionMenu, Action openStatisticsMenu, Action openSettingsPage, Action openStorePage)
        {
            startGame.onClick.AddListener(()=>Enable(false));
            startGame.onClick.AddListener(StageLoadManager.LoadLatestUnlockedStage);
            
            openStageSelectionMenuButton.onClick.AddListener(()=>openStageSelectionMenu());
            
            openStatisticsMenuButton.onClick.AddListener(()=>openStatisticsMenu());
            
            openSettingsMenuButton.onClick.AddListener(()=>openSettingsPage());

            openStoreMenuButton.onClick.AddListener(()=>openStorePage());
        }

        public void Enable(bool state = true)
        {
            display.enabled = state;
        }

        private void SetStartGameText()
        {
            var content = "START";
            if (GameStatistics.ReturnMostRecentUnlockedLevel() >= 1) content = "CONTINUE";
            startGame.GetComponent<TMP_Text>().text = content;
        }
    }
}