using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Accessibility;
using UnityTemplate.Core;
using UnityTemplate.Theming;

namespace UnityTemplate.UserInterface.MainMenus.StageSelection
{
    /// <summary>A stage button in the stage selection menu: its label, rating stars and locked state, referenced so the menu never has to search the prefab.</summary>
    public class StageListItem : MonoBehaviour, IScreenReaderLabelSource
    {
        private const float LockedAlpha = 0.4f;
        [SerializeField] private Button stageLevelButton;
        [SerializeField] private TMP_Text stageNameDisplay;
        [SerializeField] private Image[] stars;
        [SerializeField] private CanvasGroup lockFade;
        [SerializeField] private ThemeColour earnedStar = ThemeColour.Accent;
        [SerializeField] private ThemeColour unearnedStar = ThemeColour.StarUnearned;
        private readonly StringBuilder spokenBuilder = new();
        private ThemeService theme;
        private int shownRating;

        public Button Button => stageLevelButton;
        public TMP_Text StageNameDisplay => stageNameDisplay;

        /// <summary>What the screen reader says for the tile, e.g. "Stage 1-2, 2 of 3 stars"; a locked tile is also read as dimmed.</summary>
        public string SpokenLabel => spokenBuilder.Clear().Append("Stage ").Append(ScreenReaderLabels.Clean(stageNameDisplay.text)).Append(", ")
            .Append(shownRating).Append(" of ").Append(stars.Length).Append(" stars").ToString();

        /// <summary>Makes the stage tappable, or fades it out as locked.</summary>
        public void SetSelectable(bool selectable)
        {
            stageLevelButton.interactable = selectable;
            lockFade.alpha = selectable ? 1f : LockedAlpha;
        }

        /// <summary>Fills one star per rating point in the current theme; 0 means the stage has not been completed.</summary>
        public void ShowRating(int rating)
        {
            shownRating = rating;
            theme ??= ServiceLocator.Get<ThemeService>();
            var earned = theme.Get(earnedStar);
            var unearned = theme.Get(unearnedStar);
            for (var i = 0; i < stars.Length; i++)
            {
                stars[i].color = i < rating ? earned : unearned;
            }
        }
    }
}
