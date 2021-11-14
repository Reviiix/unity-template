using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ConditionalMenus
{
    public class FirstOpenPopUpMenu : PopUpInterface, IUserInterface
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private Button continueButton;

        protected override void Awake()
        {
            base.Awake();
            SetButtonEvents();
            title.text = "WELCOME TO " + PlayerSettings.productName.ToUpper();
        }
        
        private void SetButtonEvents()
        {
            continueButton.onClick.AddListener(ContinuePressed);
        }

        private void ContinuePressed()
        {
            Enable(false);
        }
    }
}