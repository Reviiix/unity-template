using System;
using Audio;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    [Serializable]
    public class SettingsMenu : Menu, IMenu
    {
        private Button _settingsButton;
        [SerializeField]
        private GameObject settingsButtonObject;
        [SerializeField]
        private Button closeButton;

        public void Initialise(Action enablePauseButton, Action disablePauseButton, Action enablePauseMenu)
        {
            ResolveDependencies();
        
            AddButtonEvents(enablePauseButton, disablePauseButton);
        }

        private void ResolveDependencies()
        {
            _settingsButton = settingsButtonObject.GetComponent<Button>();
        }

        private void AddButtonEvents(Action enablePauseButton, Action disablePauseButton)
        {
            closeButton.onClick.AddListener(() => Enable(false));
            closeButton.onClick.AddListener(() => enablePauseButton());
            closeButton.onClick.AddListener(BaseAudioManager.PlayButtonClickSound);
        
            _settingsButton.onClick.AddListener(() => Enable());
            _settingsButton.onClick.AddListener(() => disablePauseButton());
            _settingsButton.onClick.AddListener(BaseAudioManager.PlayButtonClickSound);
        }

        public void BaseEnable(bool state = true)
        {
            display.enabled = state;
            settingsButtonObject.SetActive(!state);
        }
        
        public void Enable(bool state = true)
        {
            if (state)
            {
                BaseEnable();
                UserInterfaceManager.AppearAnimation(popUpMenu, () =>
                {
                    BaseEnable();
                });
            }
            else
            {
                UserInterfaceManager.DisappearAnimation(popUpMenu, () =>
                {
                    BaseEnable(false);
                });
            }
        }
    }
}
