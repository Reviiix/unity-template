using System;
using Credits;
using UnityEngine;

namespace Statistics.Experience
{
    /// <summary>
    /// This class tracks experience
    /// </summary>
    public static class ExperienceManager
    {
        public static int CurrentLevelID { get; private set; }
        public static long TotalExperience  { get; private set; }
        private static ExperienceData[] _levels;
        public static Action<long, long> OnExperienceChange;
        public static Action<int> OnLevelChange;

        public static void Initialise()
        {
            CreateLevels(RemoteConfigurationManager.OfflineConfiguration.AmountOfExperienceLevels);
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
            RemoteConfigurationManager.OnConfigurationChanged += OnConfigurationChanged;
        }

        private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            
            CurrentLevelID = saveData.levelID;
            TotalExperience = saveData.totalExperience;
        }
        
        private static void OnConfigurationChanged(RemoteConfigurationManager.Configuration configuration)
        {
            CreateLevels(RemoteConfigurationManager.CurrentConfiguration.AmountOfExperienceLevels);
        }

        private static void CreateLevels(int amount)
        {
            _levels = new CreateExperienceData().CreateLevels(amount);
        }
        
        public static void AddExperience(long experience)
        {
            var oldValue = TotalExperience;
            TotalExperience += experience;
            OnExperienceChange?.Invoke(oldValue, TotalExperience);
            if (TotalExperience >= GetCurrentExperienceRequirement)
            {
                LevelUp();
            }
            SaveSystem.Save();
        }

        private static int GetLevelIDFromExperience(long experience)
        {
            for (var i = 0; i < _levels.Length; i++)
            {
                if (experience >= GetAllExperienceToLevelUp(i)) continue;
                return _levels[i].ID;
            }
            const string errorMessage = "There is no level that accounts for that much experience. Returning 0, Please create more level data to fix this error.";
            Debug.LogError(errorMessage);
            return 0;
        }

        private static void LevelUp()
        {
            CurrentLevelID = GetLevelIDFromExperience(TotalExperience);
            OnLevelChange?.Invoke(CurrentLevelID);
        }

        public static long GetAllExperienceToLevelUp(int level)
        {
            long experienceOfPriorLevels = 0;
            for (var i = 0; i < level; i++)
            {
                experienceOfPriorLevels += _levels[i].Experience;
            }
            return experienceOfPriorLevels + ReturnExperienceForLevelID(level);
        }
        
        private static long GetCurrentExperienceRequirement => GetAllExperienceToLevelUp(CurrentLevelID);
        
        public static long ReturnExperienceForLevelID(int id) => _levels[id].Experience;
    }

    public class CreateExperienceData
    {
        private long experience = 100;
        private const float ExperienceCurve = 1.1f;
        private float ExperienceIncrement => experience * ExperienceCurve / 2;
        private long reward = 10;
        private const float RewardCurve = 1.1f;
        private float RewardIncrement => reward * RewardCurve / 2;

        public ExperienceData[] CreateLevels(int amountOfLevels)
        {
            var levels = new ExperienceData[amountOfLevels];
            for (var i = 0; i < amountOfLevels; i++)
            {
                levels[i] = new ExperienceData(i, experience, reward);
                IncrementReward();
            }
            return levels;
        }

        private void IncrementReward()
        {
            reward += (int)RewardIncrement;
            experience += (int)ExperienceIncrement;
        }
    }

    public readonly struct ExperienceData
    {
        public readonly int ID;
        public readonly long Experience;
        public readonly long Reward;

        public ExperienceData(int id, long experience, long reward)
        {
            ID = id;
            Experience = experience;
            Reward = reward;
        }
    }
}