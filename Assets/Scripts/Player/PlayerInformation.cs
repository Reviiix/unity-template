using System;
using System.Collections.Generic;
using Credits;
using DebuggingAid.Cheats;
using Statistics.Experience;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [Serializable]
    public static class PlayerInformation
    {
        public static int PlayerID { get; private set; }
        public static int Level { get; private set; } = 1;
        public static long TotalExperience { get; private set; }
        public static KeyValuePair<int, int> FurthestStageIndex { get; private set; } = new KeyValuePair<int, int>();
        public static DateTime Birthday { get; private set; } = new DateTime(1993, 2, 3);
        public static long Credits { get; private set; } 
        public static long PremiumCredits { get; private set; }
        public static DateTime LastTimeAppWasOpen { get; private set; } 
        public static int ConsecutiveDailyOpens { get; private set; }
        public static TimeSpan TotalPlayTime{ get; set; } 
        
        public static void Initialise()
        {
            SaveSystem.OnSaveDataLoaded += LoadPlayerInformation;
            ExperienceManager.OnExperienceChange += OnExperienceChange;
            ExperienceManager.OnLevelChange += OnLevelChange;

            CreditsManager.OnCreditsChanged += OnCreditChange;
            CreditsManager.OnPremiumCreditsChanged += OnPremiumCreditChange;

            PlayerEngagementManager.OnDailyBonusOpen += OnDailyBonusOpen;
        }
        
        private static void LoadPlayerInformation(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            
            PlayerID = saveData.PlayerID;
            TotalExperience = saveData.TotalExperience;
            Level = saveData.Level;
            Credits = saveData.Credits;
            PremiumCredits = saveData.PremiumCredits;
            LastTimeAppWasOpen = saveData.LastTimeAppWasOpen;
            ConsecutiveDailyOpens = saveData.ConsecutiveDailyOpens;
            TotalPlayTime = saveData.TotalPlayTime;
        }

        private static void OnExperienceChange(long oldValue, long newValue)
        {
            TotalExperience = newValue;
        }

        private static void OnLevelChange(int levelID)
        {
            Level = levelID + 1;
        }
        
        private static void OnCreditChange(long originalValue, long newValue)
        {
            Credits = newValue;
        }
        
        private static void OnPremiumCreditChange(long originalValue, long newValue)
        {
            PremiumCredits = newValue;
        }

        private static void OnDailyBonusOpen(int consecutiveDailyOpens)
        {
            ConsecutiveDailyOpens = consecutiveDailyOpens;
        }
    }
}