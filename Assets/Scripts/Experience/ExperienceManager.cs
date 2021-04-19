using System;
using UnityEngine;

namespace Experience
{
    public static class ExperienceManager
    {
        private static int CurrentLevelID { get; set; }
        public static long CurrentLevelExperience{ get; private set; }

        private static ExperienceData[] _levels;
        
        public static Action<long> OnExperienceChange;
        public static Action<int> OnLevelChange;

        public static void Initialise()
        {
            CreateLevels();
            LoadExperience();
        }

        private static void LoadExperience()
        {
            var levelID = PlayerInformation.Level - 1;
            if (levelID < 0) levelID = 0;
            CurrentLevelID = levelID;
            CurrentLevelExperience = PlayerInformation.CurrentExperience;
        }

        private static void CreateLevels()
        {
            _levels = new CreateExperienceData().CreateLevels(100);
        }

        private static long ReturnExperienceOfAllPriorLevels()
        {
            long returnVariable = 0;
            for (var i = 0; i < CurrentLevelID-1; i++)
            {
                returnVariable += _levels[i].Experience;
            }
            return returnVariable;
        }

        private static long _remainingExperience;

        private static bool _additionInProgress;

        public static void AddExperience(long experience)
        {
            _additionInProgress = true;
            if (CurrentLevelExperience + experience > _levels[CurrentLevelID].Experience)
            {
                _remainingExperience = CurrentLevelExperience + experience - _levels[CurrentLevelID].Experience +1; //-1 from max level so the level bar fill amount is not reset to 0 on level up. *
                experience -= _remainingExperience;
            }
            CurrentLevelExperience += experience;
            OnExperienceChange?.Invoke(experience);
            if (CurrentLevelExperience >= _levels[CurrentLevelID].Experience) LevelUp();
            
            ProjectManager.Wait(0.1f, () => //Wait for the first animation to start (avoid race conditions)
            {
                ExperienceDisplay.WaitForAnimatingToStopWrapper(() => //What for the animation to end
                {
                    if (_remainingExperience > 0)
                    {
                        AddExperience(1); //Add back the 1 experience we took earlier to stop level bar fill amount from resetting to 0 on level up. *
                        _remainingExperience -= 1;
                        ExperienceDisplay.WaitForAnimatingToStopWrapper(() => //What for the animation to end
                        {
                            AddExperience(_remainingExperience); //Add the remaining experience
                            _remainingExperience = 0;
                            _additionInProgress = false;
                        });
                    }
                    else
                    {
                        _additionInProgress = false;
                    }
                });
            });
        }

        private static void LevelUp()
        {
            CurrentLevelExperience = 0;
            CurrentLevelID++;
            OnLevelChange(CurrentLevelID);
            //Debugging.DisplayDebugMessage("Player has leveled up to level " + (CurrentLevelID+1));
        }

        public static long ReturnExperienceForCurrentLevel()
        {
             return ReturnExperienceForLevelID(CurrentLevelID);
        }
        
        public static long ReturnAllPreviousExperienceForCurrentLevel()
        {
            long experienceOfPriorLevels = 0;
            for (var i = 0; i < CurrentLevelID; i++)
            {
                experienceOfPriorLevels += _levels[i].Experience;
            }
            return experienceOfPriorLevels + ReturnExperienceForLevelID(CurrentLevelID);
        }

        private static long ReturnExperienceForLevelID(int id)
        {
            return _levels[id].Experience;
        }
    }

    public class CreateExperienceData
    {
        //Experience needed to level up.
        private long _experience = 100;
        private const float ExperienceCurve = 1.1f;
        //Reward for leveling up.
        private long _reward = 10;
        private const float RewardCurve = 1.1f;
        //Increments.
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