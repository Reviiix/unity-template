using System;
using UnityEngine;

namespace Statistics.Experience
{
    public static class ExperienceManager
    {
        public static int CurrentLevelID { get; private set; }
        public static long TotalExperience  { get; private set; }
        private static ExperienceData[] _levels;
        public static Action<long, long> OnExperienceChange;
        public static Action<int> OnLevelChange;

        public static void Initialise()
        {
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
            RemoteConfigurationManager.OnConfigurationChanged += OnConfigurationChanged;
            CreateLevels(50);
        }

        private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            
            CurrentLevelID = saveData.LevelID;
            TotalExperience = saveData.TotalExperience;
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
            if (TotalExperience >= ReturnCurrentExperienceRequirement()) LevelUp();
            SaveSystem.Save();
        }

        private static int ReturnLevelIDFromExperience(long experience)
        {
            for (var i = 0; i < _levels.Length; i++)
            {
                if (experience >= ReturnAllExperienceToLevelUp(i)) continue;
                return _levels[i].ID;
            }
            Debug.LogError("There is no level that accounts for that much experience. Returning 0, Please create more level data to fix this error.");
            return 0;
        }

        private static void LevelUp()
        {
            CurrentLevelID = ReturnLevelIDFromExperience(TotalExperience);
            OnLevelChange(CurrentLevelID);
        }

        public static long ReturnAllExperienceToLevelUp(int level)
        {
            long experienceOfPriorLevels = 0;
            for (var i = 0; i < level; i++)
            {
                experienceOfPriorLevels += _levels[i].Experience;
            }
            return experienceOfPriorLevels + ReturnExperienceForLevelID(level);
        }
        
        private static long ReturnCurrentExperienceRequirement() => ReturnAllExperienceToLevelUp(CurrentLevelID);
        
        public static long ReturnExperienceForLevelID(int id) => _levels[id].Experience;
    }

    public class CreateExperienceData
    {
        private long _experience = 100;
        private const float ExperienceCurve = 1.1f;
        private long _reward = 10;
        private const float RewardCurve = 1.1f;
        private float ExperienceIncrement => _experience * ExperienceCurve / 2;
        private float RewardIncrement => _reward * RewardCurve / 2;

        public ExperienceData[] CreateLevels(int amountOfLevels)
        {
            var returnVariable = new ExperienceData[amountOfLevels];
            for (var i = 0; i < amountOfLevels; i++)
            {
                returnVariable[i] = new ExperienceData(i, ReturnExperience(), ReturnReward());
            }

            return returnVariable;
        }

        private long ReturnReward()
        {
            var returnVariable = _reward;
            _reward += (int)RewardIncrement;
            return returnVariable;
        }

        private long ReturnExperience()
        {
            var returnVariable = _experience;
            _experience += (int)ExperienceIncrement;
            return returnVariable;
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