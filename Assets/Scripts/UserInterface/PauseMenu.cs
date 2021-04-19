using System;
using Audio;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    [Serializable]
    public class PauseUserInterface : UserInterface, IUserInterface
    {
        [SerializeField] 
        private Sprite pauseSprite;
        [SerializeField] 
        private Sprite playSprite;
        [SerializeField] 
        private Image pauseButtonImage;
        private Button _pauseButton;

        public void Initialise(Action enablePauseCanvas)
        {
            ResolveDependencies();
            AddButtonEvents(enablePauseCanvas);
        }
    
        private void ResolveDependencies()
        {
            _pauseButton = pauseButtonImage.GetComponent<Button>();
        }

        private void AddButtonEvents(Action enablePauseCanvas)
        {
            _pauseButton.onClick.AddListener(PauseManager.PauseButtonPressed);
            _pauseButton.onClick.AddListener(()=>enablePauseCanvas());
            _pauseButton.onClick.AddListener(BaseAudioManager.PlayButtonClickSound);
        }

        public void EnablePauseButtons(bool state = true)
        {
            pauseButtonImage.enabled = state;
            _pauseButton.enabled = state;
        }

        public void Enable(bool state = true)
        {
            DisableAllCanvases();
        
            display.enabled = state;
            pauseButtonImage.sprite = ReturnCurrentPauseButtonSprite(state);
        }

        public Sprite ReturnCurrentPauseButtonSprite(bool state = true)
        {
            return state ? playSprite : pauseSprite;
        }
    }
}