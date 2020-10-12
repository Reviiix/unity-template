using System;
using Audio;
using PureFunctions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    [Serializable]
    public class IntroductionMenu : Menu, IMenu
    {
        [SerializeField]
        private Button startButton;
        private TMP_Text _startButtonText;
        private static Color[] _playButtonColorSwaps;

        public void Initialise(Action disableAllCanvases, Color[] playButtonColorSwaps)
        {
            ResolveDependencies(playButtonColorSwaps, disableAllCanvases);
        
            AddButtonEvents();
        }
    
        private void ResolveDependencies(Color[] playButtonColorSwaps, Action disableAllCanvases)
        {
            _startButtonText = startButton.GetComponent<TMP_Text>();
            _playButtonColorSwaps = playButtonColorSwaps;
            DisableAllCanvases = disableAllCanvases;
        }

        private void AddButtonEvents()
        {
            startButton.onClick.AddListener(() => Enable(false));
            startButton.onClick.AddListener(BaseAudioManager.PlayButtonClickSound);
            startButton.onClick.AddListener(ChangeTextColour.StopChangeTextColorSequence);
        }
        
        public void Enable(bool state = true)
        {
            ProjectManager.Instance.userInterface.EnablePauseButton();
            ChangeTextColour.Change(_startButtonText, _playButtonColorSwaps[0], _playButtonColorSwaps[1], ProjectManager.Instance);
            
            if (state)
            {
                BaseEnable(true);
                UserInterfaceManager.AppearAnimation(popUpMenu,() =>
                {
                    BaseEnable();
                });
            }
            else
            {
                UserInterfaceManager.DisappearAnimation(popUpMenu, () =>
                {
                    BaseEnable(false);
                    ProjectManager.Instance.LoadMainGame();
                });
            }
        }

        private void BaseEnable(bool state = true)
        {
            DisableAllCanvases();
            display.enabled = state;
        }
        
    }
}