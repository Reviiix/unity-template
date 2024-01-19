using System;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.PopUpMenus
{
    /// <summary>
    /// This menu will display when the game is paused
    /// </summary>
    [Serializable]
    public class PauseUserMenu : UserInterface, IUserInterface
    {
        [SerializeField] private Sprite pauseSprite;
        [SerializeField] private Sprite playSprite;
        [SerializeField] private Image pauseButtonImage; 
        private Button _pauseButton;

        public void Initialise()
        {
            ResolveDependencies();
            AddButtonEvents();
        }
    
        private void ResolveDependencies()
        {
            _pauseButton = pauseButtonImage.GetComponent<Button>();
        }

        private void AddButtonEvents()
        {
            _pauseButton.onClick.AddListener(PauseManager.PauseButtonPressed);
            _pauseButton.onClick.AddListener(ButtonPressed);
        }

        private void EnablePauseButtons(bool state = true)
        {
            pauseButtonImage.enabled = state;
            _pauseButton.enabled = state;
        }
        
        private void ButtonPressed()
        {
            Enable(PauseManager.IsPaused);
        }

        public void Enable(bool state = true)
        {
            display.enabled = state;
            pauseButtonImage.sprite = GetCurrentPauseButtonSprite(state);
        }

        private Sprite GetCurrentPauseButtonSprite(bool state = true)
        {
            return state ? playSprite : pauseSprite;
        }
    }
}