using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ConditionalMenus
{
    /// <summary>
    /// This is the first open display class. It will be loaded once ever per user so only load through addressables.
    /// </summary>
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