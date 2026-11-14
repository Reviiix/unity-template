using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityTemplate.UserInterface.ConditionalMenus
{
    /// <summary>The welcome popup shown on the very first launch.</summary>
    public class FirstOpenPopUpMenu : PopUpWindow
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private Button continueButton;

        /// <summary>Wires CONTINUE and writes the welcome title.</summary>
        protected override void Awake()
        {
            base.Awake();
            continueButton.onClick.AddListener(() => CloseWithSlide());
            title.text = "WELCOME TO " + Application.productName.ToUpperInvariant();
        }
    }
}
