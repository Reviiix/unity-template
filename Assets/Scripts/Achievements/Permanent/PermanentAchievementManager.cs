using System;
using System.Collections.Generic;
using Credits;
using UnityEngine.AddressableAssets;

namespace Achievements.Permanent
{
    /// <summary>
    /// This class manages the permanent achievements.
    /// </summary>
    public class PermanentAchievementManager : BaseAchievementManager
    {
        private readonly Dictionary<Achievement, AchievementInformation> permanentAchievements = new()
        {
            {Achievement.PlayForOneHourConsecutively, new AchievementInformation("Play For One Hour Consecutively.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            
            {Achievement.PlayForOneHour, new AchievementInformation("Play for one hour total.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.PlayForFiveHours, new AchievementInformation("Play for five hours total.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.PlayForTenHours, new AchievementInformation("Play for ten hours total.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.PlayForTwentyFiveHours, new AchievementInformation("OPlay for twenty five hours total.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.PlayForFiftyHours, new AchievementInformation("Play for fifty hours total.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.PlayForOneHundredHours, new AchievementInformation("Play for one hundred hours total.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            
            {Achievement.OpenTheAppOnce, new AchievementInformation("Play your first game.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OpenTheAppFiveTimes, new AchievementInformation("Open the game five times.", 50, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OpenTheAppTenTimes, new AchievementInformation("Open the game ten times.", 100, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OpenTheAppTwentyFiveTimes, new AchievementInformation("Open the game twenty five times.", 250, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OpenTheAppFiftyTimes, new AchievementInformation("Open the game fifty times.", 500, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OpenTheAppOneHundredTimes, new AchievementInformation("Open the game one hundred times.", 1000, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            
            {Achievement.ReachLevelTwo, new AchievementInformation("Reach level one.", 100, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.ReachLevelFive, new AchievementInformation("Reach level five.", 5000, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.ReachLevelTen, new AchievementInformation("Reach level ten.", 10000, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.ReachLevelTwentyFive, new AchievementInformation("Reach level twenty five.", 250000, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.ReachLevelFifty, new AchievementInformation("Reach level fifty.", 900000, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.ReachLevelOneHundred, new AchievementInformation("Reach level one hundred.", 900000, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},

            {Achievement.OneHundredCredits, new AchievementInformation("Acquire one hundred credits.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.TwoHundredAndFiftyCredits, new AchievementInformation("Acquire two hundred and fifty credits.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.FiveHundredCredits, new AchievementInformation("Acquire five hundred credits.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OneThousandCredits, new AchievementInformation("Acquire one one thousand credits.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.TenThousandCredits, new AchievementInformation("Acquire one ten thousand credits.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OneHundredThousandCredits, new AchievementInformation("Acquire one hundred thousand credits.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            
            {Achievement.OneHundredPremiumCredits, new AchievementInformation("Acquire one hundred premium credits.", 5, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.TwoHundredAndFiftyPremiumCredits, new AchievementInformation("Acquire two hundred and fifty premium credits.", 5, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.FiveHundredPremiumCredits, new AchievementInformation("Acquire five hundred premium credits.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OneThousandPremiumCredits, new AchievementInformation("Acquire one thousand premium credits.", 100, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.TenThousandPremiumCredits, new AchievementInformation("Acquire ten thousand premium credits.", 1000, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OneHundredThousandPremiumCredits, new AchievementInformation("Acquire one hundred thousand premium credits.", 10000, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
        };

        public void Initialise()
        {
            Achievements = permanentAchievements;
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
        }

        private void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            var i = 0;
            foreach (var achievement in permanentAchievements)
            {
                if (saveData.permanentAchievements[i])
                {
                    achievement.Value.Unlock(false);
                }
                i++;
            }
        }

        public void UnlockAchievement(Achievement achievement) //TODO: Protect me from being called by anything nasty
        {
            if (!Enabled) return;
            
            if (GetUnlockedState(achievement)) return;
            permanentAchievements[achievement].Unlock();
        }
    }
}
