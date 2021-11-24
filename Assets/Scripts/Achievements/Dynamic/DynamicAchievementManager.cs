using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public static class DynamicAchievementManager
{
    private const string AchievementGraphicsFolderAssetPath = "Graphics/Achievements/";
    public static Action<Achievement[]> OnAchievementsSet;
    public static Action<Achievement> OnAchievementUnlocked;
    private static readonly Dictionary<Achievement, AchievementInformation> Achievements = new Dictionary<Achievement, AchievementInformation>();
    public static bool ReturnUnlockedState(Achievement achievement) => Achievements[achievement].Unlocked;
    public static string ReturnDescription(Achievement achievement) => Achievements[achievement].Description;
    public static int ReturnReward(Achievement achievement) => Achievements[achievement].Reward;
    public static AssetReference ReturnSpriteAssetReference(Achievement achievement) => Achievements[achievement].SpriteAssetReference;
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

    public static void Initialise()
    {
        SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
        RemoteConfigurationManager.OnConfigurationChanged += OnConfigurationChanged;
    }

    private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
    {
        
    }

    private static void OnConfigurationChanged(RemoteConfigurationManager.Configuration configuration)
    {
        CreateAchievements(configuration.Achievements);
    }

    private static void CreateAchievements(RemoteConfigurationManager.Achievements achievements)
    {
        var highScore = achievements.highsScore;
        var consecutivePlayTimeInSeconds = achievements.consecutivePlayTimeInSeconds;
        var totalPlayTimeInSeconds = achievements.totalPlayTimeInSeconds;
        var experienceGained = achievements.experienceGained;
        var levelsComplete = achievements.levelsComplete;
        //if (highScore != -1)
        {
            Achievements.Add(Achievement.HighScore, new AchievementInformation("Achieve a high score of " + highScore, 0, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
        }
        //if (consecutivePlayTimeInSeconds != -1)
        {
            Achievements.Add(Achievement.ConsecutivePlayTimeInSeconds, new AchievementInformation("Play for " + consecutivePlayTimeInSeconds + " seconds consecutively.", 0, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
        }
        //if (totalPlayTimeInSeconds != -1)
        {
            Achievements.Add(Achievement.TotalPlayTimeInSeconds, new AchievementInformation("Play for " + totalPlayTimeInSeconds + " seconds this week.", 0, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
        }
        //if (experienceGained != -1)
        {
            Achievements.Add(Achievement.ExperienceGained, new AchievementInformation("Gain " + experienceGained + " experience this week.", 0, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
        }
        //if (levelsComplete != -1)
        {
            Achievements.Add(Achievement.LevelsComplete, new AchievementInformation("Complete " + levelsComplete + "levels this week.", 0, new AssetReference(AchievementGraphicsFolderAssetPath + "placeholder.png")));
        }
        OnAchievementsSet?.Invoke(ReturnAllAchievements());
    }
    
    public static void UnlockAchievement(Achievement achievement) //TODO: Protect me from being called by anything
    {
        if (!Achievements.ContainsKey(achievement)) return;
        if (ReturnUnlockedState(achievement)) return;
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

        public void Unlock()
        {
            Unlocked = true;
        }
    }

}
