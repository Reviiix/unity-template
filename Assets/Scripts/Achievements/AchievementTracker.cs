using System;
using System.Collections;
using Achievements.Permanent;
using Credits;
using Player;
using PureFunctions;
using Statistics.Experience;
using UnityEngine;

namespace Achievements
{
    /// <summary>
    /// This class is the base for achievement tracking. it is shared by dynamic and permanent achievements.
    /// </summary>
    public  class AchievementTracker : MonoBehaviour
    {
        private Coroutine intermittentChecks;
        private static readonly WaitForSeconds WaitTenMinutes = new (600);
        protected static readonly WaitForSeconds WaitOneHour = new (3600);
        private static Coroutine _consecutivePlayTimeTracker;
        
        protected  void OnEnable()
        {
            ExperienceManager.OnLevelChange += OnLevelChange;
            ProjectManager.OnApplicationOpen += OnApplicationOpen;
            CreditsManager.OnCreditsChanged += CheckTotalCredits;
            CreditsManager.OnPremiumCreditsChanged += CheckTotalPremiumCredits;
            _consecutivePlayTimeTracker = StartCoroutine(ConsecutivePlayTimeAchievement());
            if (!ProjectManager.EnabledFeatures.Achievements)
            {
                Destroy(this);
                return;
            }
            intermittentChecks = StartCoroutine(PerformChecksIntermittently());
        }
        
        protected void OnDisable()
        {
            ExperienceManager.OnLevelChange -= OnLevelChange;
            ProjectManager.OnApplicationOpen -= OnApplicationOpen;
            CreditsManager.OnCreditsChanged -= CheckTotalCredits;
            CreditsManager.OnPremiumCreditsChanged -= CheckTotalPremiumCredits;
            if (_consecutivePlayTimeTracker != null)
            {
                StopCoroutine(_consecutivePlayTimeTracker);
            }            
            if (intermittentChecks != null)
            {
                StopCoroutine(intermittentChecks);
            }
        }
    
        // ReSharper disable once FunctionRecursiveOnAllPaths
        private IEnumerator PerformChecksIntermittently()
        {
            PerformChecks();
            yield return WaitTenMinutes;
            intermittentChecks = StartCoroutine(PerformChecksIntermittently());
        }
        
        private static void OnApplicationOpen()
        {
            ProjectManager.OnApplicationOpen -= OnApplicationOpen;
            CheckOpenTimes();
        }

        private static void PerformChecks()
        {
            CheckTotalPlayTime();
        }

        private static void OnLevelChange(int levelID)
        {
            switch (levelID >= 1)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.ReachLevelTwo);
                    break;
            }
            switch (levelID >= 4)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.ReachLevelFive);
                    break;
            }
            switch (levelID >= 9)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.ReachLevelTen);
                    break;
            }
            switch (levelID >= 24)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.ReachLevelTwentyFive);
                    break;
            }
            switch (levelID >= 49)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.ReachLevelFifty);
                    break;
            }
            switch (levelID >= 99)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.ReachLevelOneHundred);
                    break;
            }
        }

        private static void CheckOpenTimes()
        {
            var timesGameHasBeenOpened = PlayerEngagement.TimesGameHasBeenOpened;
            switch (timesGameHasBeenOpened)
            {
                case 5:
                    AchievementManager.UnlockAchievement(Achievement.OpenTheAppFiveTimes);
                    break;
                case 10:
                    AchievementManager.UnlockAchievement(Achievement.OpenTheAppTenTimes);
                    break;
                case 25:
                    AchievementManager.UnlockAchievement(Achievement.OpenTheAppTwentyFiveTimes);
                    break;
                case 50:
                    AchievementManager.UnlockAchievement(Achievement.OpenTheAppFiftyTimes);
                    break;
                case 100:
                    AchievementManager.UnlockAchievement(Achievement.OpenTheAppOneHundredTimes);
                    break;
            }
        }

        private static void CheckTotalPlayTime()
        {
            var totalPlayTime = PlayerEngagement.TotalPlayTime + TimeSpan.FromSeconds(Time.deltaTime);
            
            switch (totalPlayTime.Hours > 1)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.PlayForOneHour);
                    break;
            }
            switch (totalPlayTime.Hours > 5)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.PlayForFiveHours);
                    break;
            }
            switch (totalPlayTime.Hours > 10)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.PlayForTenHours);
                    break;
            }
            switch (totalPlayTime.Hours > 25)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.PlayForTwentyFiveHours);
                    break;
            }
            switch (totalPlayTime.Hours > 50)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.PlayForFiftyHours);
                    break;
            }
            switch (totalPlayTime.Hours > 100)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.PlayForOneHundredHours);
                    break;
            }
        }
        
        private static void CheckTotalCredits(ValueChangeInformation valueChangeInformation)
        {
            var credits = valueChangeInformation.NewValue;
            switch (credits > 100)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.OneHundredCredits);
                    break;
            }
            switch (credits > 250)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.TwoHundredAndFiftyCredits);
                    break;
            }
            switch (credits > 500)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.FiveHundredCredits);
                    break;
            }
            switch (credits > 1000)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.OneThousandCredits);
                    break;
            }
            switch (credits > 10000)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.TenThousandCredits);
                    break;
            }
            switch (credits > 100000)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.OneHundredThousandCredits);
                    break;
            }
        }
        
        private static void CheckTotalPremiumCredits(ValueChangeInformation valueChangeInformation)
        {
            var credits = valueChangeInformation.NewValue;
            switch (credits > 100)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.OneHundredPremiumCredits);
                    break;
            }
            switch (credits > 250)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.TwoHundredAndFiftyPremiumCredits);
                    break;
            }
            switch (credits > 500)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.FiveHundredPremiumCredits);
                    break;
            }
            switch (credits > 1000)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.OneThousandPremiumCredits);
                    break;
            }
            switch (credits > 10000)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.TenThousandPremiumCredits);
                    break;
            }
            switch (credits > 100000)
            {
                case true:
                    AchievementManager.UnlockAchievement(Achievement.OneHundredThousandPremiumCredits);
                    break;
            }
        }
        
        private static  IEnumerator ConsecutivePlayTimeAchievement()
        {
            yield return WaitOneHour;
            AchievementManager.UnlockAchievement(Achievement.PlayForOneHourConsecutively);
        }
    }
}
