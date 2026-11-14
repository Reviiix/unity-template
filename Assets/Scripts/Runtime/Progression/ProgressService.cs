using System;
using UnityTemplate.Core;

namespace UnityTemplate.Progression
{
    /// <summary>Owns stage ratings and the high score, and derives which stages are unlocked and which one CONTINUE loads.</summary>
    public class ProgressService : ISaveable
    {
        private readonly SaveService saveService;
        private readonly StageCatalogue catalogue;
        private int[] stageRatings;

        public StageCatalogue Catalogue => catalogue;
        public int HighScore { get; private set; }
        public bool HasProgress => Array.Exists(stageRatings, rating => rating > 0);

        /// <summary>Creates the service for the given stage catalogue.</summary>
        public ProgressService(SaveService saveService, StageCatalogue catalogue)
        {
            this.saveService = saveService;
            this.catalogue = catalogue;
            stageRatings = new int[catalogue.GetTotalStageCount()];
        }

        /// <summary>Returns true for the first stage and for every stage whose previous stage has been rated.</summary>
        public bool IsUnlocked(int group, int stage)
        {
            var index = catalogue.GetStageIndex(group, stage);
            return index == 0 || stageRatings[index - 1] > 0;
        }

        /// <summary>Returns the stage CONTINUE should load: the first unlocked, unrated stage, or the last stage when all are rated.</summary>
        public (int Group, int Stage) GetContinueStage()
        {
            (int, int) lastStage = (0, 0);
            for (var group = 0; group < catalogue.GroupCount; group++)
            {
                for (var stage = 0; stage < catalogue.GetStageCount(group); stage++)
                {
                    lastStage = (group, stage);
                    if (stageRatings[catalogue.GetStageIndex(group, stage)] == 0) return lastStage;
                }
            }
            return lastStage;
        }

        /// <summary>Returns a stage's best rating (0 = not completed).</summary>
        public int GetRating(int group, int stage)
        {
            return stageRatings[catalogue.GetStageIndex(group, stage)];
        }

        /// <summary>Keeps the best rating for a stage and saves when it improves.</summary>
        public void RecordRating(int group, int stage, int rating)
        {
            var index = catalogue.GetStageIndex(group, stage);
            if (rating <= stageRatings[index]) return;

            stageRatings[index] = rating;
            saveService.Save();
        }

        /// <summary>Stores the score if it beats the high score and returns true when it did.</summary>
        public bool SubmitScore(int score)
        {
            if (score <= HighScore) return false;

            HighScore = score;
            saveService.Save();
            return true;
        }

        /// <summary>Restores ratings (resized to the current catalogue) and the high score.</summary>
        public void Load(SaveData data)
        {
            stageRatings = new int[catalogue.GetTotalStageCount()];
            Array.Copy(data.stageRatings, stageRatings, Math.Min(data.stageRatings.Length, stageRatings.Length));
            HighScore = data.highScore;
        }

        /// <summary>Writes ratings and the high score to the save file.</summary>
        public void Save(SaveData data)
        {
            data.stageRatings = (int[])stageRatings.Clone();
            data.highScore = HighScore;
        }
    }
}
