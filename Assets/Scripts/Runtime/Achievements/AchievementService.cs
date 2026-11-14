using System;
using System.Collections.Generic;
using UnityTemplate.Core;
using UnityTemplate.Economy;

namespace UnityTemplate.Achievements
{
    /// <summary>Holds the permanent and remotely configured achievements, unlocks them (paying their premium-credit reward) and saves them by id.</summary>
    public class AchievementService : ISaveable
    {
        private readonly SaveService saveService;
        private readonly CreditsService credits;
        private readonly AchievementCatalogue catalogue;
        private readonly List<AchievementInfo> permanentAchievements = new();
        private readonly List<AchievementInfo> dynamicAchievements = new();
        private readonly HashSet<Achievement> unlocked = new();

        public bool IsEnabled { get; }
        public IReadOnlyList<AchievementInfo> PermanentAchievements => permanentAchievements;
        public IReadOnlyList<AchievementInfo> DynamicAchievements => dynamicAchievements;

        public event Action<AchievementInfo> AchievementUnlocked;

        /// <summary>Builds the achievement lists; when the feature is disabled the lists still exist but nothing can be unlocked.</summary>
        public AchievementService(SaveService saveService, CreditsService credits, AchievementCatalogue catalogue, DynamicAchievementTargets dynamicTargets, bool isEnabled)
        {
            this.saveService = saveService;
            this.credits = credits;
            this.catalogue = catalogue;
            IsEnabled = isEnabled;

            foreach (var definition in catalogue.PermanentAchievements)
            {
                permanentAchievements.Add(new AchievementInfo(definition.achievement, definition.description, definition.reward, definition.icon));
            }
            BuildDynamicAchievements(dynamicTargets);
        }

        /// <summary>Unlocks an achievement once, pays its reward and announces it; ignored when the feature is disabled.</summary>
        public void Unlock(Achievement achievement)
        {
            if (!IsEnabled || unlocked.Contains(achievement)) return;

            var info = Find(achievement);
            if (info == null) return;

            unlocked.Add(achievement);
            info.IsUnlocked = true;
            credits.Add(Currency.PremiumCredits, info.Reward);
            AchievementUnlocked?.Invoke(info);
            saveService.Save();
        }

        /// <summary>Sums the rewards of a list, optionally only of its unlocked achievements.</summary>
        public static int GetTotalReward(IReadOnlyList<AchievementInfo> achievements, bool unlockedOnly)
        {
            var total = 0;
            foreach (var achievement in achievements)
            {
                if (!unlockedOnly || achievement.IsUnlocked) total += achievement.Reward;
            }
            return total;
        }

        /// <summary>Counts a list's achievements, optionally only the unlocked ones.</summary>
        public static int GetCount(IReadOnlyList<AchievementInfo> achievements, bool unlockedOnly)
        {
            if (!unlockedOnly) return achievements.Count;

            var count = 0;
            foreach (var achievement in achievements)
            {
                if (achievement.IsUnlocked) count++;
            }
            return count;
        }

        /// <summary>Restores unlocked achievements by id; unknown ids from older versions are ignored.</summary>
        public void Load(SaveData data)
        {
            unlocked.Clear();
            foreach (var id in data.unlockedAchievements)
            {
                if (Enum.TryParse<Achievement>(id, out var achievement)) unlocked.Add(achievement);
            }
            RefreshUnlockedFlags(permanentAchievements);
            RefreshUnlockedFlags(dynamicAchievements);
        }

        /// <summary>Writes the ids of every unlocked achievement.</summary>
        public void Save(SaveData data)
        {
            var ids = new string[unlocked.Count];
            var i = 0;
            foreach (var achievement in unlocked) ids[i++] = achievement.ToString();
            data.unlockedAchievements = ids;
        }

        private AchievementInfo Find(Achievement achievement)
        {
            foreach (var info in permanentAchievements) if (info.Id == achievement) return info;
            foreach (var info in dynamicAchievements) if (info.Id == achievement) return info;
            return null;
        }

        private void RefreshUnlockedFlags(List<AchievementInfo> achievements)
        {
            foreach (var info in achievements) info.IsUnlocked = unlocked.Contains(info.Id);
        }

        private void BuildDynamicAchievements(DynamicAchievementTargets targets)
        {
            dynamicAchievements.Clear();
            AddDynamic(Achievement.HighScore, targets.highScore, targets.highScoreReward, "Achieve a high score of {0}");
            AddDynamic(Achievement.ConsecutivePlayTimeInSeconds, targets.consecutivePlayTimeInSeconds, targets.consecutivePlayTimeInSecondsReward, "Play for {0} seconds consecutively.");
            AddDynamic(Achievement.TotalPlayTimeInSeconds, targets.totalPlayTimeInSeconds, targets.totalPlayTimeInSecondsReward, "Play for {0} seconds.");
            AddDynamic(Achievement.ExperienceGained, targets.experienceGained, targets.experienceGainedReward, "Gain {0} experience.");
            AddDynamic(Achievement.LevelsComplete, targets.stagesComplete, targets.stagesCompleteReward, "Complete {0} levels.");
            RefreshUnlockedFlags(dynamicAchievements);
        }

        private void AddDynamic(Achievement achievement, int target, int reward, string descriptionFormat)
        {
            if (target == DynamicAchievementTargets.DisabledTarget) return;

            dynamicAchievements.Add(new AchievementInfo(achievement, string.Format(descriptionFormat, target), reward, catalogue.DynamicAchievementIcon));
        }
    }
}
