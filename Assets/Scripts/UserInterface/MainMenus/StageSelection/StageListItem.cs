using Abstract;
using Abstract.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.MainMenus.StageSelection
{
    //This class just holds the variables until the stage selection menu caches them. Then it removes itself (So no searching of the prefab for children and components is needed and eventually less MonoBehaviours)
    public class StageListItem : MonoBehaviour, IListItem
    {
        [SerializeField] private Button stageLevelButton;
        [SerializeField] private TMP_Text stageNameDisplay;
        [SerializeField] private Image[] stars;

        public Image[] ReturnStarDisplay()
        {
            return stars;
        }
        
        public Button ReturnButton()
        {
             return stageLevelButton;
        }

        public TMP_Text ReturnStageNameDisplay()
        {
            return stageNameDisplay;
        }
        
        public void RemoveClassFromObject()
        {
            Destroy(GetComponent<StageListItem>());
        }
    }
}
