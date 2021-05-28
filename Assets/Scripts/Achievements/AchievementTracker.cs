using System;
using Player;
using UnityEngine;

namespace Achievements
{
    public static class AchievementTracker
    {
        private static TimeSpan TotalPlayTime => PlayerInformation.TotalPlayTime += TimeSpan.FromSeconds(Time.deltaTime);

        public static void Initialise()
        {
            ProjectManager.OnApplicationOpen += OnApplicationOpen;
        }

        // Update is called once per frame
        private static void OnApplicationOpen()
        {
            CheckOpenTimes();
        }

        public static void PerformPeriodicalChecks()
        {
            CheckTotalPlayTime();
        }

        private static void CheckOpenTimes()
        {
            if (PlayerEngagementManager.TimesGameHasBeenOpened == 5) AchievementManager.UnlockAchievement(AchievementManager.Achievement.OpenTheAppFiveTimes);
        }

        private static void CheckTotalPlayTime()
        {
            if (TotalPlayTime.Hours > 1) AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForOneHour);
            if (TotalPlayTime.Hours > 5) AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForFiveHours);
            if (TotalPlayTime.Hours > 10) AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForTenHours);
            if (TotalPlayTime.Hours > 25) AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForTwentyFiveHours);
            if (TotalPlayTime.Hours > 50) AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForFiftyHours);
            
        }
    }
}
