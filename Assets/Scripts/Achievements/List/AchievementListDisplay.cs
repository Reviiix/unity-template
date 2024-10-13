using System.Collections;
using System.Collections.Generic;
using DebuggingAid;
using JetBrains.Annotations;
using pure_unity_methods;
using PureFunctions;
using PureFunctions.UnitySpecific;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Achievements.List
{
    /// <summary>
    /// This class is the base for handling displaying achievements in a list it is shared by dynamic and permanent achievements.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public abstract class AchievementListDisplay : MonoBehaviour
    {
        private Transform display;
        private static bool _initialised;
        private static readonly Color UnlockedColour = Color.yellow;
        private static readonly AssetReference DisplayItemPrefab = new ("Prefabs/UserInterface/Achievements/AchievementListItem.prefab");
        protected readonly Dictionary<Achievement, AchievementItemStruct> AchievementListItems = new Dictionary<Achievement, AchievementItemStruct>();
        [SerializeField] protected TMP_Text rewardDisplay;
        [SerializeField] protected TMP_Text amountOfUnlocksDisplay;
        [CanBeNull] [SerializeField] private TMP_Text noAchievementsSetText;
        
        protected virtual IEnumerator Start()
        {
            yield return Wait.WaitForInitialisation;
            display = GetComponent<Transform>();
        }
        
        protected void CreateListAsynchronously(IReadOnlyList<Achievement> achievements)
        {
            var amountOfAchievements = achievements.Count;
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<GameObject>(DisplayItemPrefab, (returnVariable) =>
            {
                for (var i = 0; i < amountOfAchievements; i++)
                {
                    var achievement = achievements[i];
                    CreateListItemAsynchronously(returnVariable, achievement, AchievementManager.GetDescription(achievement), AchievementManager.GetReward(achievement), AchievementManager.GetSpriteAssetReference(achievement), AchievementManager.GetUnlockedState(achievement));
                }
                if (amountOfAchievements > 0)
                {
                    if (noAchievementsSetText != null)
                    {
                        noAchievementsSetText.enabled = false;
                    }
                }
                _initialised = true;
                AssetReferenceLoader.DestroyOrUnload(returnVariable);
            });
        }
        
        private void CreateListItemAsynchronously(GameObject item, Achievement itemName, string description, int reward, AssetReference sprite, bool unlocked)
        {
            var achievementItemGameObject = Instantiate(item, display).GetComponentInChildren<AchievementListItem>();
            var achievementItemGameObjectTransform = achievementItemGameObject.transform;
            
            achievementItemGameObjectTransform.SetParent(display);
            achievementItemGameObjectTransform.localScale = Vector3.one;

            AchievementListItems.Add(itemName, new AchievementItemStruct(achievementItemGameObject.Background, achievementItemGameObject.ReturnNameDisplay, achievementItemGameObject.ReturnDescriptionDisplay, achievementItemGameObject.ReturnRewardDisplay, achievementItemGameObject.ReturnGraphic, unlocked));
            AchievementListItems[itemName].NameDisplay.text = StringUtilities.AddSpacesBeforeCapitals(itemName.ToString());
            AchievementListItems[itemName].DescriptionDisplay.text = description;
            AchievementListItems[itemName].RewardDisplay.text = reward.ToString();

            if (unlocked) SetUnlockedState(itemName);
            
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<Sprite>(sprite, (returnSprite) =>
            {
                AchievementListItems[itemName].Graphic.sprite = returnSprite;
                achievementItemGameObject.RemoveClassFromObject();
                
                if (sprite.Asset) 
                {
                    AssetReferenceLoader.UnloadAssetReference(sprite); //TODO: cant unload sprites in use
                }
            });
        }
        
        protected void OnAchievementUnlocked(Achievement achievement)
        {
            SetUnlockedState(achievement);
            SetRewardDisplay();
        }
        
        protected virtual void SetRewardDisplay()
        {
            DebugLogManager.LogError($"Override this method ({nameof(SetRewardDisplay)})");
        }
        
        private void SetUnlockedState(Achievement achievement)
        {
            if (!_initialised) return; //Sometimes an achievement for opening the app x times will try to set this before the list is created.
            
            AchievementListItems[achievement].Background.color = UnlockedColour;
        }
        
    
        protected readonly struct AchievementItemStruct
        {
            private static readonly Color UnLockedColourCache = UnlockedColour;
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
}
