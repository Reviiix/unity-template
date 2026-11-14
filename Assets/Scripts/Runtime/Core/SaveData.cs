using System;

namespace UnityTemplate.Core
{
    /// <summary>Everything that is persisted between sessions, as plain JsonUtility-friendly fields; bump version when the layout changes.</summary>
    [Serializable]
    public class SaveData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;

        public long credits;
        public long premiumCredits;

        public long totalExperience;

        public int[] stageRatings = Array.Empty<int>();
        public int highScore;

        public int timesOpened;
        public long totalPlayTimeTicks;
        public long firstOpenDateTicks;
        public int anniversariesClaimed;
        public int lastDailyBonusDay = -1;
        public int dailyBonusStreak;
        public int pendingWheelSegment = -1;

        public float volume = 1f;
        public bool muted;

        public string[] unlockedAchievements = Array.Empty<string>();
    }
}
