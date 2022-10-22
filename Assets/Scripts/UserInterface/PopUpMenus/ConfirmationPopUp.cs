using System;
using Audio;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.PopUpMenus
{
    /// <summary>
    /// This is the confirmation screen, It will return a choice of accept or deny
    /// </summary>
    [Serializable]
    public class ConfirmationPopUp : PopUpInterface, IUserInterface
    {
        private bool choice;
        public Action<bool> OnChoiceMade;
        [SerializeField] private Button confirm;
        [SerializeField] private Button deny;
        
        protected override void Awake()
        {
            SetButtonEvents();
            base.Awake();
        }
        
        private void SetButtonEvents()
        {
            confirm.onClick.AddListener(()=>choice = true);
            deny.onClick.AddListener(()=>choice = false);
            confirm.onClick.AddListener(()=>Enable(false));
            deny.onClick.AddListener(()=>Enable(false));
        }

        public override void Enable(bool state = true)
        {
            switch (state)
            {
                case true:
                    Display.enabled = true;
                    AppearAnimation(popUpMenu);
                    break;
                case false:
                    DisappearAnimation(popUpMenu, ()=>
                    {
                        OnChoiceMade?.Invoke(choice);
                        Display.enabled = false;
                    });
                    break;
            }
            BaseAudioManager.PlayMenuMovementSound();
        }
    }
}
