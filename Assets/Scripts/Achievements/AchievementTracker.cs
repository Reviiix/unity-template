using System;
using System.Collections;
using Abstract;
using Credits;
using Player;
using Statistics.Experience;
using UnityEngine;

namespace Achievements
{
    public class AchievementTracker : PrivateSingleton<AchievementTracker>
    {
        private static Coroutine _intermittentChecks;
        private static Coroutine _consecutivePlayTimeTracker;
        private static readonly WaitForSeconds OneHour = new WaitForSeconds(3600);
        
        protected override void OnEnable()
        {
            base.OnEnable();
            ExperienceManager.OnLevelChange += OnLevelChange;
            ProjectManager.OnApplicationOpen += OnApplicationOpen;
            _intermittentChecks = StartCoroutine(PerformChecksIntermittently());
            _consecutivePlayTimeTracker = StartCoroutine(ConsecutivePlayTimeAchievement());
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            ExperienceManager.OnLevelChange -= OnLevelChange;
            ProjectManager.OnApplicationOpen -= OnApplicationOpen;
            if (_intermittentChecks != null)
            {
                StopCoroutine(_intermittentChecks);
            }

            if (_consecutivePlayTimeTracker != null)
            {
                StopCoroutine(_consecutivePlayTimeTracker);
            }
        }
        
        private static void OnApplicationOpen()
        {
            ProjectManager.OnApplicationOpen -= OnApplicationOpen;
            CheckOpenTimes();
        }

        // ReSharper disable once FunctionRecursiveOnAllPaths
        private IEnumerator PerformChecksIntermittently()
        {
            PerformChecks();
            yield return OneHour;
            _intermittentChecks = StartCoroutine(PerformChecksIntermittently());
        }

        private static void PerformChecks()
        {
            CheckTotalPlayTime();
            CheckTotalCredits();
            CheckTotalPremiumCredits();
        }

        private static void OnLevelChange(int levelID)
        {
            switch (levelID)
            {
                case 1:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.ReachLevelTwo);
                    break;
                case 4:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.ReachLevelFive);
                    break;
                case 9:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.ReachLevelTen);
                    break;
                case 24:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.ReachLevelTwentyFive);
                    break;
                case 49:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.ReachLevelFifty);
                    break;
                case 99:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.ReachLevelOneHundred);
                    break;
            }
        }

        private static void CheckOpenTimes()
        {
            var timesGameHasBeenOpened = PlayerEngagementManager.TimesGameHasBeenOpened;
            switch (timesGameHasBeenOpened)
            {
                case 5:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.OpenTheAppFiveTimes);
                    break;
                case 10:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.OpenTheAppTenTimes);
                    break;
                case 25:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.OpenTheAppTwentyFiveTimes);
                    break;
                case 50:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.OpenTheAppFiftyTimes);
                    break;
                case 100:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.OpenTheAppOneHundredTimes);
                    break;
            }
        }

        private static void CheckTotalPlayTime()
        {
            var totalPlayTime = PlayerEngagementManager.TotalPlayTime + TimeSpan.FromSeconds(Time.deltaTime);
            
            switch (totalPlayTime.Hours > 1)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForOneHour);
                    break;
            }
            switch (totalPlayTime.Hours > 5)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForFiveHours);
                    break;
            }
            switch (totalPlayTime.Hours > 10)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForTenHours);
                    break;
            }
            switch (totalPlayTime.Hours > 25)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForTwentyFiveHours);
                    break;
            }
            switch (totalPlayTime.Hours > 50)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForFiftyHours);
                    break;
            }
            switch (totalPlayTime.Hours > 100)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.PLayForOneHundredHours);
                    break;
            }
        }
        
        private static void CheckTotalCredits()
        {
            var credits = CreditsManager.ReturnCredits(CreditsManager.Currency.Credits);
            
            switch (credits > 100)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.OneHundredCredits);
                    break;
            }
            switch (credits > 250)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.TwoHundredAndFiftyCredits);
                    break;
            }
            switch (credits > 500)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.FiveHundredCredits);
                    break;
            }
            switch (credits > 1000)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.OneThousandCredits);
                    break;
            }
            switch (credits > 10000)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.TenThousandCredits);
                    break;
            }
            switch (credits > 100000)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.OneHundredThousandCredits);
                    break;
            }
        }
        
        private static void CheckTotalPremiumCredits()
        {
            var credits = CreditsManager.ReturnCredits(CreditsManager.Currency.PremiumCredits);
            
            switch (credits > 100)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.OneHundredPremiumCredits);
                    break;
            }
            switch (credits > 250)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.TwoHundredAndFiftyPremiumCredits);
                    break;
            }
            switch (credits > 500)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.FiveHundredPremiumCredits);
                    break;
            }
            switch (credits > 1000)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.OneThousandPremiumCredits);
                    break;
            }
            switch (credits > 10000)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.TenThousandPremiumCredits);
                    break;
            }
            switch (credits > 100000)
            {
                case true:
                    AchievementManager.UnlockAchievement(AchievementManager.Achievement.OneHundredThousandPremiumCredits);
                    break;
            }
        }
        
        private static IEnumerator ConsecutivePlayTimeAchievement()
        {
            yield return OneHour;
            AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForOneHourConsecutively);
        }
    }
}
