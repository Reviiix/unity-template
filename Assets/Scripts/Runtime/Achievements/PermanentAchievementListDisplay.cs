using System.Collections.Generic;

namespace UnityTemplate.Achievements
{
    /// <summary>Lists the permanent achievements in the statistics menu.</summary>
    public class PermanentAchievementListDisplay : AchievementListDisplay
    {
        protected override IReadOnlyList<AchievementInfo> ListedAchievements => Achievements.PermanentAchievements;
    }
}
