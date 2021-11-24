using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Transform))]
public abstract class AchievementListDisplay : MonoBehaviour
{
    protected Transform Display;
    protected static bool ListCreated;
    protected static readonly Color UnLockedColour = Color.yellow;
    protected static GameObject DisplayItemAddressableAsGameObject;
    protected static readonly WaitUntil WaitUntilAssetReferenceIsLoadedAsynchronously = new WaitUntil(() => DisplayItemAddressableAsGameObject != null);
    [SerializeField] protected TMP_Text rewardDisplay;
    [SerializeField] protected TMP_Text amountOfUnlocksDisplay;
    
    protected virtual void Awake()
    {
        Display = GetComponent<Transform>();
    }
    
    protected readonly struct AchievementItemStruct
    {
        private static readonly Color UnLockedColourCache = UnLockedColour;
        public readonly RawImage Background;
        public readonly TMP_Text NameDisplay;
        public readonly TMP_Text DescriptionDisplay;
        public readonly TMP_Text RewardDisplay;
        public readonly Image Graphic;

        public AchievementItemStruct(RawImage background, TMP_Text nameDisplay, TMP_Text descriptionDisplay, TMP_Text rewardDisplay, Image graphic, bool unlocked)
        {
            Background = background;
            NameDisplay = nameDisplay;
            DescriptionDisplay = descriptionDisplay;
            RewardDisplay = rewardDisplay;
            Graphic = graphic;
            if (unlocked)
            {
                Background.color = UnLockedColourCache;
            }
        }
    }
}
