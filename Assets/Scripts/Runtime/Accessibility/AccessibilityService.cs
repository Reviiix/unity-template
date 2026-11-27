using System;
using UnityTemplate.Core;

namespace UnityTemplate.Accessibility
{
    /// <summary>The player's accessibility settings (text size, bold text, high contrast, reduced motion): a new save starts from the device's settings, every change is saved and announced.</summary>
    public class AccessibilityService : ISaveable
    {
        public const float LargeTextScale = 1.15f;
        public const float LargestTextScale = 1.3f;
        private const float LargeSystemFontScale = 1.1f;
        private readonly SaveService saveService;
        private readonly SystemAccessibility system;

        public TextSize TextSize { get; private set; }
        public bool IsBoldText { get; private set; }
        public bool IsHighContrast { get; private set; }
        public bool IsReducedMotion { get; private set; }
        public float TextScale => TextSize switch
        {
            TextSize.Large => LargeTextScale,
            TextSize.Largest => LargestTextScale,
            _ => 1f,
        };

        public event Action Changed;

        /// <summary>Creates the service with the device's settings, which a new save starts from.</summary>
        public AccessibilityService(SaveService saveService, SystemAccessibility system)
        {
            this.saveService = saveService;
            this.system = system;
            ApplySystemSettings();
        }

        /// <summary>Sets the text size.</summary>
        public void SetTextSize(TextSize size)
        {
            if (size == TextSize) return;
            TextSize = size;
            AnnounceAndSave();
        }

        /// <summary>Turns bold text on or off.</summary>
        public void SetBoldText(bool isOn)
        {
            if (isOn == IsBoldText) return;
            IsBoldText = isOn;
            AnnounceAndSave();
        }

        /// <summary>Turns the high-contrast palettes on or off.</summary>
        public void SetHighContrast(bool isOn)
        {
            if (isOn == IsHighContrast) return;
            IsHighContrast = isOn;
            AnnounceAndSave();
        }

        /// <summary>Turns reduced motion on or off.</summary>
        public void SetReducedMotion(bool isOn)
        {
            if (isOn == IsReducedMotion) return;
            IsReducedMotion = isOn;
            AnnounceAndSave();
        }

        /// <summary>Returns an animation's length, or 0 so it jumps straight to its end when reduced motion is on.</summary>
        public float MotionSeconds(float seconds)
        {
            return IsReducedMotion ? 0 : seconds;
        }

        /// <summary>Restores the saved settings, or the device's settings for a save that has none yet, and announces them.</summary>
        public void Load(SaveData data)
        {
            if (data.accessibilitySaved)
            {
                TextSize = Enum.IsDefined(typeof(TextSize), data.textSize) ? (TextSize)data.textSize : TextSize.Standard;
                IsBoldText = data.boldText;
                IsHighContrast = data.highContrast;
                IsReducedMotion = data.reduceMotion;
            }
            else
            {
                ApplySystemSettings();
            }
            Changed?.Invoke();
        }

        /// <summary>Writes the settings and marks the save as having them.</summary>
        public void Save(SaveData data)
        {
            data.textSize = (int)TextSize;
            data.boldText = IsBoldText;
            data.highContrast = IsHighContrast;
            data.reduceMotion = IsReducedMotion;
            data.accessibilitySaved = true;
        }

        /// <summary>Takes the text size, bold text, contrast and motion settings from the device.</summary>
        private void ApplySystemSettings()
        {
            TextSize = system.FontScale >= LargestTextScale ? TextSize.Largest : system.FontScale >= LargeSystemFontScale ? TextSize.Large : TextSize.Standard;
            IsBoldText = system.IsBoldText;
            IsHighContrast = system.IsHighContrast;
            IsReducedMotion = system.IsReducedMotion;
        }

        /// <summary>Tells listeners and saves.</summary>
        private void AnnounceAndSave()
        {
            Changed?.Invoke();
            saveService?.Save();
        }
    }
}
