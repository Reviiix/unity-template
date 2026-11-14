using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private Color earnedStarColour = new(0.24f, 0.39f, 0.87f);
        [SerializeField] private Color unearnedStarColour = new(0.85f, 0.87f, 0.9f);

        public Button Button => stageLevelButton;
        public TMP_Text StageNameDisplay => stageNameDisplay;

        /// <summary>Makes the stage tappable, or fades it out as locked.</summary>
        public void SetSelectable(bool selectable)
        {
            stageLevelButton.interactable = selectable;
            lockFade.alpha = selectable ? 1f : LockedAlpha;
        }

        /// <summary>Fills one star per rating point; 0 means the stage has not been completed.</summary>
        public void ShowRating(int rating)
        {
            for (var i = 0; i < stars.Length; i++)
            {
                stars[i].color = i < rating ? earnedStarColour : unearnedStarColour;
            }
        }
    }
}
