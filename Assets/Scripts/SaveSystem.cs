using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Achievements;
using Achievements.Dynamic;
using Achievements.Permanent;
using Audio;
using Credits;
using Player;
using Statistics;
using Statistics.Experience;
using UnityEngine;

/// <summary>
/// This class handles saving and loading locally from the device.
/// It saves custom binary files to the application support folder
/// </summary>
public static class SaveSystem
{
    public static Action<SaveData> OnSaveDataLoaded;
    private static readonly BinaryFormatter Formatter = new BinaryFormatter();
    private static readonly string Location = Application.persistentDataPath;
    private static readonly string FilePath = Location + "/SaveData.";

    public static void Save()
    {
        SaveClass<SaveData>(FilePath);
    }

    public static void Load()
    {
        OnSaveDataLoaded?.Invoke(LoadClass<SaveData>(FilePath));
    }

    public static void Delete()
    {
        var directory = new DirectoryInfo(Location).Parent;
        directory?.Delete(true);
    }

    private static void SaveClass<T>(string path) where T : new()
    {
        var stream = new FileStream(path, FileMode.Create);
        var saveData = new T();
        
        Formatter.Serialize(stream, saveData);
        stream.Close();
    }
    
    private static T LoadClass<T>(string path) where T : class
    {
        if (!File.Exists(path)) return null;
        
        var stream = new FileStream(path, FileMode.Open);

        if (stream.Length == 0) return null;
        
        var saveData = Formatter.Deserialize(stream) as T;
        
        stream.Close();

        return saveData;
    }

    [Serializable]
    public class SaveData
    {
        public readonly long PlayerID;
        public readonly string PlayerName;
        public readonly int LevelID = 1;
        public readonly long TotalExperience;
        public readonly long Credits;
        public readonly long PremiumCredits;
        public readonly DateTime LastTimeAppWasOpen;
        public readonly int ConsecutiveDailyOpens;
        public readonly TimeSpan TotalPlayTime;
        public readonly DateTime FirstOpen;
        public readonly int AmountOfYearsSinceFirstOpen;
        public readonly int TimesGameHasBeenOpened;
        public readonly KeyValuePair<int, int> FurthestLevelIndex;
        public readonly int[] LevelRatings;
        public readonly float Volume;
        public readonly bool[] PermanentAchievements;
        public readonly bool[] DynamicAchievements;

        public SaveData()
        {
            PlayerID = PlayerInformation.PlayerID;
            PlayerName = PlayerInformation.PlayerName;
            
            LevelID = ExperienceManager.CurrentLevelID;
            TotalExperience = ExperienceManager.TotalExperience;
            
            Credits = CreditsManager.GetCredits(CreditsManager.Currency.Credits);
            PremiumCredits = CreditsManager.GetCredits(CreditsManager.Currency.PremiumCredits);
            
            FirstOpen = HolidayManager.FirstOpen;
            AmountOfYearsSinceFirstOpen = HolidayManager.AmountOfYearsSinceFirstOpen;
            
            TimesGameHasBeenOpened = PlayerEngagement.TimesGameHasBeenOpened;
            LastTimeAppWasOpen = DateTime.Now;
            ConsecutiveDailyOpens = PlayerEngagement.ConsecutiveDailyOpens;
            TotalPlayTime = PlayerEngagement.TotalPlayTime + TimeSpan.FromSeconds(Time.deltaTime);
            
            FurthestLevelIndex = GameStatistics.FurthestLevelIndex;
            LevelRatings = GameStatistics.LevelRatings;

            Volume = BaseAudioManager.CurrentVolume;

            PermanentAchievements = PermanentAchievementManager.ReturnUnLockStates();
            DynamicAchievements = DynamicAchievementManager.ReturnUnLockStates();
        }
    }
}