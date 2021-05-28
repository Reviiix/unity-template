using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Achievements
{
    public static class AchievementManager
    {
        //TODO: weekly achievements abstract this
        public static Action<Achievement> OnAchievementUnlocked;
        private const string AchievementGraphicsFolderAssetPath = "Assets/Graphics/Achievements/";
        private static readonly Dictionary<Achievement, AchievementInformation> Achievements = new Dictionary<Achievement, AchievementInformation>
        {
            {Achievement.NumeroUno, new AchievementInformation("Play your first game.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))},
            
            {Achievement.OpenTheAppFiveTimes, new AchievementInformation("Open the game five times.", 10, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png"))}
        };
        public static string ReturnDescription(Achievement achievement) => Achievements[achievement].Description;
        public static int ReturnReward(Achievement achievement) => Achievements[achievement].Reward;
        public static AssetReference ReturnGraphic(Achievement achievement) => Achievements[achievement].SpriteAssetReference;
        public static bool ReturnUnlockedState(Achievement achievement) => Achievements[achievement].Unlocked;
        
        public static void Initialise()
        {
            AchievementTracker.Initialise();
        }

        public static Achievement[] ReturnAllAchievements()
        {
            var returnVariable = new List<Achievement>();
            foreach (var achievement in Achievements)
            {
                returnVariable.Add(achievement.Key);
            }
            return returnVariable.ToArray();
        }

        public static void UnlockAchievement(Achievement achievement)
        {
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
            NumeroUno,
            
            PlayForOneHour,
            PlayForFiveHours, 
            PlayForTenHours, 
            PlayForTwentyFiveHours, 
            PlayForFiftyHours, 
            PLayForOneHundredHours,
            
            OpenTheAppFiveTimes,
            OpenTheAppTenTimes,
            OpenTheAppTwentyFiveTimes,
            OpenTheAppFiftyTimes,
            OpenTheAppOneHundredTimes,
            
            ReachLevelFive,
            ReachLevelTen,
            ReachLevelTwentyFive,
            ReachLevelFifty,
        }
    }
}
