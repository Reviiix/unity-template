using System;
using Newtonsoft.Json;
using Unity.RemoteConfig;
using UnityEngine;

public static class RemoteConfigurationManager
{
    public static Action<Configuration> OnConfigurationChanged;
    private const string RemoteConfigKey = "Configuration";
    private static readonly Configuration OfflineConfiguration = new Configuration(true, "None", 50, new WeeklyQuests(1, 2, 3));
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
        [JsonProperty] public WeeklyQuests WeeklyQuests;

        [JsonConstructor]
        public Configuration(bool showAdvertisements, string holiday, int amountOfExperienceLevels, WeeklyQuests weeklyQuests)
        {
            ShowAdvertisements = showAdvertisements;
            Holiday = holiday;
            AmountOfExperienceLevels = amountOfExperienceLevels;
            WeeklyQuests = weeklyQuests;
        }
    }

    [Serializable] 
    public struct WeeklyQuests
    {
        [JsonProperty] public int highsSore;
        [JsonProperty] public int playTimeInSeconds;
        [JsonProperty] public int levelsComplete;
    
        [JsonConstructor]
        public WeeklyQuests(int newHighScore, int newPlayTimeInSeconds, int newLevelsComplete)
        {
            highsSore = newHighScore;
            playTimeInSeconds = newPlayTimeInSeconds;
            levelsComplete = newLevelsComplete;
        }
    }

    private struct UserAttributes { }
    private struct ApplicationAttributes { }
}