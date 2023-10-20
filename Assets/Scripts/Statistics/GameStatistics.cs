using System.Collections.Generic;

namespace Statistics
{
    /// <summary>
    /// This class manages the game statistics
    /// </summary>
    public static class GameStatistics
    {
        public static KeyValuePair<int, int> FurthestLevelIndex { get; private set; }
        public static readonly int[] LevelRatings = new int[StageLoadManager.TotalAmountOfStages];
        public static int ReturnRating(int levelGroup, int level) => LevelRatings[StageLoadManager.GetStageIndex(levelGroup, level)];
        
        public static void Initialise()
        {
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
        }

        private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            
            var amountOfLevels = saveData.levelRatings.Length;
            FurthestLevelIndex = saveData.FurthestLevelIndex;
            for (var i = 0; i < amountOfLevels; i++)
            {
                var rating = saveData.levelRatings[i];
                if (rating > 0) IncrementMostRecentUnlockedLevel();
                LevelRatings[i] = rating;
            }
        }

        private static void IncrementMostRecentUnlockedLevel()
        {
            var currentWorld = FurthestLevelIndex.Key;
            var maxLevelInCurrentWorld = StageLoadManager.GetAmountOfStagesIn(currentWorld);

            if (FurthestLevelIndex.Value < maxLevelInCurrentWorld)
            {
                var currentLevel = FurthestLevelIndex.Value + 1;
                FurthestLevelIndex = new KeyValuePair<int, int>(currentWorld, currentLevel);
                return;
            }
            FurthestLevelIndex = new KeyValuePair<int, int>(currentWorld+1, 0);
        }

        public static int ReturnMostRecentUnlockedLevel()
        {
            var returnVariable = 0;
            var furthestLevelKeyValuePair = FurthestLevelIndex;
            var totalWorlds = furthestLevelKeyValuePair.Key;
            for (var i = 0; i < totalWorlds; i++)
            {
                if (i > furthestLevelKeyValuePair.Key) break;
                var amountOfLevels = StageLoadManager.GetAmountOfStagesIn(i);
                for (var j = 0; j < amountOfLevels; j++)
                {
                    if (totalWorlds == i && furthestLevelKeyValuePair.Value < j) returnVariable++;
                }
            }
            return returnVariable;
        }
    }
}
