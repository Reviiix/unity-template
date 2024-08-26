using System;
using Achievements.Dynamic;
using Achievements.Permanent;
using UnityEngine.AddressableAssets;

public static class AchievementManager
{
    private static readonly PermanentAchievementManager PermanentAchievements = new ();
    private static readonly DynamicAchievementManager DynamicAchievements = new();
    
    public static Action<PermanentAchievementManager.Achievement> OnPermanentAchievementUnlocked;
    public static Action<DynamicAchievementManager.Achievement> OnDynamicAchievementUnlocked;
    public static Action<DynamicAchievementManager.Achievement[]> OnDynamicAchievementsSet;

    #region Achievement Getters
    public static int GetTotalRewardsOfPermanentAchievements(bool unlockedOnly = false) => PermanentAchievements.GetTotalRewards(unlockedOnly);
    public static int GetTotalRewardsOfDynamicAchievements(bool unlockedOnly = false) => DynamicAchievements.GetTotalRewards(unlockedOnly);

    public static int GetAmountOfPermanentAchievements(bool unlockedOnly = false) => PermanentAchievements.ReturnAmountOfAchievements(unlockedOnly);
    public static int GetAmountOfDynamicAchievements(bool unlockedOnly = false) => DynamicAchievements.ReturnAmountOfAchievements(unlockedOnly);
    public static AssetReference GetSpriteAssetReference(PermanentAchievementManager.Achievement achievement) => PermanentAchievements.GetSpriteAssetReference(achievement);
    public static AssetReference GetSpriteAssetReference(DynamicAchievementManager.Achievement achievement) => DynamicAchievements.GetSpriteAssetReference(achievement);
    public static string GetDescription(PermanentAchievementManager.Achievement achievement) => PermanentAchievements.GetDescription(achievement);
    public static string GetDescription(DynamicAchievementManager.Achievement achievement) => DynamicAchievements.GetDescription(achievement);
    public static int GetReward(PermanentAchievementManager.Achievement achievement) => PermanentAchievements.GetReward(achievement);
    public static int GetReward(DynamicAchievementManager.Achievement achievement) => DynamicAchievements.GetReward(achievement);
    public static bool GetUnlockedState(PermanentAchievementManager.Achievement achievement) => PermanentAchievements.GetUnlockedState(achievement);
    public static bool GetUnlockedState(DynamicAchievementManager.Achievement achievement) => DynamicAchievements.GetUnlockedState(achievement);
    public static PermanentAchievementManager.Achievement[] GetPermanentAchievements => PermanentAchievements.GetAchievements();
    public static DynamicAchievementManager.Achievement[] GetDynamicAchievements => DynamicAchievements.GetAchievements();
    public static bool[] GetUnLockStatesOfPermanentAchievements() => PermanentAchievements.GetUnLockStates();
    public static bool[] GetUnLockStatesOfDynamicAchievements() => DynamicAchievements.GetUnLockStates();
    #endregion Achievement Getters

    public static void Initialise()
    {
        PermanentAchievements.Initialise();
        DynamicAchievements.Initialise();
    }

    public static void UnlockAchievement(PermanentAchievementManager.Achievement achievement)
    {
        PermanentAchievements.UnlockAchievement(achievement);
        OnPermanentAchievementUnlocked(achievement);
    }
    
    public static void UnlockAchievement(DynamicAchievementManager.Achievement achievement)
    {
        DynamicAchievements.UnlockAchievement(achievement);
        OnDynamicAchievementUnlocked(achievement);
    }

    public static void DynamicAchievementsSet(DynamicAchievementManager.Achievement[] achievements)
    {
        OnDynamicAchievementsSet?.Invoke(achievements);
    }

}
