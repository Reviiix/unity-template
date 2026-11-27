using NUnit.Framework;
using UnityTemplate.Accessibility;

namespace UnityTemplate.Tests
{
    /// <summary>Checks that labels are cleaned for the screen reader: markup and decorative symbols removed, capitals read as words, numbers kept.</summary>
    public class ScreenReaderLabelsTests
    {
        [TestCase("‹  BACK", "Back")]
        [TestCase("NEXT  ›", "Next")]
        [TestCase("×", "")]
        [TestCase("<size=87%><alpha=#A6>CREDITS<alpha=#FF></size>   1000", "Credits 1000")]
        [TestCase("RESET ALL", "Reset All")]
        [TestCase("130%", "130%")]
        [TestCase("v0.1", "v0.1")]
        [TestCase("This deletes all of your progress.", "This deletes all of your progress.")]
        [TestCase(null, "")]
        public void LabelsAreCleanedForSpeech(string raw, string expected)
        {
            Assert.AreEqual(expected, ScreenReaderLabels.Clean(raw));
        }
    }
}
