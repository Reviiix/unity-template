using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityTemplate.Core;

namespace UnityTemplate.Achievements
{
    /// <summary>Base for the achievement lists in the statistics menu: builds one row per achievement and keeps the totals text up to date.</summary>
    public abstract class AchievementListDisplay : MonoBehaviour
    {
        private static readonly Color UnlockedColour = Color.yellow;
        [SerializeField] protected TMP_Text rewardDisplay;
        [SerializeField] protected TMP_Text amountOfUnlocksDisplay;
        [SerializeField] private TMP_Text noAchievementsSetText;
        [SerializeField] private AchievementListItem listItemPrefab;
        private readonly Dictionary<Achievement, AchievementListItem> rows = new();
        private readonly StringBuilder textBuilder = new();

        protected AchievementService Achievements { get; private set; }

        protected abstract IReadOnlyList<AchievementInfo> ListedAchievements { get; }

        private void Awake()
        {
            Achievements = ServiceLocator.Get<AchievementService>();
            BuildRows();
        }

        private void OnEnable()
        {
            Achievements.AchievementUnlocked += OnAchievementUnlocked;
            RefreshTotals();
        }

        private void OnDisable()
        {
            Achievements.AchievementUnlocked -= OnAchievementUnlocked;
        }

        private void BuildRows()
        {
            foreach (var achievement in ListedAchievements)
            {
                var row = Instantiate(listItemPrefab, transform);
                row.transform.localScale = Vector3.one;
                row.Show(achievement, achievement.IsUnlocked ? UnlockedColour : row.Background.color);
                rows.Add(achievement.Id, row);
            }
            if (noAchievementsSetText != null) noAchievementsSetText.enabled = ListedAchievements.Count == 0;
        }

        private void OnAchievementUnlocked(AchievementInfo achievement)
        {
            if (!rows.TryGetValue(achievement.Id, out var row)) return;

            row.Background.color = UnlockedColour;
            RefreshTotals();
        }

        private void RefreshTotals()
        {
            var achievements = ListedAchievements;
            textBuilder.Clear().Append("Total Rewards: ").Append(AchievementService.GetTotalReward(achievements, true)).Append('/').Append(AchievementService.GetTotalReward(achievements, false));
            rewardDisplay.SetText(textBuilder);
            textBuilder.Clear().Append("Total Achievements: ").Append(AchievementService.GetCount(achievements, true)).Append('/').Append(AchievementService.GetCount(achievements, false));
            amountOfUnlocksDisplay.SetText(textBuilder);
        }
    }
}
