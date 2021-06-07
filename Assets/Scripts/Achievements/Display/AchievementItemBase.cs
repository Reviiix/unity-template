using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Achievements.Display
{
    public abstract class AchievementItemBase : MonoBehaviour
    {
        [SerializeField] protected Image graphic;
        [SerializeField] protected TMP_Text title;
        [SerializeField] protected TMP_Text description;
        [SerializeField] protected TMP_Text reward;

        public Image ReturnGraphic=>graphic;
        public TMP_Text ReturnNameDisplay=> title;
        public TMP_Text ReturnDescriptionDisplay=> description;
        public TMP_Text ReturnRewardDisplay=> reward;
    }
}
