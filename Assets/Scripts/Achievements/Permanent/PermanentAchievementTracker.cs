using System;
using System.Collections;
using Achievements.Shared;
using Credits;
using Player;
using PureFunctions;
using Statistics.Experience;
using UnityEngine;

namespace Achievements.Permanent
{
    /// <summary>
    /// This class tracks progress towards unlocking permanent achievements.
    /// </summary>
    public class PermanentAchievementTracker : AchievementTracker
    {
        private static Coroutine _consecutivePlayTimeTracker;
        
        protected override void OnEnable()
        {
            ExperienceManager.OnLevelChange += OnLevelChange;
            ProjectManager.OnApplicationOpen += OnApplicationOpen;
            CreditsManager.OnCreditsChanged += CheckTotalCredits;
            CreditsManager.OnPremiumCreditsChanged += CheckTotalPremiumCredits;
            _consecutivePlayTimeTracker = StartCoroutine(ConsecutivePlayTimeAchievement());
            base.OnEnable(); //Do after event subscriptions.
        }
        
        protected override void OnDisable()
        {
            ExperienceManager.OnLevelChange -= OnLevelChange;
            ProjectManager.OnApplicationOpen -= OnApplicationOpen;
            CreditsManager.OnCreditsChanged -= CheckTotalCredits;
            CreditsManager.OnPremiumCreditsChanged -= CheckTotalPremiumCredits;
            if (_consecutivePlayTimeTracker != null)
            {
                StopCoroutine(_consecutivePlayTimeTracker);
            }
            base.OnDisable();
        }
        
        private static void OnApplicationOpen()
        {
            ProjectManager.OnApplicationOpen -= OnApplicationOpen;
            CheckOpenTimes();
        }

        protected override void PerformChecks()
        {
            CheckTotalPlayTime();
        }

        private static void OnLevelChange(int levelID)
        {
            switch (levelID >= 1)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelTwo);
                    break;
            }
            switch (levelID >= 4)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelFive);
                    break;
            }
            switch (levelID >= 9)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelTen);
                    break;
            }
            switch (levelID >= 24)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelTwentyFive);
                    break;
            }
            switch (levelID >= 49)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelFifty);
                    break;
            }
            switch (levelID >= 99)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelOneHundred);
                    break;
            }
        }

        private static void CheckOpenTimes()
        {
            var timesGameHasBeenOpened = PlayerEngagement.TimesGameHasBeenOpened;
            switch (timesGameHasBeenOpened)
            {
                case 5:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OpenTheAppFiveTimes);
                    break;
                case 10:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OpenTheAppTenTimes);
                    break;
                case 25:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OpenTheAppTwentyFiveTimes);
                    break;
                case 50:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OpenTheAppFiftyTimes);
                    break;
                case 100:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OpenTheAppOneHundredTimes);
                    break;
            }
        }

        private static void CheckTotalPlayTime()
        {
            var totalPlayTime = PlayerEngagement.TotalPlayTime + TimeSpan.FromSeconds(Time.deltaTime);
            
            switch (totalPlayTime.Hours > 1)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForOneHour);
                    break;
            }
            switch (totalPlayTime.Hours > 5)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForFiveHours);
                    break;
            }
            switch (totalPlayTime.Hours > 10)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForTenHours);
                    break;
            }
            switch (totalPlayTime.Hours > 25)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForTwentyFiveHours);
                    break;
            }
            switch (totalPlayTime.Hours > 50)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForFiftyHours);
                    break;
            }
            switch (totalPlayTime.Hours > 100)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForOneHundredHours);
                    break;
            }
        }
        
        private static void CheckTotalCredits(ValueChangeInformation valueChangeInformation)
        {
            var credits = valueChangeInformation.NewValue;
            switch (credits > 100)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneHundredCredits);
                    break;
            }
            switch (credits > 250)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.TwoHundredAndFiftyCredits);
                    break;
            }
            switch (credits > 500)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.FiveHundredCredits);
                    break;
            }
            switch (credits > 1000)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneThousandCredits);
                    break;
            }
            switch (credits > 10000)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.TenThousandCredits);
                    break;
            }
            switch (credits > 100000)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneHundredThousandCredits);
                    break;
            }
        }
        
        private static void CheckTotalPremiumCredits(ValueChangeInformation valueChangeInformation)
        {
            var credits = valueChangeInformation.NewValue;
            switch (credits > 100)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneHundredPremiumCredits);
                    break;
            }
            switch (credits > 250)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.TwoHundredAndFiftyPremiumCredits);
                    break;
            }
            switch (credits > 500)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.FiveHundredPremiumCredits);
                    break;
            }
            switch (credits > 1000)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneThousandPremiumCredits);
                    break;
            }
            switch (credits > 10000)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.TenThousandPremiumCredits);
                    break;
            }
            switch (credits > 100000)
            {
                case true:
                    AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneHundredThousandPremiumCredits);
                    break;
            }
        }
        
        private static  IEnumerator ConsecutivePlayTimeAchievement()
        {
            yield return WaitOneHour;
            AchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForOneHourConsecutively);
        }
    }
}
