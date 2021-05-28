using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Audio;
using Player;
using Statistics;
using UnityEngine;

public static class SaveSystem
{
    public static Action<SaveData> OnSaveDataLoaded;
    private static readonly BinaryFormatter Formatter = new BinaryFormatter();
    private static readonly string Location = Application.persistentDataPath;
    private static readonly string FilePath = Location + "SaveData.";

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
        #region PlayerInformation
        public readonly int PlayerID;
        public readonly int Level = 1;
        public readonly long TotalExperience;
        public readonly long Credits;
        public readonly long PremiumCredits;
        public readonly DateTime LastTimeAppWasOpen;
        public readonly int ConsecutiveDailyOpens;
        public readonly TimeSpan TotalPlayTime;
        #endregion PlayerInformation

        #region ProjectInformation
        public readonly int TimesGameHasBeenOpenedSave;
        #endregion ProjectInformation

        #region GameStatistics
        public readonly KeyValuePair<int, int> FurthestLevelIndex;
        public readonly int[] LevelRatings;
        #endregion GameStatistics

        public readonly float Volume;

        public SaveData()
        {
            #region PlayerInformation
            PlayerID = PlayerInformation.PlayerID;
            Level = PlayerInformation.Level;
            TotalExperience = PlayerInformation.TotalExperience;
            Credits = PlayerInformation.Credits;
            PremiumCredits = PlayerInformation.PremiumCredits;
            LastTimeAppWasOpen = DateTime.Now;
            ConsecutiveDailyOpens = PlayerInformation.ConsecutiveDailyOpens;
            TotalPlayTime = PlayerInformation.TotalPlayTime += TimeSpan.FromSeconds(Time.deltaTime);
            DebuggingAid.Debugging.DisplayDebugMessage("Current Session Time:: " + Time.deltaTime + ", Total Play Time: " + TotalPlayTime);
            #endregion PlayerInformation
            
            #region ProjectInformation
            TimesGameHasBeenOpenedSave = PlayerEngagementManager.TimesGameHasBeenOpened;
            #endregion ProjectInformation
            
            #region GameStatistics
            FurthestLevelIndex = GameStatistics.FurthestLevelIndex;
            LevelRatings = GameStatistics.LevelRatings;
            #endregion GameStatistics

            Volume = BaseAudioManager.CurrentVolume;
        }
    }
}