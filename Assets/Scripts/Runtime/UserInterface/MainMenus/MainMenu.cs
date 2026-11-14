using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Progression;

namespace UnityTemplate.UserInterface.MainMenus
{
    /// <summary>The main menu: START/CONTINUE plus the buttons that open the other menus.</summary>
    [Serializable]
    public class MainMenu : MenuBase
    {
        private const string StartText = "START";
        private const string ContinueText = "CONTINUE";
        [SerializeField] private Button startGame;
        [SerializeField] private Button openStageSelectionMenuButton;
        [SerializeField] private Button openStatisticsMenuButton;
        [SerializeField] private Button openSettingsMenuButton;
        [SerializeField] private Button openStoreMenuButton;
        private TMP_Text startGameText;
        private ProgressService progress;

        /// <summary>Wires the buttons to the callbacks that open each menu.</summary>
        public void Initialise(Action openStageSelectionMenu, Action openStatisticsMenu, Action openSettingsMenu, Action openStoreMenu)
        {
            progress = ServiceLocator.Get<ProgressService>();
            startGameText = startGame.GetComponent<TMP_Text>();
            startGame.onClick.AddListener(StartGamePressed);
            openStageSelectionMenuButton.onClick.AddListener(() => openStageSelectionMenu());
            openStatisticsMenuButton.onClick.AddListener(() => openStatisticsMenu());
            openSettingsMenuButton.onClick.AddListener(() => openSettingsMenu());
            openStoreMenuButton.onClick.AddListener(() => openStoreMenu());
        }

        /// <summary>Shows the menu with START or CONTINUE depending on whether the player has made progress.</summary>
        public override void Show()
        {
            startGameText.text = progress.HasProgress ? ContinueText : StartText;
            base.Show();
        }

        /// <summary>Loads the stage CONTINUE points at (the first stage for a new player).</summary>
        private void StartGamePressed()
        {
            var (group, stage) = progress.GetContinueStage();
            ServiceLocator.Get<StageLoader>().LoadStage(group, stage);
        }
    }
}
