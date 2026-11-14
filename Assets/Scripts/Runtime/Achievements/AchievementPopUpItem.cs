using System;
using DG.Tweening;
using UnityEngine;

namespace UnityTemplate.Achievements
{
    /// <summary>The toast shown when an achievement unlocks: visible for three seconds, then fades out.</summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class AchievementPopUpItem : AchievementItemBase
    {
        private const float DisplaySeconds = 3;
        private const float FadeSeconds = 101 / 30f;
        private CanvasGroup canvasGroup;
        private Sequence showAndHide;

        /// <summary>Caches the canvas group used for the fade.</summary>
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>Shows the achievement, then fades out and calls onHidden.</summary>
        public void Show(AchievementInfo achievement, Action onHidden)
        {
            ShowTexts(achievement);
            canvasGroup.alpha = 1;
            gameObject.SetActive(true);

            showAndHide?.Kill();
            showAndHide = DOTween.Sequence()
                .AppendInterval(DisplaySeconds)
                .Append(DOTween.To(() => canvasGroup.alpha, alpha => canvasGroup.alpha = alpha, 0, FadeSeconds).SetEase(Ease.Linear))
                .SetLink(gameObject)
                .OnComplete(() => onHidden());
        }

        /// <summary>Stops the show-and-fade sequence and releases the icon.</summary>
        protected override void OnDestroy()
        {
            showAndHide?.Kill();
            base.OnDestroy();
        }
    }
}
