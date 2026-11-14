using System;

namespace UnityTemplate.Core
{
    /// <summary>Remotely tunable settings; field names must match the JSON keys of the "Configuration" entry in the Remote Config dashboard.</summary>
    [Serializable]
    public class GameConfiguration
    {
        private const int MaximumExperienceLevels = 200;

        public string Holiday = "None";
        public int AmountOfExperienceLevels = 50;
        public DynamicAchievementTargets DynamicAchievements = new();

        public bool IsValid => AmountOfExperienceLevels is > 0 and <= MaximumExperienceLevels && DynamicAchievements != null && DynamicAchievements.IsValid;
    }

    /// <summary>Targets and rewards of the remotely configured achievements; a target of -1 disables that achievement.</summary>
    [Serializable]
    public class DynamicAchievementTargets
    {
        public const int DisabledTarget = -1;

        public int highScore = DisabledTarget;
        public int highScoreReward;
        public int consecutivePlayTimeInSeconds = DisabledTarget;
        public int consecutivePlayTimeInSecondsReward;
        public int totalPlayTimeInSeconds = DisabledTarget;
        public int totalPlayTimeInSecondsReward;
        public int experienceGained = DisabledTarget;
        public int experienceGainedReward;
        public int stagesComplete = DisabledTarget;
        public int stagesCompleteReward;

        public bool IsValid => highScore >= DisabledTarget && consecutivePlayTimeInSeconds >= DisabledTarget && totalPlayTimeInSeconds >= DisabledTarget
                               && experienceGained >= DisabledTarget && stagesComplete >= DisabledTarget
                               && highScoreReward >= 0 && consecutivePlayTimeInSecondsReward >= 0 && totalPlayTimeInSecondsReward >= 0
                               && experienceGainedReward >= 0 && stagesCompleteReward >= 0;
    }
}
