using UnityEngine;

namespace UnityTemplate.Achievements
{
    /// <summary>One row of an achievement list.</summary>
    public class AchievementListItem : AchievementItemBase
    {
        /// <summary>Shows an achievement with the given background colour (yellow once unlocked).</summary>
        public void Show(AchievementInfo achievement, Color backgroundColour)
        {
            Background.color = backgroundColour;
            ShowTexts(achievement);
        }
    }
}
