using UnityEngine;
using UnityTemplate.Core;
using UnityTemplate.Economy;
using UnityTemplate.Engagement;
using UnityTemplate.Progression;

namespace UnityTemplate.Achievements
{
    /// <summary>Watches levels, app opens, balances and play time and unlocks the matching permanent achievements.</summary>
    public class AchievementTracker : MonoBehaviour
    {
        private const float PlayTimeCheckIntervalSeconds = 600;
        private const float OneHourInSeconds = 3600;
        private static readonly (int LevelIndex, Achievement Achievement)[] LevelAchievements =
        {
            (1, Achievement.ReachLevelTwo), (4, Achievement.ReachLevelFive), (9, Achievement.ReachLevelTen),
            (24, Achievement.ReachLevelTwentyFive), (49, Achievement.ReachLevelFifty), (99, Achievement.ReachLevelOneHundred),
        };
        private static readonly (int Opens, Achievement Achievement)[] OpenAchievements =
        {
            (1, Achievement.OpenTheAppOnce), (5, Achievement.OpenTheAppFiveTimes), (10, Achievement.OpenTheAppTenTimes),
            (25, Achievement.OpenTheAppTwentyFiveTimes), (50, Achievement.OpenTheAppFiftyTimes), (100, Achievement.OpenTheAppOneHundredTimes),
        };
        private static readonly (int Hours, Achievement Achievement)[] PlayTimeAchievements =
        {
            (1, Achievement.PlayForOneHour), (5, Achievement.PlayForFiveHours), (10, Achievement.PlayForTenHours),
            (25, Achievement.PlayForTwentyFiveHours), (50, Achievement.PlayForFiftyHours), (100, Achievement.PlayForOneHundredHours),
        };
        private static readonly (long Balance, Achievement Credits, Achievement Premium)[] BalanceAchievements =
        {
            (100, Achievement.OneHundredCredits, Achievement.OneHundredPremiumCredits),
            (250, Achievement.TwoHundredAndFiftyCredits, Achievement.TwoHundredAndFiftyPremiumCredits),
            (500, Achievement.FiveHundredCredits, Achievement.FiveHundredPremiumCredits),
            (1000, Achievement.OneThousandCredits, Achievement.OneThousandPremiumCredits),
            (10000, Achievement.TenThousandCredits, Achievement.TenThousandPremiumCredits),
            (100000, Achievement.OneHundredThousandCredits, Achievement.OneHundredThousandPremiumCredits),
        };
        private AchievementService achievements;
        private ExperienceService experience;
        private CreditsService credits;
        private EngagementService engagement;

        private void OnEnable()
        {
            achievements = ServiceLocator.Get<AchievementService>();
            if (!achievements.IsEnabled) return;

            experience = ServiceLocator.Get<ExperienceService>();
            credits = ServiceLocator.Get<CreditsService>();
            engagement = ServiceLocator.Get<EngagementService>();
            experience.LevelChanged += OnLevelChanged;
            credits.BalanceChanged += OnBalanceChanged;

            foreach (var (opens, achievement) in OpenAchievements)
            {
                if (engagement.TimesOpened >= opens) achievements.Unlock(achievement);
            }
            CheckPlayTimePeriodicallyAsync();
            UnlockAfterOneHourOfPlayAsync();
        }

        private void OnDisable()
        {
            if (!achievements.IsEnabled) return;

            experience.LevelChanged -= OnLevelChanged;
            credits.BalanceChanged -= OnBalanceChanged;
        }

        private void OnLevelChanged(int levelIndex)
        {
            foreach (var (requiredLevel, achievement) in LevelAchievements)
            {
                if (levelIndex >= requiredLevel) achievements.Unlock(achievement);
            }
        }

        private void OnBalanceChanged(Currency currency, ValueChange change)
        {
            foreach (var (balance, creditsAchievement, premiumAchievement) in BalanceAchievements)
            {
                if (change.NewValue > balance) achievements.Unlock(currency == Currency.Credits ? creditsAchievement : premiumAchievement);
            }
        }

        private async void CheckPlayTimePeriodicallyAsync()
        {
            try
            {
                while (true)
                {
                    var hoursPlayed = engagement.TotalPlayTime.TotalHours;
                    foreach (var (hours, achievement) in PlayTimeAchievements)
                    {
                        if (hoursPlayed >= hours) achievements.Unlock(achievement);
                    }
                    await Awaitable.WaitForSecondsAsync(PlayTimeCheckIntervalSeconds, destroyCancellationToken);
                }
            }
            catch (System.OperationCanceledException)
            {
            }
        }

        private async void UnlockAfterOneHourOfPlayAsync()
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(OneHourInSeconds, destroyCancellationToken);
                achievements.Unlock(Achievement.PlayForOneHourConsecutively);
            }
            catch (System.OperationCanceledException)
            {
            }
        }
    }
}
