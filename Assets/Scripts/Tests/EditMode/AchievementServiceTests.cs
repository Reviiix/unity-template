using NUnit.Framework;
using UnityEngine;
using UnityTemplate.Achievements;
using UnityTemplate.Core;
using UnityTemplate.Economy;

namespace UnityTemplate.Tests
{
    /// <summary>Tests unlocking, rewards, the feature switch and saving achievements by id.</summary>
    public class AchievementServiceTests
    {
        private TestSaveFolder folder;
        private AchievementCatalogue catalogue;

        [SetUp]
        public void CreateCatalogue()
        {
            folder = new TestSaveFolder();
            catalogue = ScriptableObject.CreateInstance<AchievementCatalogue>();
            JsonUtility.FromJsonOverwrite("{\"permanentAchievements\":[{\"achievement\":1,\"description\":\"Play for one hour total.\",\"reward\":10},{\"achievement\":7,\"description\":\"Play your first game.\",\"reward\":25}]}", catalogue);
        }

        [TearDown]
        public void Clean()
        {
            Object.DestroyImmediate(catalogue);
            folder.Dispose();
        }

        [Test]
        public void UnlockingPaysTheRewardOnce()
        {
            var (achievements, credits, _) = CreateServices(true);

            achievements.Unlock(Achievement.OpenTheAppOnce);
            achievements.Unlock(Achievement.OpenTheAppOnce);

            Assert.AreEqual(25, credits.GetBalance(Currency.PremiumCredits));
            Assert.AreEqual(1, AchievementService.GetCount(achievements.PermanentAchievements, true));
        }

        [Test]
        public void NothingUnlocksWhileTheFeatureIsDisabled()
        {
            var (achievements, credits, _) = CreateServices(false);

            achievements.Unlock(Achievement.OpenTheAppOnce);

            Assert.AreEqual(0, credits.GetBalance(Currency.PremiumCredits));
            Assert.AreEqual(2, achievements.PermanentAchievements.Count);
        }

        [Test]
        public void UnlockedAchievementsAreRestoredById()
        {
            var (achievements, _, _) = CreateServices(true);
            achievements.Unlock(Achievement.PlayForOneHour);

            var (restored, credits, _) = CreateServices(true);

            Assert.IsTrue(restored.PermanentAchievements[0].IsUnlocked);
            Assert.IsFalse(restored.PermanentAchievements[1].IsUnlocked);
            Assert.AreEqual(10, credits.GetBalance(Currency.PremiumCredits));
        }

        [Test]
        public void DisabledRemoteTargetsCreateNoDynamicAchievements()
        {
            var (achievements, _, _) = CreateServices(true);

            Assert.AreEqual(0, achievements.DynamicAchievements.Count);
        }

        private (AchievementService, CreditsService, SaveService) CreateServices(bool enabled)
        {
            var saveService = folder.CreateSaveService();
            var credits = new CreditsService(saveService);
            var achievements = new AchievementService(saveService, credits, catalogue, new DynamicAchievementTargets(), enabled);
            saveService.Register(credits);
            saveService.Register(achievements);
            saveService.Load();
            return (achievements, credits, saveService);
        }
    }
}
