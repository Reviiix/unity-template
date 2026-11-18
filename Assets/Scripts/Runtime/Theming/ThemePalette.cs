using System;
using UnityEngine;

namespace UnityTemplate.Theming
{
    /// <summary>One theme's colour for every ThemeColour; the light and dark palettes in Assets/Settings/Themes are what designers edit.</summary>
    [CreateAssetMenu(menuName = "Unity Template/Theme Palette", fileName = "ThemePalette")]
    public class ThemePalette : ScriptableObject
    {
        [Header("Surfaces")]
        [SerializeField] private Color background = Color.white;
        [SerializeField] private Color surface = Color.white;
        [Tooltip("Raised above a muted surface, such as the selected tab.")]
        [SerializeField] private Color raised = Color.white;
        [SerializeField] private Color muted = Color.white;
        [SerializeField] private Color border = Color.white;
        [Tooltip("Slider tracks and the off state of switches.")]
        [SerializeField] private Color track = Color.white;
        [Header("Text")]
        [SerializeField] private Color textPrimary = Color.black;
        [SerializeField] private Color textSecondary = Color.black;
        [SerializeField] private Color textTertiary = Color.black;
        [Header("Accent")]
        [SerializeField] private Color accent = Color.blue;
        [Tooltip("Text and icons drawn on the accent or danger colour.")]
        [SerializeField] private Color onAccent = Color.white;
        [Tooltip("Tinted rows, such as unlocked achievements.")]
        [SerializeField] private Color accentSoft = Color.white;
        [SerializeField] private Color danger = Color.red;
        [Header("Other")]
        [Tooltip("The overlay behind popups and the pause screen (alpha included).")]
        [SerializeField] private Color dim = new(0f, 0f, 0f, 0.6f);
        [SerializeField] private Color starUnearned = Color.grey;

        /// <summary>Returns this palette's colour for a ThemeColour.</summary>
        public Color Get(ThemeColour colour)
        {
            return colour switch
            {
                ThemeColour.Background => background,
                ThemeColour.Surface => surface,
                ThemeColour.Raised => raised,
                ThemeColour.Muted => muted,
                ThemeColour.Border => border,
                ThemeColour.Track => track,
                ThemeColour.TextPrimary => textPrimary,
                ThemeColour.TextSecondary => textSecondary,
                ThemeColour.TextTertiary => textTertiary,
                ThemeColour.Accent => accent,
                ThemeColour.OnAccent => onAccent,
                ThemeColour.AccentSoft => accentSoft,
                ThemeColour.Danger => danger,
                ThemeColour.Dim => dim,
                ThemeColour.StarUnearned => starUnearned,
                _ => throw new ArgumentOutOfRangeException(nameof(colour), colour, "ThemePalette has no field for this colour."),
            };
        }
    }
}
