using UnityEngine;

namespace UnityTemplate.UserInterface
{
    /// <summary>Base for the full-screen menus UserInterfaceManager switches between; each sits on the canvas at the root of its menu prefab and is shown or hidden by toggling that canvas.</summary>
    [RequireComponent(typeof(Canvas))]
    [DisallowMultipleComponent]
    public abstract class MenuBase : MonoBehaviour
    {
        private Canvas display;

        public bool IsVisible => Display.enabled;

        private Canvas Display
        {
            get
            {
                if (display == null) display = GetComponent<Canvas>();
                return display;
            }
        }

        /// <summary>Shows the menu.</summary>
        public virtual void Show()
        {
            Display.enabled = true;
        }

        /// <summary>Hides the menu.</summary>
        public void Hide()
        {
            Display.enabled = false;
        }
    }
}
