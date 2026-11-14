using UnityEngine;

namespace UnityTemplate.Achievements
{
    /// <summary>One row of an achievement list; locked rows are faded so unlocked ones stand out without relying on colour alone.</summary>
    public class AchievementListItem : AchievementItemBase
    {
        private const float LockedAlpha = 0.5f;
        [SerializeField] private CanvasGroup lockFade;

        /// <summary>Shows an achievement and whether it is unlocked.</summary>
        public void Show(AchievementInfo achievement, bool unlocked, Color backgroundColour)
        {
            ShowTexts(achievement);
            SetUnlocked(unlocked, backgroundColour);
        }

        /// <summary>Shows the row at full strength on the given background when unlocked, or faded when locked.</summary>
        public void SetUnlocked(bool unlocked, Color backgroundColour)
        {
            Background.color = backgroundColour;
            lockFade.alpha = unlocked ? 1f : LockedAlpha;
        }
    }
}
