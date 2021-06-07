using System;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.PopUpMenus
{
    [Serializable]
    public class ConfirmationPopUp : PopUpInterface, IUserInterface
    {
        private bool _choice;
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
            confirm.onClick.AddListener(()=>_choice = true);
            deny.onClick.AddListener(()=>_choice = false);
            confirm.onClick.AddListener(()=>Enable(false));
            deny.onClick.AddListener(()=>Enable(false));
        }

        public void Enable(bool state = true)
        {
            Display.enabled = state;
            switch (state)
            {
                case true:
                    AppearAnimation(popUpMenu);
                    break;
                case false:
                    DisappearAnimation(popUpMenu, ChoiceMade);
                    break;
            }
        }

        private void ChoiceMade()
        {
            OnChoiceMade?.Invoke(_choice);
        }
    }
}
