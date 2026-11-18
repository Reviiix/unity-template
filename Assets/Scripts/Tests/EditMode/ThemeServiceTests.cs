using System;
using NUnit.Framework;
using UnityEditor;
using UnityTemplate.Core;
using UnityTemplate.Theming;

namespace UnityTemplate.Tests
{
    /// <summary>Checks ThemeService against the real light and dark palettes: switching, change events and saving.</summary>
    public class ThemeServiceTests
    {
        private ThemePalette light;
        private ThemePalette dark;
        private ThemeService theme;

        [SetUp]
        public void CreateService()
        {
            light = AssetDatabase.LoadAssetAtPath<ThemePalette>("Assets/Settings/Themes/LightTheme.asset");
            dark = AssetDatabase.LoadAssetAtPath<ThemePalette>("Assets/Settings/Themes/DarkTheme.asset");
            theme = new ThemeService(light, dark, null);
        }

        [Test]
        public void StartsLightAndSwitchesToTheDarkPalette()
        {
            Assert.AreEqual(ThemeMode.Light, theme.Mode);
            Assert.AreEqual(light.Get(ThemeColour.Background), theme.Get(ThemeColour.Background));

            theme.SetMode(ThemeMode.Dark);

            Assert.AreEqual(ThemeMode.Dark, theme.Mode);
            Assert.AreEqual(dark.Get(ThemeColour.Background), theme.Get(ThemeColour.Background));
        }

        [Test]
        public void ChangedIsRaisedOnlyWhenTheThemeChanges()
        {
            var raised = 0;
            theme.Changed += () => raised++;

            theme.SetMode(ThemeMode.Light);
            theme.SetMode(ThemeMode.Dark);
            theme.SetMode(ThemeMode.Dark);

            Assert.AreEqual(1, raised);
        }

        [Test]
        public void TheThemeIsSavedAndLoaded()
        {
            theme.SetMode(ThemeMode.Dark);
            var data = new SaveData();
            theme.Save(data);
            Assert.IsTrue(data.darkMode);

            var reloaded = new ThemeService(light, dark, null);
            reloaded.Load(data);
            Assert.AreEqual(ThemeMode.Dark, reloaded.Mode);

            reloaded.Load(new SaveData());
            Assert.AreEqual(ThemeMode.Light, reloaded.Mode, "A new game (or RESET ALL) starts light.");
        }

        [Test]
        public void BothPalettesDefineEveryColourAndDiffer()
        {
            foreach (ThemeColour colour in Enum.GetValues(typeof(ThemeColour)))
            {
                Assert.DoesNotThrow(() => light.Get(colour), $"The light palette has no {colour}.");
                Assert.DoesNotThrow(() => dark.Get(colour), $"The dark palette has no {colour}.");
            }
            Assert.AreNotEqual(light.Get(ThemeColour.Surface), dark.Get(ThemeColour.Surface));
            Assert.AreNotEqual(light.Get(ThemeColour.TextPrimary), dark.Get(ThemeColour.TextPrimary));
        }
    }
}
