using System;
using System.Collections.Generic;
using Credits;
using UnityEngine.AddressableAssets;

namespace Achievements
{
    /// <summary>
    /// This class manages the permanent achievements.
    /// </summary>
    public static class PermanentAchievementManager
    {
        public static Action<Achievement> OnAchievementUnlocked;
        private const string AchievementGraphicsFolderAssetPath = "Graphics/Achievements/";
        private static readonly Dictionary<Achievement, AchievementInformation> Achievements = new Dictionary<Achievement, AchievementInformation>
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
        public static string ReturnDescription(Achievement achievement) => Achievements[achievement].Description;
        public static int ReturnReward(Achievement achievement) => Achievements[achievement].Reward;
        public static AssetReference ReturnSpriteAssetReference(Achievement achievement) => Achievements[achievement].SpriteAssetReference;
        public static bool ReturnUnlockedState(Achievement achievement) => Achievements[achievement].Unlocked;

        public static Achievement[] ReturnAllAchievements()
        {
            var returnVariable = new List<Achievement>();
            foreach (var achievement in Achievements)
            {
                returnVariable.Add(achievement.Key);
            }
            return returnVariable.ToArray();
        }

        public static int ReturnTotalRewards(bool unlockedOnly = false)
        {
            var returnVariable = 0;
            foreach (var achievement in Achievements)
            {
                if (unlockedOnly)
                {
                    if (achievement.Value.Unlocked)
                    {
                        returnVariable += ReturnReward(achievement.Key);
                    }
                }
                else
                {
                    returnVariable += ReturnReward(achievement.Key);
                }
            }
            return returnVariable;
        }
        
        public static int ReturnAmountOfAchievements(bool unlockedOnly = false)
        {
            var returnVariable = 0;
            foreach (var achievement in Achievements)
            {
                if (unlockedOnly)
                {
                    if (achievement.Value.Unlocked)
                    {
                        returnVariable++;
                    }
                }
                else
                {
                    returnVariable++;
                }
            }
            return returnVariable;
        }
        
        public static bool[] ReturnUnLockStates()
        {
            var returnVariable = new List<bool>();
            foreach (var achievement in Achievements)
            {
                returnVariable.Add(achievement.Value.Unlocked);
            }
            return returnVariable.ToArray();
        }

        public static void Initialise()
        {
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
        }

        private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            var i = 0;
            foreach (var achievement in Achievements)
            {
                if (saveData.PermanentAchievements[i])
                {
                    achievement.Value.Unlock(false);
                }
                i++;
            }
        }

        public static void UnlockAchievement(Achievement achievement) //TODO: Protect me from being called by anything
        {
            if (ReturnUnlockedState(achievement)) return;
            Achievements[achievement].Unlock();
            OnAchievementUnlocked?.Invoke(achievement);
        }

        private class AchievementInformation
        {
            public readonly string Description;
            public readonly int Reward;
            public readonly AssetReference SpriteAssetReference;
            public bool Unlocked { get; private set; }

            public AchievementInformation(string description, int reward, AssetReference sprite, bool unlocked = false)
            {
                Description = description;
                Reward = reward;
                SpriteAssetReference = sprite;
                Unlocked = unlocked;
            }

            public void Unlock(bool addCredits = true)
            {
                Unlocked = true;
                if (addCredits)
                {
                    CreditsManager.IncrementCredits(CreditsManager.Currency.PremiumCredits, Reward);
                }
            }
        }

        public enum Achievement
        {
            PlayForOneHourConsecutively,
            PlayForOneHour,
            PlayForFiveHours, 
            PlayForTenHours, 
            PlayForTwentyFiveHours, 
            PlayForFiftyHours, 
            PlayForOneHundredHours,
            
            OpenTheAppOnce,
            OpenTheAppFiveTimes,
            OpenTheAppTenTimes,
            OpenTheAppTwentyFiveTimes,
            OpenTheAppFiftyTimes,
            OpenTheAppOneHundredTimes,
            
            ReachLevelTwo,
            ReachLevelFive,
            ReachLevelTen,
            ReachLevelTwentyFive,
            ReachLevelFifty,
            ReachLevelOneHundred,
            
            OneHundredCredits,
            TwoHundredAndFiftyCredits,
            FiveHundredCredits,
            OneThousandCredits,
            TenThousandCredits,
            OneHundredThousandCredits,
            
            OneHundredPremiumCredits,
            TwoHundredAndFiftyPremiumCredits,
            FiveHundredPremiumCredits,
            OneThousandPremiumCredits,
            TenThousandPremiumCredits,
            OneHundredThousandPremiumCredits,
        }
    }
}
