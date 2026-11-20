using NUnit.Framework;
using UnityEngine;
using UnityTemplate.Progression;

namespace UnityTemplate.Tests
{
    /// <summary>Tests stage unlocking (including the catalogue's unlock-all switch), stage names, the CONTINUE target and the high score.</summary>
    public class ProgressServiceTests
    {
        private TestSaveFolder folder;
        private StageCatalogue catalogue;
        private ProgressService progress;

        [SetUp]
        public void CreateService()
        {
            folder = new TestSaveFolder();
            catalogue = ScriptableObject.CreateInstance<StageCatalogue>();
            JsonUtility.FromJsonOverwrite("{\"groups\":[{\"scenes\":[{},{},{}]},{\"scenes\":[{},{}]}]}", catalogue);
            var saveService = folder.CreateSaveService();
            progress = new ProgressService(saveService, catalogue);
            saveService.Register(progress);
            saveService.Load();
        }

        [TearDown]
        public void Clean()
        {
            Object.DestroyImmediate(catalogue);
            folder.Dispose();
        }

        [Test]
        public void ANewGameHasOnlyTheFirstStageUnlocked()
        {
            Assert.IsFalse(progress.HasProgress);
            Assert.IsTrue(progress.IsUnlocked(0, 0));
            Assert.IsFalse(progress.IsUnlocked(0, 1));
            Assert.AreEqual((0, 0), progress.GetContinueStage());
        }

        [Test]
        public void RatingAStageUnlocksTheNextOneAcrossGroups()
        {
            progress.RecordRating(0, 0, 3);
            progress.RecordRating(0, 1, 1);
            progress.RecordRating(0, 2, 2);

            Assert.IsTrue(progress.HasProgress);
            Assert.IsTrue(progress.IsUnlocked(1, 0));
            Assert.AreEqual((1, 0), progress.GetContinueStage());
        }

        [Test]
        public void OnlyABetterRatingIsKept()
        {
            progress.RecordRating(0, 0, 3);
            progress.RecordRating(0, 0, 1);

            Assert.AreEqual(3, progress.GetRating(0, 0));
        }

        [Test]
        public void OnlyAHigherScoreBecomesTheHighScore()
        {
            Assert.IsTrue(progress.SubmitScore(10));
            Assert.IsFalse(progress.SubmitScore(5));
            Assert.AreEqual(10, progress.HighScore);
        }

        [Test]
        public void AllStagesUnlockedUnlocksEveryStageWithoutRatings()
        {
            JsonUtility.FromJsonOverwrite("{\"allStagesUnlocked\":true}", catalogue);
            Assert.IsTrue(progress.IsUnlocked(0, 2));
            Assert.IsTrue(progress.IsUnlocked(1, 1));
        }

        [Test]
        public void StageNamesAreOptionalPerSlot()
        {
            JsonUtility.FromJsonOverwrite("{\"groups\":[{\"scenes\":[{},{},{}],\"names\":[\"Waves\",\" \"]},{\"scenes\":[{},{}]}]}", catalogue);
            Assert.AreEqual("Waves", catalogue.GetStageName(0, 0));
            Assert.IsNull(catalogue.GetStageName(0, 1), "A blank name should fall back to group-stage.");
            Assert.IsNull(catalogue.GetStageName(0, 2), "A slot past the end of the names should fall back to group-stage.");
            Assert.IsNull(catalogue.GetStageName(1, 0), "A group without names should fall back to group-stage.");
        }
    }
}
