using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityTemplate.UserInterface.MainMenus.StageSelection
{
    /// <summary>References to the parts of a stage button prefab, so the stage selection menu never has to search the prefab.</summary>
    public class StageListItem : MonoBehaviour
    {
        [SerializeField] private Button stageLevelButton;
        [SerializeField] private TMP_Text stageNameDisplay;
        [SerializeField] private Image[] stars;

        public Button Button => stageLevelButton;
        public TMP_Text StageNameDisplay => stageNameDisplay;
        public Image[] Stars => stars;
    }
}
