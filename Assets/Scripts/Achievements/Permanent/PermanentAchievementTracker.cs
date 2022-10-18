using System;
using System.Collections;
using Credits;
using Player;
using Statistics.Experience;
using UnityEngine;

namespace Achievements
{
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
            //CheckTotalPlayTime();
        }

        private static void OnLevelChange(int levelID)
        {
            switch (levelID >= 1)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelTwo);
                    break;
            }
            switch (levelID >= 4)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelFive);
                    break;
            }
            switch (levelID >= 9)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelTen);
                    break;
            }
            switch (levelID >= 24)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelTwentyFive);
                    break;
            }
            switch (levelID >= 49)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelFifty);
                    break;
            }
            switch (levelID >= 99)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.ReachLevelOneHundred);
                    break;
            }
        }

        private static void CheckOpenTimes()
        {
            var timesGameHasBeenOpened = PlayerEngagementManager.TimesGameHasBeenOpened;
            switch (timesGameHasBeenOpened)
            {
                case 5:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OpenTheAppFiveTimes);
                    break;
                case 10:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OpenTheAppTenTimes);
                    break;
                case 25:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OpenTheAppTwentyFiveTimes);
                    break;
                case 50:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OpenTheAppFiftyTimes);
                    break;
                case 100:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OpenTheAppOneHundredTimes);
                    break;
            }
        }

        private static void CheckTotalPlayTime()
        {
            var totalPlayTime = PlayerEngagementManager.TotalPlayTime + TimeSpan.FromSeconds(Time.deltaTime);
            
            switch (totalPlayTime.Hours > 1)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForOneHour);
                    break;
            }
            switch (totalPlayTime.Hours > 5)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForFiveHours);
                    break;
            }
            switch (totalPlayTime.Hours > 10)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForTenHours);
                    break;
            }
            switch (totalPlayTime.Hours > 25)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForTwentyFiveHours);
                    break;
            }
            switch (totalPlayTime.Hours > 50)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForFiftyHours);
                    break;
            }
            switch (totalPlayTime.Hours > 100)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForOneHundredHours);
                    break;
            }
        }
        
        private static void CheckTotalCredits(long startingValue, long endValue) //TODO use parametres!
        {
            switch (endValue > 100)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneHundredCredits);
                    break;
            }
            switch (endValue > 250)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.TwoHundredAndFiftyCredits);
                    break;
            }
            switch (endValue > 500)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.FiveHundredCredits);
                    break;
            }
            switch (endValue > 1000)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneThousandCredits);
                    break;
            }
            switch (endValue > 10000)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.TenThousandCredits);
                    break;
            }
            switch (endValue > 100000)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneHundredThousandCredits);
                    break;
            }
        }
        
        private static void CheckTotalPremiumCredits(long startingValue, long endValue)
        {
            switch (endValue > 100)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneHundredPremiumCredits);
                    break;
            }
            switch (endValue > 250)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.TwoHundredAndFiftyPremiumCredits);
                    break;
            }
            switch (endValue > 500)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.FiveHundredPremiumCredits);
                    break;
            }
            switch (endValue > 1000)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneThousandPremiumCredits);
                    break;
            }
            switch (endValue > 10000)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.TenThousandPremiumCredits);
                    break;
            }
            switch (endValue > 100000)
            {
                case true:
                    PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.OneHundredThousandPremiumCredits);
                    break;
            }
        }
        
        private static IEnumerator ConsecutivePlayTimeAchievement()
        {
            yield return OneHour;
            PermanentAchievementManager.UnlockAchievement(PermanentAchievementManager.Achievement.PlayForOneHourConsecutively);
        }
    }
}
