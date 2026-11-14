using NUnit.Framework;
using UnityTemplate.Progression;

namespace UnityTemplate.Tests
{
    /// <summary>Tests the precomputed level table, levelling up and the maximum level.</summary>
    public class ExperienceServiceTests
    {
        private TestSaveFolder folder;

        [SetUp]
        public void CreateFolder()
        {
            folder = new TestSaveFolder();
        }

        [TearDown]
        public void DeleteFolder()
        {
            folder.Dispose();
        }

        [Test]
        public void LevelTableMatchesTheOriginalCurve()
        {
            var experience = new ExperienceService(folder.CreateSaveService(), 50);

            Assert.AreEqual(100, experience.GetExperienceForLevel(0));
            Assert.AreEqual(155, experience.GetExperienceForLevel(1));
            Assert.AreEqual(240, experience.GetExperienceForLevel(2));
            Assert.AreEqual(255, experience.GetCumulativeExperience(1));
        }

        [Test]
        public void GainingEnoughExperienceRaisesTheLevel()
        {
            var experience = new ExperienceService(folder.CreateSaveService(), 50);
            var levelChanges = 0;
            experience.LevelChanged += _ => levelChanges++;

            experience.Add(99);
            Assert.AreEqual(0, experience.CurrentLevel);
            experience.Add(1);
            Assert.AreEqual(1, experience.CurrentLevel);
            Assert.AreEqual(1, levelChanges);
        }

        [Test]
        public void ExperienceBeyondTheLastLevelStaysAtTheMaximumLevel()
        {
            var experience = new ExperienceService(folder.CreateSaveService(), 3);

            experience.Add(long.MaxValue / 2048);

            Assert.AreEqual(2, experience.CurrentLevel);
            Assert.AreEqual(experience.MaximumLevel, experience.CurrentLevel);
        }

        [Test]
        public void TwoHundredLevelsDoNotOverflow()
        {
            var experience = new ExperienceService(folder.CreateSaveService(), 200);

            for (var level = 1; level <= experience.MaximumLevel; level++)
            {
                Assert.GreaterOrEqual(experience.GetCumulativeExperience(level), experience.GetCumulativeExperience(level - 1));
            }
        }
    }
}
