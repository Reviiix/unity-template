using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityTemplate.Core;
using UnityTemplate.Theming;

namespace UnityTemplate.Achievements
{
    /// <summary>Base for the achievement lists in the statistics menu: builds one row per achievement and keeps the totals text up to date.</summary>
    public abstract class AchievementListDisplay : MonoBehaviour
    {
        [SerializeField] private ThemeColour unlockedColour = ThemeColour.AccentSoft;
        [SerializeField] private ThemeColour lockedColour = ThemeColour.Muted;
        [SerializeField] protected TMP_Text rewardDisplay;
        [SerializeField] protected TMP_Text amountOfUnlocksDisplay;
        [SerializeField] private TMP_Text noAchievementsSetText;
        [SerializeField] private AchievementListItem listItemPrefab;
        private readonly Dictionary<Achievement, AchievementListItem> rows = new();
        private readonly StringBuilder textBuilder = new();
        private SaveService saveService;
        private ThemeService theme;

        protected AchievementService Achievements { get; private set; }

        protected abstract IReadOnlyList<AchievementInfo> ListedAchievements { get; }

        /// <summary>Resolves the achievement service and builds one row per listed achievement.</summary>
        private void Awake()
        {
            Achievements = ServiceLocator.Get<AchievementService>();
            saveService = ServiceLocator.Get<SaveService>();
            theme = ServiceLocator.Get<ThemeService>();
            BuildRows();
        }

        /// <summary>Listens for unlocks, resets and theme changes, and redraws every row in case any happened while hidden.</summary>
        private void OnEnable()
        {
            Achievements.AchievementUnlocked += OnAchievementUnlocked;
            saveService.DataLoaded += Refresh;
            theme.Changed += Refresh;
            Refresh();
        }

        /// <summary>Stops listening for unlocks, resets and theme changes.</summary>
        private void OnDisable()
        {
            Achievements.AchievementUnlocked -= OnAchievementUnlocked;
            saveService.DataLoaded -= Refresh;
            theme.Changed -= Refresh;
        }

        /// <summary>Re-shows every row's unlock state in the current theme and rewrites the totals, e.g. after RESET ALL.</summary>
        private void Refresh()
        {
            foreach (var achievement in ListedAchievements)
            {
                if (rows.TryGetValue(achievement.Id, out var row)) row.SetUnlocked(achievement.IsUnlocked, RowColour(achievement.IsUnlocked));
            }
            RefreshTotals();
        }

        /// <summary>Instantiates a row for every listed achievement and shows the empty-list text when there are none.</summary>
        private void BuildRows()
        {
            foreach (var achievement in ListedAchievements)
            {
                var row = Instantiate(listItemPrefab, transform);
                row.transform.localScale = Vector3.one;
                row.Show(achievement, achievement.IsUnlocked, RowColour(achievement.IsUnlocked));
                rows.Add(achievement.Id, row);
            }
            if (noAchievementsSetText != null) noAchievementsSetText.enabled = ListedAchievements.Count == 0;
        }

        /// <summary>Shows the newly unlocked achievement's row as unlocked and updates the totals.</summary>
        private void OnAchievementUnlocked(AchievementInfo achievement)
        {
            if (!rows.TryGetValue(achievement.Id, out var row)) return;

            row.SetUnlocked(true, RowColour(true));
            RefreshTotals();
        }

        /// <summary>Returns the current theme's background for an unlocked or locked row.</summary>
        private Color RowColour(bool unlocked)
        {
            return theme.Get(unlocked ? unlockedColour : lockedColour);
        }

        /// <summary>Writes the unlocked/total reward and achievement counts.</summary>
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
