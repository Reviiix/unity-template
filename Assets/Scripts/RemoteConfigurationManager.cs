using System;
using Newtonsoft.Json;
using Unity.RemoteConfig;
using UnityEngine;

public static class RemoteConfigurationManager
{
    public static Action<Configuration> OnConfigurationChanged;
    private const string RemoteConfigKey = "Configuration";
    private static readonly Configuration OfflineConfiguration = new Configuration(true, "None", 50, new Achievements(-1, -1, -1, -1, -1));
    public static Configuration CurrentConfiguration { get; private set; } = OfflineConfiguration;

    public static void Initialise()
    {
        ConfigManager.FetchCompleted += OnRemoteConfigUpdated;
        //Debug.Log(JsonUtility.ToJson(OfflineConfiguration));
    }

    public static void UpdateConfiguration()
    {
        ConfigManager.FetchConfigs(new UserAttributes(), new ApplicationAttributes());
    }

    private static void OnRemoteConfigUpdated(ConfigResponse response)
    {
        var newConfigurationAsJson = ConfigManager.appConfig.GetJson(RemoteConfigKey);
        var newConfiguration = JsonUtility.FromJson<Configuration>(newConfigurationAsJson);

        CurrentConfiguration = newConfiguration;

        OnConfigurationChanged?.Invoke(newConfiguration);

        DebuggingAid.Debugging.DisplayDebugMessage("Remote Config Updated: \n" + newConfigurationAsJson);
    }
    
    [JsonObject]
    public struct Configuration
    {
        [JsonProperty] public bool ShowAdvertisements;
        [JsonProperty] public string Holiday;
        [JsonProperty] public int AmountOfExperienceLevels;
        [JsonProperty] public Achievements Achievements;

        [JsonConstructor]
        public Configuration(bool showAdvertisements, string holiday, int amountOfExperienceLevels, Achievements achievements)
        {
            ShowAdvertisements = showAdvertisements;
            Holiday = holiday;
            AmountOfExperienceLevels = amountOfExperienceLevels;
            Achievements = achievements;
        }
    }

    [Serializable] 
    public struct Achievements
    {
        [JsonProperty] public int highsScore;
        [JsonProperty] public int consecutivePlayTimeInSeconds;
        [JsonProperty] public int totalPlayTimeInSeconds;
        [JsonProperty] public int experienceGained;
        [JsonProperty] public int levelsComplete;
    
        [JsonConstructor]
        public Achievements(int newHighScore, int newConsecutivePlayTimeInSeconds, int newTotalPlayTimeInSeconds, int newExperienceGained, int newLevelsComplete)
        {
            highsScore = newHighScore;
            consecutivePlayTimeInSeconds = newConsecutivePlayTimeInSeconds;
            totalPlayTimeInSeconds = newTotalPlayTimeInSeconds;
            experienceGained = newExperienceGained;
            levelsComplete = newLevelsComplete;
        }
    }

    private struct UserAttributes { }
    private struct ApplicationAttributes { }
}