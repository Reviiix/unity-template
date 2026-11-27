using System;
using UnityEngine;
using UnityTemplate.Accessibility;
using UnityTemplate.Core;

namespace UnityTemplate.Theming
{
    /// <summary>The player's light or dark theme: resolves ThemeColours from the current palette (the high-contrast one when that accessibility setting is on), saves the choice and announces changes.</summary>
    public class ThemeService : ISaveable
    {
        private readonly ThemePalette lightPalette;
        private readonly ThemePalette darkPalette;
        private readonly ThemePalette highContrastLightPalette;
        private readonly ThemePalette highContrastDarkPalette;
        private readonly SaveService saveService;
        private readonly AccessibilityService accessibility;
        private bool isHighContrast;

        /// <summary>Creates the service with the light and dark palettes only; high contrast is unavailable.</summary>
        public ThemeService(ThemePalette lightPalette, ThemePalette darkPalette, SaveService saveService)
            : this(lightPalette, darkPalette, null, null, saveService, null)
        {
        }

        /// <summary>Creates the service and follows the accessibility service's high-contrast setting; a missing high-contrast palette falls back to the normal one.</summary>
        public ThemeService(ThemePalette lightPalette, ThemePalette darkPalette, ThemePalette highContrastLightPalette, ThemePalette highContrastDarkPalette, SaveService saveService, AccessibilityService accessibility)
        {
            this.lightPalette = lightPalette != null ? lightPalette : throw new ArgumentNullException(nameof(lightPalette));
            this.darkPalette = darkPalette != null ? darkPalette : throw new ArgumentNullException(nameof(darkPalette));
            this.highContrastLightPalette = highContrastLightPalette != null ? highContrastLightPalette : lightPalette;
            this.highContrastDarkPalette = highContrastDarkPalette != null ? highContrastDarkPalette : darkPalette;
            this.saveService = saveService;
            this.accessibility = accessibility;
            if (accessibility == null) return;
            isHighContrast = accessibility.IsHighContrast;
            accessibility.Changed += FollowContrast;
        }

        public ThemeMode Mode { get; private set; }

        public event Action Changed;

        /// <summary>Returns the current theme's colour for a ThemeColour.</summary>
        public Color Get(ThemeColour colour)
        {
            var palette = Mode == ThemeMode.Dark
                ? isHighContrast ? highContrastDarkPalette : darkPalette
                : isHighContrast ? highContrastLightPalette : lightPalette;
            return palette.Get(colour);
        }

        /// <summary>Switches theme, recolours every listener and saves the choice; ignored when the theme is already current.</summary>
        public void SetMode(ThemeMode mode)
        {
            if (mode == Mode) return;
            Mode = mode;
            Changed?.Invoke();
            saveService?.Save();
        }

        /// <summary>Restores the saved theme (light for a new game) and recolours every listener.</summary>
        public void Load(SaveData data)
        {
            Mode = data.darkMode ? ThemeMode.Dark : ThemeMode.Light;
            Changed?.Invoke();
        }

        /// <summary>Recolours every listener when the high-contrast setting changes.</summary>
        private void FollowContrast()
        {
            if (accessibility.IsHighContrast == isHighContrast) return;
            isHighContrast = accessibility.IsHighContrast;
            Changed?.Invoke();
        }

        /// <summary>Writes the current theme.</summary>
        public void Save(SaveData data)
        {
            data.darkMode = Mode == ThemeMode.Dark;
        }
    }
}
