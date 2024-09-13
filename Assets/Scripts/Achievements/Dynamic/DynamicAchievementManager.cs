using System.Collections.Generic;
using PureFunctions.UnitySpecific;
using UnityEngine.AddressableAssets;

namespace Achievements.Dynamic
{
    /// <summary>
    /// This class manages the dynamic achievements which are intended to change regularly based on the updated remote config
    /// </summary>
    public class DynamicAchievementManager : BaseAchievementManager
    {
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
        
        public void UnlockAchievement(Achievement achievement) //TODO: Protect me from being called by anything
        {
            if (!Enabled) return;

            if (!Achievements.ContainsKey(achievement)) return;
            if (GetUnlockedState(achievement)) return;
            Achievements[achievement].Unlock();
        }
        
                private void CreateAchievements(RemoteConfigurationManager.DynamicAchievements dynamicAchievements)
        {
            var highScore = dynamicAchievements.highScore;
            var consecutivePlayTimeInSeconds = dynamicAchievements.consecutivePlayTimeInSeconds;
            var totalPlayTimeInSeconds = dynamicAchievements.totalPlayTimeInSeconds;
            var experienceGained = dynamicAchievements.experienceGained;
            var stagesComplete = dynamicAchievements.stagesComplete;
            Achievements = new Dictionary<Achievement, AchievementInformation>();
            if (highScore != -1)
            {
                Achievements.Add(Achievement.HighScore,
                    new AchievementInformation("Achieve a high score of " + highScore,
                        dynamicAchievements.highScoreReward,
                        new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
            }

            if (consecutivePlayTimeInSeconds != -1)
            {
                Achievements.Add(Achievement.ConsecutivePlayTimeInSeconds,
                    new AchievementInformation("Play for " + consecutivePlayTimeInSeconds + " seconds consecutively.",
                        dynamicAchievements.consecutivePlayTimeInSecondsReward,
                        new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
            }

            if (totalPlayTimeInSeconds != -1)
            {
                Achievements.Add(Achievement.TotalPlayTimeInSeconds,
                    new AchievementInformation("Play for " + totalPlayTimeInSeconds + " seconds.",
                        dynamicAchievements.totalPlayTimeInSecondsReward,
                        new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
            }

            if (experienceGained != -1)
            {
                Achievements.Add(Achievement.ExperienceGained,
                    new AchievementInformation("Gain " + experienceGained + " experience.",
                        dynamicAchievements.experienceGainedReward,
                        new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
            }

            if (stagesComplete != -1)
            {
                Achievements.Add(Achievement.LevelsComplete,
                    new AchievementInformation("Complete " + stagesComplete + " levels.",
                        dynamicAchievements.stagesCompleteReward,
                        new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
            }

            AchievementManager.DynamicAchievementsSet(GetAchievements());
        }

    }
}
