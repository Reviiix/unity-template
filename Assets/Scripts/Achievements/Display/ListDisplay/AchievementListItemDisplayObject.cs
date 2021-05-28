using Abstract;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserInterface.MainMenus.StageSelection;

namespace Achievements.Display.ListDisplay
{
    public class AchievementListItemDisplayObject : MonoBehaviour, IListItemDisplayObject
    {
        [SerializeField] private TMP_Text nameDisplay;
        [SerializeField] private TMP_Text descriptionDisplay;
        [SerializeField] private TMP_Text rewardDisplay;
        [SerializeField] private Image graphic;

        public Image ReturnGraphic()
        {
            return graphic;
        }

        public TMP_Text ReturnNameDisplay()
        {
            return nameDisplay;
        }
        
        public TMP_Text ReturnDescriptionDisplay()
        {
            return descriptionDisplay;
        }
        
        public TMP_Text ReturnRewardDisplay()
        {
            return rewardDisplay;
        }
        
        public void RemoveClassFromObject()
        {
            Destroy(GetComponent<StageListItemDisplayObject>());
        }
    }
}