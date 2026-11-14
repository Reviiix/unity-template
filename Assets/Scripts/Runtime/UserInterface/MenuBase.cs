using System;
using UnityEngine;

namespace UnityTemplate.UserInterface
{
    /// <summary>Base for the full-screen menus owned by UserInterfaceManager; each menu is shown or hidden by toggling its canvas.</summary>
    [Serializable]
    public abstract class MenuBase
    {
        [SerializeField] protected Canvas display;

        public bool IsVisible => display.enabled;

        /// <summary>Shows the menu.</summary>
        public virtual void Show()
        {
            display.enabled = true;
        }

        /// <summary>Hides the menu.</summary>
        public void Hide()
        {
            display.enabled = false;
        }
    }
}
