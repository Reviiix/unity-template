using System;
using Achievements.Dynamic;
using Achievements.Permanent;
using UnityEngine.AddressableAssets;

namespace Achievements
{
    public static class AchievementManager
    {
        private static readonly PermanentAchievementManager PermanentAchievements = new ();
        private static readonly DynamicAchievementManager DynamicAchievements = new();
    
        public static Action<Achievement> OnPermanentAchievementUnlocked;
        public static Action<Achievement> OnDynamicAchievementUnlocked;
        public static Action<Achievement[]> OnDynamicAchievementsSet;

        #region Achievement Getters
        public static int GetTotalRewardsOfPermanentAchievements(bool unlockedOnly = false) => PermanentAchievements.GetTotalRewards(unlockedOnly);
        public static int GetTotalRewardsOfDynamicAchievements(bool unlockedOnly = false) => DynamicAchievements.GetTotalRewards(unlockedOnly);

        public static int GetAmountOfPermanentAchievements(bool unlockedOnly = false) => PermanentAchievements.ReturnAmountOfAchievements(unlockedOnly);
        public static int GetAmountOfDynamicAchievements(bool unlockedOnly = false) => DynamicAchievements.ReturnAmountOfAchievements(unlockedOnly);
        public static AssetReference GetSpriteAssetReference(Achievement achievement) => PermanentAchievements.AchievementIsInSet(achievement) ? PermanentAchievements.GetSpriteAssetReference(achievement) : DynamicAchievements.GetSpriteAssetReference(achievement);
        public static string GetDescription(Achievement achievement) => PermanentAchievements.AchievementIsInSet(achievement) ? PermanentAchievements.GetDescription(achievement) : DynamicAchievements.GetDescription(achievement);
        public static int GetReward(Achievement achievement) => PermanentAchievements.AchievementIsInSet(achievement) ? PermanentAchievements.GetReward(achievement) : DynamicAchievements.GetReward(achievement);
        public static bool GetUnlockedState(Achievement achievement) => PermanentAchievements.AchievementIsInSet(achievement) ? PermanentAchievements.GetUnlockedState(achievement) : DynamicAchievements.GetUnlockedState(achievement);
        public static Achievement[] GetPermanentAchievements => PermanentAchievements.GetAchievements();
        public static Achievement[] GetDynamicAchievements => DynamicAchievements.GetAchievements();
        public static bool[] GetUnLockStatesOfPermanentAchievements() => PermanentAchievements.GetUnLockStates();
        public static bool[] GetUnLockStatesOfDynamicAchievements() => DynamicAchievements.GetUnLockStates();
        #endregion Achievement Getters

        public static void Initialise()
        {
            PermanentAchievements.Initialise();
            DynamicAchievements.Initialise();
        }

        public static void UnlockAchievement(Achievement achievement)
        {
            PermanentAchievements.UnlockAchievement(achievement);
            DynamicAchievements.UnlockAchievement(achievement);
            OnPermanentAchievementUnlocked(achievement);
        }

        public static void DynamicAchievementsSet(Achievement[] achievements)
        {
            OnDynamicAchievementsSet?.Invoke(achievements);
        }
    }
}
