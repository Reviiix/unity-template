using System;
using UnityTemplate.Core;

namespace UnityTemplate.Progression
{
    /// <summary>Tracks total experience and derives the player's level from a level table precomputed at start-up.</summary>
    public class ExperienceService : ISaveable
    {
        private const long FirstLevelExperience = 100;
        private const float ExperienceCurve = 1.1f;
        private const long MaximumLevelExperience = long.MaxValue / 1024;
        private readonly SaveService saveService;
        private long[] experiencePerLevel = Array.Empty<long>();
        private long[] cumulativeExperience = Array.Empty<long>();

        public long TotalExperience { get; private set; }
        public int CurrentLevel { get; private set; }
        public int MaximumLevel => experiencePerLevel.Length - 1;

        public event Action<ValueChange> ExperienceChanged;
        public event Action<int> LevelChanged;

        /// <summary>Creates the service and precomputes the experience needed for every level once.</summary>
        public ExperienceService(SaveService saveService, int amountOfLevels)
        {
            this.saveService = saveService;
            BuildLevelTable(amountOfLevels);
        }

        /// <summary>Adds experience, levels up if a threshold is passed, notifies listeners and saves.</summary>
        public void Add(long amount)
        {
            if (amount <= 0) return;

            var oldExperience = TotalExperience;
            TotalExperience += amount;
            ExperienceChanged?.Invoke(new ValueChange(oldExperience, TotalExperience));

            var newLevel = GetLevelForExperience(TotalExperience);
            if (newLevel != CurrentLevel)
            {
                CurrentLevel = newLevel;
                LevelChanged?.Invoke(CurrentLevel);
            }
            saveService.Save();
        }

        /// <summary>Returns the total experience needed to complete a level, counting every level before it.</summary>
        public long GetCumulativeExperience(int level)
        {
            return cumulativeExperience[ClampLevel(level)];
        }

        /// <summary>Returns the experience needed within a single level.</summary>
        public long GetExperienceForLevel(int level)
        {
            return experiencePerLevel[ClampLevel(level)];
        }

        /// <summary>Restores total experience and derives the level from it.</summary>
        public void Load(SaveData data)
        {
            TotalExperience = Math.Max(0, data.totalExperience);
            CurrentLevel = GetLevelForExperience(TotalExperience);
        }

        /// <summary>Writes total experience to the save file.</summary>
        public void Save(SaveData data)
        {
            data.totalExperience = TotalExperience;
        }

        private int GetLevelForExperience(long experience)
        {
            for (var level = 0; level < cumulativeExperience.Length; level++)
            {
                if (experience < cumulativeExperience[level]) return level;
            }
            return MaximumLevel;
        }

        private int ClampLevel(int level)
        {
            return Math.Clamp(level, 0, MaximumLevel);
        }

        private void BuildLevelTable(int amountOfLevels)
        {
            amountOfLevels = Math.Max(1, amountOfLevels);
            experiencePerLevel = new long[amountOfLevels];
            cumulativeExperience = new long[amountOfLevels];

            var levelExperience = FirstLevelExperience;
            long runningTotal = 0;
            for (var level = 0; level < amountOfLevels; level++)
            {
                experiencePerLevel[level] = levelExperience;
                runningTotal = Math.Min(MaximumLevelExperience, runningTotal + levelExperience);
                cumulativeExperience[level] = runningTotal;
                levelExperience = Math.Min(MaximumLevelExperience, levelExperience + (long)(levelExperience * ExperienceCurve / 2));
            }
        }
    }
}
