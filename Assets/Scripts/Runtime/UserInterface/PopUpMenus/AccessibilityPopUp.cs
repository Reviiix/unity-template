using UnityEngine;
using UnityTemplate.Accessibility;
using UnityTemplate.Core;
using UnityTemplate.UserInterface.Kit;

namespace UnityTemplate.UserInterface.PopUpMenus
{
    /// <summary>The accessibility options opened from Settings: text size, bold text, high contrast and reduced motion, each applied and saved at once.</summary>
    public class AccessibilityPopUp : PopUpWindow
    {
        [SerializeField] private SegmentedControl textSize;
        [SerializeField] private ToggleSwitch boldText;
        [SerializeField] private ToggleSwitch highContrast;
        [SerializeField] private ToggleSwitch reduceMotion;
        private AccessibilityService accessibility;

        /// <summary>Wires every control to the accessibility service.</summary>
        protected override void Awake()
        {
            base.Awake();
            accessibility = ServiceLocator.Get<AccessibilityService>();
            textSize.SelectionChanged += index => accessibility.SetTextSize((TextSize)index);
            boldText.Toggle.onValueChanged.AddListener(accessibility.SetBoldText);
            highContrast.Toggle.onValueChanged.AddListener(accessibility.SetHighContrast);
            reduceMotion.Toggle.onValueChanged.AddListener(accessibility.SetReducedMotion);
        }

        /// <summary>Shows the current settings each time the popup opens, e.g. after RESET ALL.</summary>
        protected override void OnOpening()
        {
            textSize.SetSelectedWithoutNotify((int)accessibility.TextSize);
            boldText.SetIsOnWithoutNotify(accessibility.IsBoldText);
            highContrast.SetIsOnWithoutNotify(accessibility.IsHighContrast);
            reduceMotion.SetIsOnWithoutNotify(accessibility.IsReducedMotion);
        }

        /// <summary>Keeps the popup in the scene for reuse instead of releasing it.</summary>
        protected override void OnClosed()
        {
        }
    }
}
