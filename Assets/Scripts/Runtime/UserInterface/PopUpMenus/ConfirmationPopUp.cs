using UnityEngine;
using UnityEngine.UI;

namespace UnityTemplate.UserInterface.PopUpMenus
{
    /// <summary>A reusable YES/NO popup in the scene; open it with UserInterfaceManager.OpenPopUpAsync and read Confirmed afterwards.</summary>
    public class ConfirmationPopUp : PopUpWindow
    {
        [SerializeField] private Button confirm;
        [SerializeField] private Button deny;

        public bool Confirmed { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            confirm.onClick.AddListener(() => Choose(true));
            deny.onClick.AddListener(() => Choose(false));
        }

        /// <summary>Resets the answer each time the popup opens.</summary>
        protected override void OnOpening()
        {
            Confirmed = false;
        }

        /// <summary>The back button answers NO.</summary>
        protected override void OnBackPressed()
        {
            Choose(false);
        }

        /// <summary>Keeps the popup in the scene for reuse instead of releasing it.</summary>
        protected override void OnClosed()
        {
        }

        private void Choose(bool confirmed)
        {
            if (State is PopUpState.Hidden or PopUpState.Closing) return;

            Confirmed = confirmed;
            CloseWithSlide(playSound: true);
        }
    }
}
