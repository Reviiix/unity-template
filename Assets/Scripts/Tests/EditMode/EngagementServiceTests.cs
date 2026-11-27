using System;
using NUnit.Framework;
using UnityTemplate.Engagement;

namespace UnityTemplate.Tests
{
    /// <summary>Tests the welcome reward (owed once, kept until claimed), the daily bonus (once per day, streaks, clock tampering) and first-open anniversaries.</summary>
    public class EngagementServiceTests
    {
        private static readonly DateTime Monday = new(2026, 11, 9, 10, 0, 0);
        private TestSaveFolder folder;
        private double clockSeconds;
        private EngagementService engagement;

        [SetUp]
        public void CreateService()
        {
            folder = new TestSaveFolder();
            engagement = LoadEngagementFromSave();
        }

        [TearDown]
        public void DeleteFolder()
        {
            folder.Dispose();
        }

        [Test]
        public void TheDailyBonusCanBeClaimedOncePerDay()
        {
            Assert.IsTrue(engagement.IsDailyBonusAvailable(Monday));
            Assert.IsTrue(engagement.ClaimDailyBonus(Monday));
            Assert.IsFalse(engagement.IsDailyBonusAvailable(Monday.AddHours(10)));
            Assert.IsFalse(engagement.ClaimDailyBonus(Monday.AddHours(10)));
            Assert.IsTrue(engagement.IsDailyBonusAvailable(Monday.AddDays(1)));
        }

        [Test]
        public void InstallDayShowsTheWelcomeInsteadOfTheDailyBonus()
        {
            engagement.RecordAppOpen(Monday);

            Assert.IsTrue(engagement.IsFirstOpen);
            Assert.IsFalse(engagement.IsDailyBonusAvailable(Monday.AddHours(5)));
            Assert.IsTrue(engagement.IsDailyBonusAvailable(Monday.AddDays(1).Date));
        }

        [Test]
        public void OnlyTheFirstOpenOwesTheWelcomeRewardAndItIsPaidOnce()
        {
            Assert.IsFalse(engagement.IsWelcomeRewardPending);
            engagement.RecordAppOpen(Monday);

            Assert.IsTrue(engagement.IsWelcomeRewardPending);
            Assert.IsTrue(engagement.ClaimWelcomeReward());
            Assert.IsFalse(engagement.IsWelcomeRewardPending);
            Assert.IsFalse(engagement.ClaimWelcomeReward());

            engagement.RecordAppOpen(Monday.AddDays(1));
            Assert.IsFalse(engagement.IsWelcomeRewardPending);
        }

        [Test]
        public void AnUnclaimedWelcomeRewardIsStillOwedAfterARelaunch()
        {
            engagement.RecordAppOpen(Monday);

            var relaunched = LoadEngagementFromSave();
            relaunched.RecordAppOpen(Monday.AddHours(1));
            Assert.IsTrue(relaunched.IsWelcomeRewardPending);
            relaunched.ClaimWelcomeReward();

            Assert.IsFalse(LoadEngagementFromSave().IsWelcomeRewardPending);
        }

        [Test]
        public void ConsecutiveDaysGrowTheStreakAndAGapResetsIt()
        {
            engagement.ClaimDailyBonus(Monday);
            Assert.AreEqual(2 * EngagementService.DailyBonusCreditsPerStreakDay, engagement.GetDailyBonusCredits(Monday.AddDays(1)));
            engagement.ClaimDailyBonus(Monday.AddDays(1));
            Assert.AreEqual(3, engagement.GetStreakForClaim(Monday.AddDays(2)));
            Assert.AreEqual(1, engagement.GetStreakForClaim(Monday.AddDays(4)));
        }

        [Test]
        public void MovingTheClockBackwardsGivesNoBonus()
        {
            engagement.ClaimDailyBonus(Monday);

            Assert.IsFalse(engagement.IsDailyBonusAvailable(Monday.AddDays(-3)));
        }

        [Test]
        public void ThePendingWheelResultSurvivesUntilClaimed()
        {
            engagement.SetPendingWheelSegment(4);
            Assert.AreEqual(4, engagement.PendingWheelSegment);

            engagement.ClaimDailyBonus(Monday);

            Assert.AreEqual(EngagementService.NoPendingWheelSegment, engagement.PendingWheelSegment);
        }

        [Test]
        public void AnniversaryBecomesAvailableOnceAYearAndStaysUntilClaimed()
        {
            engagement.RecordAppOpen(new DateTime(2025, 3, 10));

            Assert.IsFalse(engagement.IsAnniversaryAvailable(new DateTime(2026, 3, 9)));
            Assert.IsTrue(engagement.IsAnniversaryAvailable(new DateTime(2026, 3, 10)));
            Assert.IsTrue(engagement.IsAnniversaryAvailable(new DateTime(2026, 3, 20)));
            Assert.IsTrue(engagement.ClaimAnniversary(new DateTime(2026, 3, 20)));
            Assert.IsFalse(engagement.IsAnniversaryAvailable(new DateTime(2026, 12, 1)));
            Assert.AreEqual(200, engagement.GetAnniversaryCredits(new DateTime(2027, 3, 10)));
        }

        [Test]
        public void ALeapDayFirstOpenCelebratesOnTheTwentyEighthInOtherYears()
        {
            engagement.RecordAppOpen(new DateTime(2024, 2, 29));

            Assert.IsFalse(engagement.IsAnniversaryAvailable(new DateTime(2025, 2, 27)));
            Assert.IsTrue(engagement.IsAnniversaryAvailable(new DateTime(2025, 2, 28)));
        }

        [Test]
        public void PlayTimeExcludesTimeSpentInTheBackground()
        {
            engagement.ResumeSession();
            clockSeconds = 60;
            engagement.PauseSession();
            clockSeconds = 600;
            engagement.ResumeSession();
            clockSeconds = 630;

            Assert.AreEqual(90, engagement.TotalPlayTime.TotalSeconds, 0.001);
        }
    
        /// <summary>Creates an engagement service from the test folder's save file, as a fresh launch would.</summary>
        private EngagementService LoadEngagementFromSave()
        {
            var saveService = folder.CreateSaveService();
            var loaded = new EngagementService(saveService, () => clockSeconds);
            saveService.Register(loaded);
            saveService.Load();
            return loaded;
        }
    }
}
