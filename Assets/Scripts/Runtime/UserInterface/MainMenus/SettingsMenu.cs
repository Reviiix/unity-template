using System;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.UserInterface.PopUpMenus;

namespace UnityTemplate.UserInterface.MainMenus
{
    /// <summary>The settings menu: currently the RESET ALL button, which deletes the save after a confirmation.</summary>
    [Serializable]
    public class SettingsMenu : MenuBase
    {
        [SerializeField] private Button deleteAllDataButton;
        private UserInterfaceManager userInterface;

        /// <summary>Wires the RESET ALL button.</summary>
        public void Initialise(UserInterfaceManager userInterfaceManager)
        {
            userInterface = userInterfaceManager;
            deleteAllDataButton.onClick.AddListener(DeleteAllDataPressed);
        }

        /// <summary>Asks for confirmation and, on YES, deletes the save and resets the game.</summary>
        private async void DeleteAllDataPressed()
        {
            var confirmation = userInterface.ConfirmationScreen;
            await userInterface.OpenPopUpAsync(confirmation);
            if (confirmation.Confirmed) ServiceLocator.Get<SaveService>().DeleteAll();
        }
    }
}
