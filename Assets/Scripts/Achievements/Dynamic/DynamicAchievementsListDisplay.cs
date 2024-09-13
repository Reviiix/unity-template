using Achievements.List;

namespace Achievements.Dynamic
{
    /// <summary>
    /// This class displays the dynamic achievements in a list
    /// </summary>
    public class DynamicAchievementsListDisplay : AchievementListDisplay
    {
        private void OnEnable()
        {
            AchievementManager.OnDynamicAchievementsSet += OnAchievementsSet;
            AchievementManager.OnDynamicAchievementUnlocked += OnAchievementUnlocked;
        }
        
        private void OnDisable()
        {
            AchievementManager.OnDynamicAchievementsSet -= OnAchievementsSet;
            AchievementManager.OnDynamicAchievementUnlocked -= OnAchievementUnlocked;
        }

        private void OnAchievementsSet(Achievement[] achievements)
        {
            CreateListAsynchronously(achievements);
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
