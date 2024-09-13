using System;
using Newtonsoft.Json;
using Unity.RemoteConfig;
using Unity.Services.RemoteConfig;
using UnityEngine;

/// <summary>
/// This class handles to remote config changes and sets features accordingly.
/// </summary>
public static class RemoteConfigurationManager
{
    public static bool RemoteConfigSet { get; private set; }
    public static Action<Configuration> OnConfigurationChanged;
    private const string RemoteConfigKey = "Configuration";
    public static readonly Configuration OfflineConfiguration = new Configuration(true, "None", 50, new DynamicAchievements(-1, -1, -1, -1, -1, -1, -1, -1, -1, -1));
    public static Configuration CurrentConfiguration { get; private set; } = OfflineConfiguration;

    public static void Initialise()
    {
        RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigUpdated;
    }

    public static void UpdateConfiguration()
    {
        RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new ApplicationAttributes());
    }

    private static void OnRemoteConfigUpdated(ConfigResponse response)
    {
        var newConfigurationAsJson = RemoteConfigService.Instance.appConfig.GetJson(RemoteConfigKey);
        var newConfiguration = JsonUtility.FromJson<Configuration>(newConfigurationAsJson);

        CurrentConfiguration = newConfiguration;
        OnConfigurationChanged?.Invoke(newConfiguration);
        RemoteConfigSet = true;
        DebuggingAid.DebugLogManager.Log("Remote Config Updated:\n\n" + newConfigurationAsJson);
    }
    
    [JsonObject]
    public struct Configuration
    {
        [JsonProperty] public bool ShowAdvertisements;
        [JsonProperty] public string Holiday;
        [JsonProperty] public int AmountOfExperienceLevels;
        [JsonProperty] public DynamicAchievements DynamicAchievements;

        [JsonConstructor]
        public Configuration(bool showAdvertisements, string holiday, int amountOfExperienceLevels, DynamicAchievements dynamicAchievements)
        {
            ShowAdvertisements = showAdvertisements;
            Holiday = holiday;
            AmountOfExperienceLevels = amountOfExperienceLevels;
            DynamicAchievements = dynamicAchievements;
        }
    }

    [Serializable] 
    public struct DynamicAchievements
    {
        [JsonProperty] public int highScore;
        [JsonProperty] public int highScoreReward;
        
        [JsonProperty] public int consecutivePlayTimeInSeconds;
        [JsonProperty] public int consecutivePlayTimeInSecondsReward;
        
        [JsonProperty] public int totalPlayTimeInSeconds;
        [JsonProperty] public int totalPlayTimeInSecondsReward;
        
        [JsonProperty] public int experienceGained;
        [JsonProperty] public int experienceGainedReward;
        
        [JsonProperty] public int stagesComplete;
        [JsonProperty] public int stagesCompleteReward;
    
        [JsonConstructor]
        public DynamicAchievements(int newHighScore, int newHighScoreReward, int newConsecutivePlayTimeInSeconds, int newConsecutivePlayTimeInSecondsReward, int newTotalPlayTimeInSeconds, int newTotalPlayTimeInSecondsReward, int newExperienceGained, int newExperienceGainedReward, int newStagesComplete, int newStagesCompleteReward)
        {
            highScore = newHighScore;
            highScoreReward = newHighScoreReward;
            
            consecutivePlayTimeInSeconds = newConsecutivePlayTimeInSeconds;
            consecutivePlayTimeInSecondsReward = newConsecutivePlayTimeInSecondsReward;
            
            totalPlayTimeInSeconds = newTotalPlayTimeInSeconds;
            totalPlayTimeInSecondsReward = newTotalPlayTimeInSecondsReward;
            
            experienceGained = newExperienceGained;
            experienceGainedReward = newExperienceGainedReward;
            
            stagesComplete = newStagesComplete;
            stagesCompleteReward = newStagesCompleteReward;
        }
    }

    private struct UserAttributes { }
    private struct ApplicationAttributes { }
}