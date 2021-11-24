using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Achievements
{
    public static class PermanentAchievementManager
    {
        public static Action<Achievement> OnAchievementUnlocked;
        private const string AchievementGraphicsFolderAssetPath = "Graphics/Achievements/";
        private static readonly Dictionary<Achievement, AchievementInformation> Achievements = new Dictionary<Achievement, AchievementInformation>
        {
            {Achievement.PlayForOneHourConsecutively, new AchievementInformation("Play For One Hour Consecutively.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            
            {Achievement.PlayForOneHour, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.PlayForFiveHours, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.PlayForTenHours, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.PlayForTwentyFiveHours, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.PlayForFiftyHours, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.PLayForOneHundredHours, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            
            {Achievement.OpenTheAppOnce, new AchievementInformation("Play your first game.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OpenTheAppFiveTimes, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OpenTheAppTenTimes, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OpenTheAppTwentyFiveTimes, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OpenTheAppFiftyTimes, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OpenTheAppOneHundredTimes, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            
            {Achievement.ReachLevelTwo, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.ReachLevelFive, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.ReachLevelTen, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.ReachLevelTwentyFive, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.ReachLevelFifty, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.ReachLevelOneHundred, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},

            {Achievement.OneHundredCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.TwoHundredAndFiftyCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.FiveHundredCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OneThousandCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.TenThousandCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OneHundredThousandCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            
            {Achievement.OneHundredPremiumCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.TwoHundredAndFiftyPremiumCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.FiveHundredPremiumCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OneThousandPremiumCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.TenThousandPremiumCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            {Achievement.OneHundredThousandPremiumCredits, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
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
                if (saveData.Achievements[i])
                {
                    achievement.Value.Unlock();
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

            public void Unlock()
            {
                Unlocked = true;
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
            PLayForOneHundredHours,
            
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
