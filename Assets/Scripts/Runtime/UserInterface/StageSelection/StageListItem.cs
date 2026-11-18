using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Theming;

namespace UnityTemplate.UserInterface.MainMenus.StageSelection
{
    /// <summary>A stage button in the stage selection menu: its label, rating stars and locked state, referenced so the menu never has to search the prefab.</summary>
    public class StageListItem : MonoBehaviour
    {
        private const float LockedAlpha = 0.4f;
        [SerializeField] private Button stageLevelButton;
        [SerializeField] private TMP_Text stageNameDisplay;
        [SerializeField] private Image[] stars;
        [SerializeField] private CanvasGroup lockFade;
        [SerializeField] private ThemeColour earnedStar = ThemeColour.Accent;
        [SerializeField] private ThemeColour unearnedStar = ThemeColour.StarUnearned;
        private ThemeService theme;

        public Button Button => stageLevelButton;
        public TMP_Text StageNameDisplay => stageNameDisplay;

        /// <summary>Makes the stage tappable, or fades it out as locked.</summary>
        public void SetSelectable(bool selectable)
        {
            stageLevelButton.interactable = selectable;
            lockFade.alpha = selectable ? 1f : LockedAlpha;
        }

        /// <summary>Fills one star per rating point in the current theme; 0 means the stage has not been completed.</summary>
        public void ShowRating(int rating)
        {
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
