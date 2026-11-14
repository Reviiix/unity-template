using System.Collections.Generic;

namespace UnityTemplate.Achievements
{
    /// <summary>Lists the weekly, remotely configured achievements in the statistics menu.</summary>
    public class DynamicAchievementsListDisplay : AchievementListDisplay
    {
        protected override IReadOnlyList<AchievementInfo> ListedAchievements => Achievements.DynamicAchievements;
    }
}
