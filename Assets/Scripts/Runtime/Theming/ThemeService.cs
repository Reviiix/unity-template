using System;
using UnityEngine;
using UnityTemplate.Core;

namespace UnityTemplate.Theming
{
    /// <summary>The player's light or dark theme: resolves ThemeColours from the current palette, saves the choice and announces changes.</summary>
    public class ThemeService : ISaveable
    {
        private readonly ThemePalette lightPalette;
        private readonly ThemePalette darkPalette;
        private readonly SaveService saveService;

        public ThemeService(ThemePalette lightPalette, ThemePalette darkPalette, SaveService saveService)
        {
            this.lightPalette = lightPalette != null ? lightPalette : throw new ArgumentNullException(nameof(lightPalette));
            this.darkPalette = darkPalette != null ? darkPalette : throw new ArgumentNullException(nameof(darkPalette));
            this.saveService = saveService;
        }

        public ThemeMode Mode { get; private set; }

        public event Action Changed;

        /// <summary>Returns the current theme's colour for a ThemeColour.</summary>
        public Color Get(ThemeColour colour)
        {
            return (Mode == ThemeMode.Dark ? darkPalette : lightPalette).Get(colour);
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

        /// <summary>Writes the current theme.</summary>
        public void Save(SaveData data)
        {
            data.darkMode = Mode == ThemeMode.Dark;
        }
    }
}
