using System.Collections.Generic;

namespace Statistics
{
    public static class GameStatistics
    {
        public static KeyValuePair<int, int> FurthestLevelIndex { get; private set; }
        public static readonly int[] LevelRatings = new int[StageLoadManager.ReturnTotalAmountOfStages];
        public static int ReturnRating(int levelGroup, int level) => LevelRatings[StageLoadManager.ReturnStageIndex(levelGroup, level)];
        
        public static void Initialise()
        {
            SaveSystem.OnSaveDataLoaded += LoadLevelRatings;
        }

        private static void LoadLevelRatings(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            
            var amountOfLevels = saveData.LevelRatings.Length;
            FurthestLevelIndex = saveData.FurthestLevelIndex;
            for (var i = 0; i < amountOfLevels; i++)
            {
                var rating = saveData.LevelRatings[i];
                if (rating > 0) IncrementMostRecentUnlockedLevel();
                LevelRatings[i] = rating;
            }
        }

        private static void IncrementMostRecentUnlockedLevel()
        {
            var currentWorld = FurthestLevelIndex.Key;
            var maxLevelInCurrentWorld = StageLoadManager.ReturnAmountOfStagesIn(currentWorld);

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
                var amountOfLevels = StageLoadManager.ReturnAmountOfStagesIn(i);
                for (var j = 0; j < amountOfLevels; j++)
                {
                    if (totalWorlds == i && furthestLevelKeyValuePair.Value < j) returnVariable++;
                }
            }
            return returnVariable;
        }
    }
}
