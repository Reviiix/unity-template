using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;

namespace UnityTemplate.Progression
{
    /// <summary>Shows the player's level, an experience bar and "current/needed" text, animating every experience gain.</summary>
    public class ExperienceDisplay : MonoBehaviour
    {
        private const float AnimationSeconds = 3;
        [SerializeField] private Image levelBar;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text experienceText;
        private readonly Queue<ValueChange> queuedGains = new();
        private readonly StringBuilder textBuilder = new();
        private ExperienceService experience;
        private Sequence gainAnimation;
        private int shownLevel;

        private SaveService saveService;

        private void OnEnable()
        {
            experience = ServiceLocator.Get<ExperienceService>();
            saveService = ServiceLocator.Get<SaveService>();
            experience.ExperienceChanged += OnExperienceChanged;
            saveService.DataLoaded += ShowCurrentExperience;
            ShowCurrentExperience();
        }

        private void OnDisable()
        {
            gainAnimation?.Kill();
            gainAnimation = null;
            experience.ExperienceChanged -= OnExperienceChanged;
            saveService.DataLoaded -= ShowCurrentExperience;
        }

        private void ShowCurrentExperience()
        {
            gainAnimation?.Kill();
            queuedGains.Clear();
            ShowState(experience.CurrentLevel, experience.TotalExperience);
        }

        private void OnExperienceChanged(ValueChange change)
        {
            if (gainAnimation != null && gainAnimation.IsActive())
            {
                queuedGains.Enqueue(change);
                return;
            }
            AnimateGain(change);
        }

        private void AnimateGain(ValueChange change)
        {
            gainAnimation = DOTween.Sequence().SetLink(gameObject);
            var from = change.OldValue;
            var totalGain = change.NewValue - change.OldValue;
            var level = shownLevel;

            while (from < change.NewValue)
            {
                var levelEnd = experience.GetCumulativeExperience(level);
                var reachesNextLevel = change.NewValue >= levelEnd && level < experience.MaximumLevel;
                var to = reachesNextLevel ? levelEnd : change.NewValue;
                var seconds = AnimationSeconds * (to - from) / totalGain;
                AppendSegment(level, from, to, seconds);

                if (!reachesNextLevel) break;
                level++;
                var nextLevel = level;
                gainAnimation.AppendCallback(() => ShowState(nextLevel, levelEnd));
                from = to;
            }

            gainAnimation.OnComplete(() =>
            {
                gainAnimation = null;
                if (queuedGains.Count > 0) AnimateGain(queuedGains.Dequeue());
            });
        }

        private void AppendSegment(int level, long from, long to, float seconds)
        {
            double shownExperience = from;
            gainAnimation.Append(DOTween.To(() => levelBar.fillAmount, fill => levelBar.fillAmount = fill, GetBarFill(to, level), seconds).SetEase(Ease.OutQuad));
            gainAnimation.Join(DOTween.To(() => shownExperience, value =>
            {
                shownExperience = value;
                ShowExperienceText((long)shownExperience, level);
            }, to, seconds).SetEase(Ease.OutQuad));
        }

        private void ShowState(int level, long totalExperience)
        {
            shownLevel = level;
            levelText.SetText("{0:0}", level + 1);
            levelBar.fillAmount = GetBarFill(totalExperience, level);
            ShowExperienceText(totalExperience, level);
        }

        private void ShowExperienceText(long totalExperience, int level)
        {
            textBuilder.Clear().Append(totalExperience).Append('/').Append(experience.GetCumulativeExperience(level));
            experienceText.SetText(textBuilder);
        }

        private float GetBarFill(long totalExperience, int level)
        {
            var levelStart = level > 0 ? experience.GetCumulativeExperience(level - 1) : 0;
            return Mathf.Clamp01((float)(totalExperience - levelStart) / experience.GetExperienceForLevel(level));
        }
    }
}
