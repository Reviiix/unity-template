using System;
using System.Collections;
using Abstract;
using Credits;
using Player;
using UnityEngine;

namespace Achievements
{
    public class AchievementTracker : PrivateSingleton<AchievementTracker>
    {
        private static readonly WaitForSeconds ConsecutivePlayTimeAchievementTime = new WaitForSeconds(3600);
        
        private static Coroutine _intermittentChecks;
        private static readonly WaitForSeconds TimeBetweenIntermittentChecks = new WaitForSeconds(3600);
        
        protected override void OnEnable()
        {
            base.OnEnable();
            ProjectManager.OnApplicationOpen += OnApplicationOpen;
            _intermittentChecks = StartCoroutine(PerformChecksIntermittently());
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            ProjectManager.OnApplicationOpen -= OnApplicationOpen;
            if (_intermittentChecks != null) StopCoroutine(_intermittentChecks);
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
            yield return TimeBetweenIntermittentChecks;
            _intermittentChecks = StartCoroutine(PerformChecksIntermittently());
        }

        private static void PerformChecks()
        {
            CheckTotalPlayTime();
            CheckTotalCredits();
            CheckTotalPremiumCredits();
        }

        private static void CheckOpenTimes()
        {
            var timesGameHasBeenOpened = PlayerEngagementManager.TimesGameHasBeenOpened;
            
            if (timesGameHasBeenOpened == 5) AchievementManager.UnlockAchievement(AchievementManager.Achievement.OpenTheAppFiveTimes);
            if (timesGameHasBeenOpened == 10) AchievementManager.UnlockAchievement(AchievementManager.Achievement.OpenTheAppTenTimes);
            if (timesGameHasBeenOpened == 25) AchievementManager.UnlockAchievement(AchievementManager.Achievement.OpenTheAppTwentyFiveTimes);
            if (timesGameHasBeenOpened == 50) AchievementManager.UnlockAchievement(AchievementManager.Achievement.OpenTheAppFiftyTimes);
            if (timesGameHasBeenOpened == 100) AchievementManager.UnlockAchievement(AchievementManager.Achievement.OpenTheAppOneHundredTimes);
        }

        private static void CheckTotalPlayTime()
        {
            var totalPlayTime = PlayerEngagementManager.TotalPlayTime + TimeSpan.FromSeconds(Time.deltaTime);
            
            if (totalPlayTime.Hours > 1) AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForOneHour);
            if (totalPlayTime.Hours > 5) AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForFiveHours);
            if (totalPlayTime.Hours > 10) AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForTenHours);
            if (totalPlayTime.Hours > 25) AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForTwentyFiveHours);
            if (totalPlayTime.Hours > 50) AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForFiftyHours);
        }
        
        private static void CheckTotalCredits()
        {
            var credits = CreditsManager.ReturnCredits(CreditsManager.Currency.Credits);
            
            if (credits > 100) AchievementManager.UnlockAchievement(AchievementManager.Achievement.OneHundredCredits);
            if (credits > 250) AchievementManager.UnlockAchievement(AchievementManager.Achievement.TwoHundredAndFiftyCredits);
            if (credits > 500) AchievementManager.UnlockAchievement(AchievementManager.Achievement.FiveHundredCredits);
            if (credits > 1000) AchievementManager.UnlockAchievement(AchievementManager.Achievement.OneThousandCredits);
            if (credits > 10000) AchievementManager.UnlockAchievement(AchievementManager.Achievement.TenThousandCredits);
        }
        
        private static void CheckTotalPremiumCredits()
        {
            var credits = CreditsManager.ReturnCredits(CreditsManager.Currency.PremiumCredits);
            
            if (credits > 100) AchievementManager.UnlockAchievement(AchievementManager.Achievement.OneHundredPremiumCredits);
            if (credits > 250) AchievementManager.UnlockAchievement(AchievementManager.Achievement.TwoHundredAndFiftyPremiumCredits);
            if (credits > 500) AchievementManager.UnlockAchievement(AchievementManager.Achievement.FiveHundredPremiumCredits);
            if (credits > 1000) AchievementManager.UnlockAchievement(AchievementManager.Achievement.OneThousandPremiumCredits);
            if (credits > 10000) AchievementManager.UnlockAchievement(AchievementManager.Achievement.TenThousandPremiumCredits);
        }
        
        private static IEnumerator ConsecutivePlayTimeAchievement()
        {
            yield return ConsecutivePlayTimeAchievementTime;
            AchievementManager.UnlockAchievement(AchievementManager.Achievement.PlayForOneHourConsecutively);
        }
    }
}
