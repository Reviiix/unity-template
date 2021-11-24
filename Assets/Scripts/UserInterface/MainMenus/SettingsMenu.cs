using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserInterface.PopUpMenus;

namespace UserInterface.MainMenus
{
    [Serializable]
    public class SettingsMenu : UserInterface, IUserInterface
    {
        public static Action<string> OnNameChange;
        [SerializeField] private Button deleteAllDataButton;
        [SerializeField] private TMP_InputField nameInputField;
        private static ConfirmationPopUp ConfirmationPopUp => UserInterfaceManager.ConfirmationScreen;
        
        public void Initialise()
        {
            AssignEvents();
        }

        private void AssignEvents()
        {
            //nameInputField.onDeselect.AddListener(ChangeNamePressed);
            deleteAllDataButton.onClick.AddListener(DeleteAllDataPressed);
        }
        
        #region ChangeName
        private void ChangeNamePressed(string name)
        {
            ConfirmationPopUp.OnChoiceMade += OnChangeNameConfirmation;
            ConfirmationPopUp.Enable();
        }

        private void OnChangeNameConfirmation(bool confirmation)
        {
            ConfirmationPopUp.OnChoiceMade -= OnChangeNameConfirmation;

            if (confirmation) ChangeName();
        }
        
        private void ChangeName()
        {
            OnNameChange?.Invoke(nameInputField.text);
        }
        #endregion ChangeName

        #region Delete All Save Data
        private void DeleteAllDataPressed()
        {
            ConfirmationPopUp.OnChoiceMade += OnDeleteAllDataConfirmation;
            ConfirmationPopUp.Enable();
        }

        private void OnDeleteAllDataConfirmation(bool confirmation)
        {
            ConfirmationPopUp.OnChoiceMade -= OnDeleteAllDataConfirmation;

            if (confirmation) DeleteAllData();
        }
        
        private void DeleteAllData()
        {
            SaveSystem.Delete();
        }
        #endregion Delete All Save Data

        public void Enable(bool state = true)
        {
            display.enabled = state;
        }
    }
}
