using System;
using System.Collections;
using System.Collections.Generic;
using Credits;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Achievements.Dynamic
{
    /// <summary>
    /// This class manages the dynamic achievements which are intended to change regularly based on the updated remote config
    /// </summary>
    public static class DynamicAchievementManager
    {
        private const string AchievementGraphicsFolderAssetPath = "Graphics/Achievements/";
        private static bool _achievementsSet;
        public static Action<Achievement[]> OnAchievementsSet;
        public static Action<Achievement> OnAchievementUnlocked;
        private static readonly Dictionary<Achievement, AchievementInformation> Achievements = new Dictionary<Achievement, AchievementInformation>();
        public static bool ReturnUnlockedState(Achievement achievement) => Achievements[achievement].Unlocked;
        public static string ReturnDescription(Achievement achievement) => Achievements[achievement].Description;
        public static int ReturnReward(Achievement achievement) => Achievements[achievement].Reward;
        public static AssetReference ReturnSpriteAssetReference(Achievement achievement) => Achievements[achievement].SpriteAssetReference;
        private static Achievement[] ReturnAllAchievements()
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
            RemoteConfigurationManager.OnConfigurationChanged += OnConfigurationChanged;
        }

        private static void OnConfigurationChanged(RemoteConfigurationManager.Configuration configuration)
        {
            CreateAchievements(configuration.DynamicAchievements);
        }
        
        private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            
            ProjectManager.Instance.StartCoroutine(WaitForRemoteConfig(() =>
            {
                var i = 0;
                foreach (var achievement in Achievements)
                {
                    if (saveData.DynamicAchievements[i])
                    {
                        achievement.Value.Unlock(false);
                    }
                    i++;
                }
            }));
        }

        private static void CreateAchievements(RemoteConfigurationManager.DynamicAchievements dynamicAchievements)
        {
            var highScore = dynamicAchievements.highScore;
            var consecutivePlayTimeInSeconds = dynamicAchievements.consecutivePlayTimeInSeconds;
            var totalPlayTimeInSeconds = dynamicAchievements.totalPlayTimeInSeconds;
            var experienceGained = dynamicAchievements.experienceGained;
            var stagesComplete = dynamicAchievements.stagesComplete;
            if (highScore != -1)
            {
                Achievements.Add(Achievement.HighScore, new AchievementInformation("Achieve a high score of " + highScore, dynamicAchievements.highScoreReward, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
            }
            if (consecutivePlayTimeInSeconds != -1)
            {
                Achievements.Add(Achievement.ConsecutivePlayTimeInSeconds, new AchievementInformation("Play for " + consecutivePlayTimeInSeconds + " seconds consecutively.", dynamicAchievements.consecutivePlayTimeInSecondsReward, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
            }
            if (totalPlayTimeInSeconds != -1)
            {
                Achievements.Add(Achievement.TotalPlayTimeInSeconds, new AchievementInformation("Play for " + totalPlayTimeInSeconds + " seconds.", dynamicAchievements.totalPlayTimeInSecondsReward, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
            }
            if (experienceGained != -1)
            {
                Achievements.Add(Achievement.ExperienceGained, new AchievementInformation("Gain " + experienceGained + " experience.", dynamicAchievements.experienceGainedReward, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
            }
            if (stagesComplete != -1)
            {
                Achievements.Add(Achievement.LevelsComplete, new AchievementInformation("Complete " + stagesComplete + " levels.", dynamicAchievements.stagesCompleteReward, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
            }
            _achievementsSet = true;
            OnAchievementsSet?.Invoke(ReturnAllAchievements());
        }
        
        public static void UnlockAchievement(Achievement achievement) //TODO: Protect me from being called by anything
        {
            if (!Achievements.ContainsKey(achievement)) return;
            if (ReturnUnlockedState(achievement)) return;
            Achievements[achievement].Unlock();
            OnAchievementUnlocked?.Invoke(achievement);
        }
        
        private static IEnumerator WaitForRemoteConfig(Action callBack)
        {
            yield return new WaitUntil(() => _achievementsSet);
            callBack();
        }

        public enum Achievement
        {
            HighScore,
            ConsecutivePlayTimeInSeconds,
            TotalPlayTimeInSeconds,
            ExperienceGained,
            LevelsComplete,
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
    }
}
