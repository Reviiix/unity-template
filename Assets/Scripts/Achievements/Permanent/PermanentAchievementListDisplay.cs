using System.Collections;
using Achievements.List;
using pure_unity_methods;
using PureFunctions.UnitySpecific;
using UnityEngine;

namespace Achievements.Permanent
{
    /// <summary>
    /// This class displays the permanent achievements in a list
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class PermanentAchievementListDisplay : AchievementListDisplay
    {
        protected override IEnumerator Start()
        {
            yield return Wait.WaitForInitialisation;
            yield return StartCoroutine(base.Start());
            CreateDisplayAsynchronously();
        }

        private void OnEnable()
        {
            AchievementManager.OnPermanentAchievementUnlocked += OnAchievementUnlocked;
        }

        private void OnDisable()
        {
            AchievementManager.OnPermanentAchievementUnlocked += OnAchievementUnlocked;
        }

        private void CreateDisplayAsynchronously()
        {
            CreateListAsynchronously(AchievementManager.GetPermanentAchievements);
            SetRewardDisplay();
        }
        
        protected override void SetRewardDisplay()
        {
            const string rewardsPrefix = "Total Rewards: ";
            const string amountPrefix = "Total Achievements: ";
            rewardDisplay.text = rewardsPrefix + AchievementManager.GetTotalRewardsOfPermanentAchievements(true) + "/" + AchievementManager.GetTotalRewardsOfPermanentAchievements();
            amountOfUnlocksDisplay.text = amountPrefix + AchievementManager.GetAmountOfPermanentAchievements(true) + "/" + AchievementManager.GetAmountOfPermanentAchievements();
        }
        
    }
}