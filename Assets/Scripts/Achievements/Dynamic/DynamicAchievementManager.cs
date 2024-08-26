using System;
using System.Collections;
using System.Collections.Generic;
using Credits;
using PureFunctions;
using PureFunctions.UnitySpecific;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Achievements.Dynamic
{
    /// <summary>
    /// This class manages the dynamic achievements which are intended to change regularly based on the updated remote config
    /// </summary>
    public class DynamicAchievementManager
    {
        private const bool Enabled = ProjectManager.EnabledFeatures.Achievements;
        private const string AchievementGraphicsFolderAssetPath = "Graphics/Achievements/";
        public Action<Achievement> OnAchievementUnlocked;
        private readonly Dictionary<Achievement, AchievementInformation> Achievements = new Dictionary<Achievement, AchievementInformation>();
        public bool GetUnlockedState(Achievement achievement) => Achievements[achievement].Unlocked;
        public string GetDescription(Achievement achievement) => Achievements[achievement].Description;
        public int GetReward(Achievement achievement) => Achievements[achievement].Reward;
        public AssetReference GetSpriteAssetReference(Achievement achievement) => Achievements[achievement].SpriteAssetReference;
        public Achievement[] GetAchievements()
        {
            var returnVariable = new List<Achievement>();
            foreach (var achievement in Achievements)
            {
                returnVariable.Add(achievement.Key);
            }
            return returnVariable.ToArray();
        }
        
        public  int GetTotalRewards(bool unlockedOnly = false)
        {
            var returnVariable = 0;
            foreach (var achievement in Achievements)
            {
                if (unlockedOnly)
                {
                    if (achievement.Value.Unlocked)
                    {
                        returnVariable += GetReward(achievement.Key);
                    }
                }
                else
                {
                    returnVariable += GetReward(achievement.Key);
                }
            }
            return returnVariable;
        }
            
        public  int ReturnAmountOfAchievements(bool unlockedOnly = false)
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
        
        public  bool[] GetUnLockStates()
        {
            var returnVariable = new List<bool>();
            foreach (var achievement in Achievements)
            {
                returnVariable.Add(achievement.Value.Unlocked);
            }
            return returnVariable.ToArray();
        }

        public void Initialise()
        {
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
            RemoteConfigurationManager.OnConfigurationChanged += OnConfigurationChanged;
        }

        private void OnConfigurationChanged(RemoteConfigurationManager.Configuration configuration)
        {
            CreateAchievements(configuration.DynamicAchievements);
        }
        
        private void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            
            Coroutiner.StartCoroutine(Wait.WaitForRemoteConfigToUpdate(() =>
            {
                var i = 0;
                foreach (var achievement in Achievements)
                {
                    if (saveData.dynamicAchievements[i])
                    {
                        achievement.Value.Unlock(false);
                    }
                    i++;
                }
            }));
        }

        private void CreateAchievements(RemoteConfigurationManager.DynamicAchievements dynamicAchievements)
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
            AchievementManager.DynamicAchievementsSet(GetAchievements());
        }
        
        public void UnlockAchievement(Achievement achievement) //TODO: Protect me from being called by anything
        {
            if (!Enabled) return;
            
            if (!Achievements.ContainsKey(achievement)) return;
            if (GetUnlockedState(achievement)) return;
            Achievements[achievement].Unlock();
            OnAchievementUnlocked?.Invoke(achievement);
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
                    CreditsManager.AddCredits(CreditsManager.Currency.PremiumCredits, Reward);
                }
            }
        }
    }
}
