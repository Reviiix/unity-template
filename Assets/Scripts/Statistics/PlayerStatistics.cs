using System.Collections.Generic;

namespace Statistics
{
    /// <summary>
    /// This class manages the game statistics
    /// </summary>
    public static class PlayerStatistics
    {
        public static KeyValuePair<int, int> FurthestStage { get; private set; }
        public static readonly int[] LevelRatings = new int[StageLoadManager.TotalAmountOfStages];

        public static void Initialise()
        {
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
        }

        private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            
            var amountOfLevels = saveData.levelRatings.Length;
            FurthestStage = saveData.FurthestLevelIndex;
            for (var i = 0; i < amountOfLevels; i++)
            {
                var rating = saveData.levelRatings[i];
                if (rating > 0) IncrementMostRecentUnlockedLevel();
                LevelRatings[i] = rating;
            }
        }

        private static void IncrementMostRecentUnlockedLevel()
        {
            var currentWorld = FurthestStage.Key;
            var maxLevelInCurrentWorld = StageLoadManager.GetAmountOfStagesIn(currentWorld);

            if (FurthestStage.Value < maxLevelInCurrentWorld)
            {
                var currentLevel = FurthestStage.Value + 1;
                FurthestStage = new KeyValuePair<int, int>(currentWorld, currentLevel);
                return;
            }
            FurthestStage = new KeyValuePair<int, int>(currentWorld+1, 0);
        }

        public static int ReturnMostRecentUnlockedLevel()
        {
            var returnVariable = 0;
            var furthestLevelKeyValuePair = FurthestStage;
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
