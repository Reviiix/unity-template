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
        private readonly Dictionary<Achievement, Color> lockedColours = new();
        private readonly StringBuilder textBuilder = new();
        private SaveService saveService;

        protected AchievementService Achievements { get; private set; }

        protected abstract IReadOnlyList<AchievementInfo> ListedAchievements { get; }

        /// <summary>Resolves the achievement service and builds one row per listed achievement.</summary>
        private void Awake()
        {
            Achievements = ServiceLocator.Get<AchievementService>();
            saveService = ServiceLocator.Get<SaveService>();
            BuildRows();
        }

        /// <summary>Listens for unlocks and resets, and redraws every row in case either happened while hidden.</summary>
        private void OnEnable()
        {
            Achievements.AchievementUnlocked += OnAchievementUnlocked;
            saveService.DataLoaded += Refresh;
            Refresh();
        }

        /// <summary>Stops listening for unlocks and resets.</summary>
        private void OnDisable()
        {
            Achievements.AchievementUnlocked -= OnAchievementUnlocked;
            saveService.DataLoaded -= Refresh;
        }

        /// <summary>Recolours every row from its unlock state and rewrites the totals, e.g. after RESET ALL.</summary>
        private void Refresh()
        {
            foreach (var achievement in ListedAchievements)
            {
                if (rows.TryGetValue(achievement.Id, out var row)) row.Background.color = achievement.IsUnlocked ? UnlockedColour : lockedColours[achievement.Id];
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
                lockedColours.Add(achievement.Id, row.Background.color);
                row.Show(achievement, achievement.IsUnlocked ? UnlockedColour : row.Background.color);
                rows.Add(achievement.Id, row);
            }
            if (noAchievementsSetText != null) noAchievementsSetText.enabled = ListedAchievements.Count == 0;
        }

        /// <summary>Highlights the unlocked achievement's row and updates the totals.</summary>
        private void OnAchievementUnlocked(AchievementInfo achievement)
        {
            if (!rows.TryGetValue(achievement.Id, out var row)) return;

            row.Background.color = UnlockedColour;
            RefreshTotals();
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
