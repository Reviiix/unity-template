using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Theming;
using UnityTemplate.UserInterface.Kit;

namespace UnityTemplate.UserInterface.MainMenus
{
    /// <summary>The settings menu: the dark mode switch, the ACCESSIBILITY button, which opens the accessibility options, and the RESET ALL button, which deletes the save after a confirmation.</summary>
    public class SettingsMenu : MenuBase
    {
        [SerializeField] private ToggleSwitch darkModeSwitch;
        [SerializeField] private Button accessibilityButton;
        [SerializeField] private Button deleteAllDataButton;
        private UserInterfaceManager userInterface;
        private ThemeService theme;

        /// <summary>Wires the dark mode switch to the theme and the ACCESSIBILITY and RESET ALL buttons.</summary>
        public void Initialise(UserInterfaceManager userInterfaceManager)
        {
            userInterface = userInterfaceManager;
            theme = ServiceLocator.Get<ThemeService>();
            ShowThemeMode();
            theme.Changed += ShowThemeMode;
            darkModeSwitch.Toggle.onValueChanged.AddListener(isOn => theme.SetMode(isOn ? ThemeMode.Dark : ThemeMode.Light));
            accessibilityButton.onClick.AddListener(AccessibilityPressed);
            deleteAllDataButton.onClick.AddListener(DeleteAllDataPressed);
        }

        /// <summary>Stops following theme changes when the menu is destroyed.</summary>
        private void OnDestroy()
        {
            if (theme != null) theme.Changed -= ShowThemeMode;
        }

        /// <summary>Sets the switch to the current theme, e.g. after RESET ALL returns to light.</summary>
        private void ShowThemeMode()
        {
            darkModeSwitch.SetIsOnWithoutNotify(theme.Mode == ThemeMode.Dark);
        }

        /// <summary>Opens the accessibility options.</summary>
        private async void AccessibilityPressed()
        {
            await userInterface.OpenPopUpAsync(userInterface.AccessibilityPopUp);
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
