using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Achievements;
using Achievements.Dynamic;
using Achievements.Permanent;
using Audio;
using Credits;
using DebuggingAid;
using Newtonsoft.Json;
using Player;
using Statistics;
using Statistics.Experience;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This class handles saving and loading locally from the device.
/// It saves using either the Binary Formatter or JSON Utilities.
/// Save data is stored in to the Application.persistentDataPath folder (Usually the application support folder)
/// Custom binary files arte more secure but can be unreliable and difficult to work with.
/// JSON is easy to work with but is human readable and can be edited by someone with a decent amount of computing experience.
/// </summary>
public static class SaveSystem
{
    private const bool SaveWithBinaryFormatter = ProjectManager.EnabledFeatures.SaveWithBinaryFormatter; //Alternative is JSON.
    public static Action<SaveData> OnSaveDataLoaded;
    private static readonly BinaryFormatter Formatter = new();
    private static readonly string Location = Application.persistentDataPath;
    private static readonly string FilePath = Location + "/SaveData.";

    public static void Save()
    {
        if (SaveWithBinaryFormatter)
        {
            SaveClassWithBinaryFormatter<SaveData>(FilePath);
        }
        else
        {
            SaveClassWithJSON<SaveData>(FilePath);
        }
    }

    public static void Load()
    {
        OnSaveDataLoaded?.Invoke(SaveWithBinaryFormatter ? LoadClassFromBinaryFormatter<SaveData>(FilePath) : LoadClassFromJSON<SaveData>(FilePath));
    }
    
    public static void Delete()
    {
        try
        {
            new DirectoryInfo(Location).Parent?.Delete(true);
        }
        catch (Exception e)
        {
            const string errorMessage = "Failed To delete save data. Exception:";
            Debug.LogError(errorMessage + e);
        }
    }
    
    #region Binary Save System
    private static void SaveClassWithBinaryFormatter<T>(string path) where T : new()
    {
        try
        {
            using var stream = new FileStream(path, FileMode.Create);
            
            if (stream.Length == 0) return;
            
            var saveData = new T();
            Formatter.Serialize(stream, saveData);
            stream?.Close();
        }
        catch (Exception e)
        {
            const string errorMessage = "Failed To save data. Exception:";
            Debug.LogError(errorMessage + e);
        }
    }
    
    private static T LoadClassFromBinaryFormatter<T>(string path) where T : class
    {
        if (!File.Exists(path)) return null;
        
        using var stream = new FileStream(path, FileMode.Open);

        if (stream.Length == 0) return null;
        
        var saveData = Formatter.Deserialize(stream) as T;
        
        stream?.Close();

        return saveData;
    }
    #endregion Binary Save System

    #region JSON Save System
    private static void SaveClassWithJSON<T>(string path) where T : new()
    {
        try
        {
            var v = JsonUtility.ToJson(new T(), true);
            File.WriteAllText(path, v);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    private static T LoadClassFromJSON<T>(string path) where T : class
    {
        try
        {
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }
    #endregion JSON Save System

    [Serializable]
    public class SaveData
    {
        [FormerlySerializedAs("PlayerID")] public long playerID;
        [FormerlySerializedAs("PlayerName")] public string playerName;
        [FormerlySerializedAs("LevelID")] public int levelID = 1;
        [FormerlySerializedAs("TotalExperience")] public long totalExperience;
        [FormerlySerializedAs("Credits")] public long credits;
        [FormerlySerializedAs("PremiumCredits")] public long premiumCredits;
        public DateTime LastTimeAppWasOpen;
        [FormerlySerializedAs("ConsecutiveDailyOpens")] public int consecutiveDailyOpens;
        public TimeSpan TotalPlayTime;
        public DateTime FirstOpen;
        [FormerlySerializedAs("AmountOfYearsSinceFirstOpen")] public int amountOfYearsSinceFirstOpen;
        [FormerlySerializedAs("TimesGameHasBeenOpened")] public int timesGameHasBeenOpened;
        public KeyValuePair<int, int> FurthestLevelIndex;
        [FormerlySerializedAs("LevelRatings")] public int[] levelRatings;
        [FormerlySerializedAs("Volume")] public float volume;
        [FormerlySerializedAs("PermanentAchievements")] public bool[] permanentAchievements;
        [FormerlySerializedAs("DynamicAchievements")] public bool[] dynamicAchievements;
        
        public SaveData()
        {
            playerID = PlayerInformation.PlayerID;
            playerName = PlayerInformation.PlayerName;
            
            levelID = ExperienceManager.CurrentLevelID;
            totalExperience = ExperienceManager.TotalExperience;
            
            credits = CreditsManager.GetCredits(CreditsManager.Currency.Credits);
            premiumCredits = CreditsManager.GetCredits(CreditsManager.Currency.PremiumCredits);
            
            FirstOpen = HolidayManager.FirstOpen;
            amountOfYearsSinceFirstOpen = HolidayManager.AmountOfYearsSinceFirstOpen;
            
            timesGameHasBeenOpened = PlayerEngagement.TimesGameHasBeenOpened;
            LastTimeAppWasOpen = DateTime.Now;
            consecutiveDailyOpens = PlayerEngagement.ConsecutiveDailyOpens;
            TotalPlayTime = PlayerEngagement.TotalPlayTime + TimeSpan.FromSeconds(Time.deltaTime);
            
            FurthestLevelIndex = GameStatistics.FurthestLevelIndex;
            levelRatings = GameStatistics.LevelRatings;

            volume = BaseAudioManager.CurrentVolume;

            permanentAchievements = PermanentAchievementManager.ReturnUnLockStates();
            dynamicAchievements = DynamicAchievementManager.ReturnUnLockStates();
        }
    }
}