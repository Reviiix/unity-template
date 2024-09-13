using System.Collections.Generic;
using Credits;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Achievements
{
    public abstract class BaseAchievementManager : MonoBehaviour
    {
        protected const bool Enabled = ProjectManager.EnabledFeatures.Achievements;
        protected const string AchievementGraphicsFolderAssetPath = "Graphics/Achievements/";
        protected Dictionary<Achievement, AchievementInformation> Achievements;
        public string GetDescription(Achievement achievement) => Achievements[achievement].Description;
        public int GetReward(Achievement achievement) => Achievements[achievement].Reward;
        public AssetReference GetSpriteAssetReference(Achievement achievement) => Achievements[achievement].SpriteAssetReference;
        public bool GetUnlockedState(Achievement achievement) => Achievements[achievement].Unlocked;

        public bool AchievementIsInSet(Achievement achievement) => Achievements.ContainsKey(achievement);

        public Achievement[] GetAchievements()
        {
            var returnVariable = new List<Achievement>();
            foreach (var achievement in Achievements)
            {
                returnVariable.Add(achievement.Key);
            }
            return returnVariable.ToArray();
        }
        
        public int GetTotalRewards(bool unlockedOnly = false)
        {
            var returnVariable = 0;
            foreach (var achievement in Achievements)
            {
                if (unlockedOnly)
                {
                    if (achievement.Value.Unlocked)
                    {
                        returnVariable += GetReward(achievement.Key);
                    }
                }
                else
                {
                    returnVariable += GetReward(achievement.Key);
                }
            }
            return returnVariable;
        }
        
        public int ReturnAmountOfAchievements(bool unlockedOnly = false)
        {
            var returnVariable = 0;
            foreach (var achievement in Achievements)
            {
                if (unlockedOnly)
                {
                    if (achievement.Value.Unlocked)
                    {
                        returnVariable++;
                    }
                }
                else
                {
                    returnVariable++;
                }
            }
            return returnVariable;
        }
        
        public bool[] GetUnLockStates()
        {
            var returnVariable = new List<bool>();
            foreach (var achievement in Achievements)
            {
                returnVariable.Add(achievement.Value.Unlocked);
            }
            return returnVariable.ToArray();
        }
                
        
        protected class AchievementInformation
        {
            public readonly string Description;
            public readonly int Reward;
            public readonly AssetReference SpriteAssetReference;
            public bool Unlocked { get; private set; }

            public AchievementInformation(string description, int reward, AssetReference sprite, bool unlocked = false)
            {
                Description = description;
                Reward = reward;
                SpriteAssetReference = sprite;
                Unlocked = unlocked;
            }

            public void Unlock(bool addCredits = true)
            {
                Unlocked = true;
                if (addCredits)
                {
                    CreditsManager.AddCredits(CreditsManager.Currency.PremiumCredits, Reward);
                }
            }
        }
    }

    public enum Achievement
    {
        PlayForOneHourConsecutively,
        PlayForOneHour,
        PlayForFiveHours, 
        PlayForTenHours, 
        PlayForTwentyFiveHours, 
        PlayForFiftyHours, 
        PlayForOneHundredHours,
            
        OpenTheAppOnce,
        OpenTheAppFiveTimes,
        OpenTheAppTenTimes,
        OpenTheAppTwentyFiveTimes,
        OpenTheAppFiftyTimes,
        OpenTheAppOneHundredTimes,
            
        ReachLevelTwo,
        ReachLevelFive,
        ReachLevelTen,
        ReachLevelTwentyFive,
        ReachLevelFifty,
        ReachLevelOneHundred,
            
        OneHundredCredits,
        TwoHundredAndFiftyCredits,
        FiveHundredCredits,
        OneThousandCredits,
        TenThousandCredits,
        OneHundredThousandCredits,
            
        OneHundredPremiumCredits,
        TwoHundredAndFiftyPremiumCredits,
        FiveHundredPremiumCredits,
        OneThousandPremiumCredits,
        TenThousandPremiumCredits,
        OneHundredThousandPremiumCredits,
            
            
            
            
            
            
        HighScore,
        ConsecutivePlayTimeInSeconds,
        TotalPlayTimeInSeconds,
        ExperienceGained,
        LevelsComplete,
    }
}