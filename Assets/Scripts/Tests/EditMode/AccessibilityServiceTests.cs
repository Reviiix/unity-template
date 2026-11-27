using NUnit.Framework;
using UnityTemplate.Accessibility;
using UnityTemplate.Core;

namespace UnityTemplate.Tests
{
    /// <summary>Checks that a new save starts from the device's accessibility settings, that the player's choices replace them and are saved, and that reduced motion removes animation time.</summary>
    public class AccessibilityServiceTests
    {
        private static readonly SystemAccessibility DefaultDevice = new(1f, false, false, false);

        [TestCase(1f, TextSize.Standard)]
        [TestCase(1.15f, TextSize.Large)]
        [TestCase(1.5f, TextSize.Largest)]
        public void ANewSaveStartsFromTheDeviceTextSize(float fontScale, TextSize expected)
        {
            var accessibility = new AccessibilityService(null, new SystemAccessibility(fontScale, false, false, false));
            accessibility.Load(new SaveData());

            Assert.AreEqual(expected, accessibility.TextSize);
        }

        [Test]
        public void ANewSaveStartsFromTheDeviceBoldContrastAndMotion()
        {
            var accessibility = new AccessibilityService(null, new SystemAccessibility(1f, true, true, true));
            accessibility.Load(new SaveData());

            Assert.IsTrue(accessibility.IsBoldText);
            Assert.IsTrue(accessibility.IsHighContrast);
            Assert.IsTrue(accessibility.IsReducedMotion);
        }

        [Test]
        public void SavedChoicesReplaceTheDeviceSettings()
        {
            var accessibility = new AccessibilityService(null, DefaultDevice);
            accessibility.SetTextSize(TextSize.Largest);
            accessibility.SetHighContrast(true);
            var data = new SaveData();
            accessibility.Save(data);

            var reloaded = new AccessibilityService(null, new SystemAccessibility(1f, true, false, true));
            reloaded.Load(data);

            Assert.AreEqual(TextSize.Largest, reloaded.TextSize);
            Assert.AreEqual(AccessibilityService.LargestTextScale, reloaded.TextScale);
            Assert.IsFalse(reloaded.IsBoldText, "The saved choice, not the device, decides bold text.");
            Assert.IsTrue(reloaded.IsHighContrast);
            Assert.IsFalse(reloaded.IsReducedMotion);
        }

        [Test]
        public void ChangedIsRaisedOnlyWhenASettingChanges()
        {
            var accessibility = new AccessibilityService(null, DefaultDevice);
            var raised = 0;
            accessibility.Changed += () => raised++;

            accessibility.SetBoldText(false);
            accessibility.SetBoldText(true);
            accessibility.SetBoldText(true);
            accessibility.SetTextSize(TextSize.Standard);

            Assert.AreEqual(1, raised);
        }

        [Test]
        public void ReducedMotionRemovesAnimationTime()
        {
            var accessibility = new AccessibilityService(null, DefaultDevice);
            Assert.AreEqual(0.87f, accessibility.MotionSeconds(0.87f));

            accessibility.SetReducedMotion(true);

            Assert.AreEqual(0, accessibility.MotionSeconds(0.87f));
        }
    }
}
