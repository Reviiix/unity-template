using System;
using PureFunctions.DateTime;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ConditionalMenus
{
    public class FirstOpenPopUpMenu : PopUpInterface, IUserInterface
    {
        public static Action<string, DateTime> OnPlayerInformationSubmitted;
        [SerializeField] private Button continueButton;
        [SerializeField] private InputField nameInput;
        [SerializeField] private DatePicker datePicker;

        protected override void Awake()
        {
            base.Awake();
            SetButtonEvents();
        }
        
        private void SetButtonEvents()
        {
            continueButton.onClick.AddListener(ContinuePressed);
        }

        private void ContinuePressed()
        {
            OnPlayerInformationSubmitted?.Invoke(nameInput.text, datePicker.ReturnCurrentDateSelection);
            StageLoadManager.LoadTutorial();
            Enable(false);
        }

        public void Enable(bool state = true)
        {
            Display.enabled = state;
            if (!state) DisappearAnimation(popUpMenu, UnloadSelf);
        }
    }
}