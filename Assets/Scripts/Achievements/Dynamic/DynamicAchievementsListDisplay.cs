using System.Collections;
using System.Collections.Generic;
using Achievements.Shared.List;
using PureFunctions;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Achievements.Dynamic
{
    /// <summary>
    /// This class displays the dynamic achievements in a list
    /// </summary>
    public class DynamicAchievementsListDisplay : AchievementListDisplay
    {
        private const string DisplayItemPrefabPath = "Prefabs/UserInterface/Achievements/AchievementListItem.prefab";
        private static readonly AssetReference DisplayItemPrefab = new AssetReference(DisplayItemPrefabPath);
        private readonly Dictionary<DynamicAchievementManager.Achievement, AchievementItemStruct> achievementListItems = new Dictionary<DynamicAchievementManager.Achievement, AchievementItemStruct>();
        [SerializeField] private TMP_Text noAchievementsSetText;

        private void OnEnable()
        {
            AchievementManager.OnDynamicAchievementsSet += OnAchievementsSet;
            AchievementManager.OnDynamicAchievementUnlocked += OnAchievementUnlocked;
        }
        
        private void OnDisable()
        {
            AchievementManager.OnDynamicAchievementsSet -= OnAchievementsSet;
            AchievementManager.OnDynamicAchievementUnlocked -= OnAchievementUnlocked;
        }
        
        private void OnAchievementUnlocked(DynamicAchievementManager.Achievement achievement)
        {
            SetUnlockedState(achievement);
            SetRewardDisplay();
        }

        private void OnAchievementsSet(DynamicAchievementManager.Achievement[] achievements)
        {
            CreateListAsynchronously(achievements);
            SetRewardDisplay();
        }

        private void CreateListAsynchronously(IReadOnlyList<DynamicAchievementManager.Achievement> achievements)
        {
            var amountOfAchievements = achievements.Count;
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<GameObject>(DisplayItemPrefab, (returnVariable) =>
            {
                for (var i = 0; i < amountOfAchievements; i++)
                {
                    var achievement = achievements[i];
                    CreateListItemAsynchronously(returnVariable, achievement, AchievementManager.GetDescription(achievement), AchievementManager.GetReward(achievement), AchievementManager.GetSpriteAssetReference(achievement), AchievementManager.GetUnlockedState(achievement));
                }
                //DisplayItemAddressableAsGameObject = null;
                if (amountOfAchievements > 0)
                {
                    noAchievementsSetText.enabled = false;
                }
                Initialised = true;
                AssetReferenceLoader.DestroyOrUnload(returnVariable);
            });
        }

        private void CreateListItemAsynchronously(GameObject item, DynamicAchievementManager.Achievement itemName, string description, int reward, AssetReference sprite, bool unlocked)
        {
            var achievementItemGameObject = Instantiate(item, Display).GetComponentInChildren<AchievementListItem>();
            var achievementItemGameObjectTransform = achievementItemGameObject.transform;
            
            achievementItemGameObjectTransform.SetParent(Display);
            achievementItemGameObjectTransform.localScale = Vector3.one;

            achievementListItems.Add(itemName, new AchievementItemStruct(achievementItemGameObject.Background, achievementItemGameObject.ReturnNameDisplay, achievementItemGameObject.ReturnDescriptionDisplay, achievementItemGameObject.ReturnRewardDisplay, achievementItemGameObject.ReturnGraphic, unlocked));
            achievementListItems[itemName].NameDisplay.text = StringUtilities.AddSpacesBeforeCapitals(itemName.ToString());
            achievementListItems[itemName].DescriptionDisplay.text = description;
            achievementListItems[itemName].RewardDisplay.text = reward.ToString();

            if (unlocked)
            {
                SetUnlockedState(itemName);
            }
            
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<Sprite>(sprite, (returnSprite) =>
            {
                achievementListItems[itemName].Graphic.sprite = returnSprite;
                achievementItemGameObject.RemoveClassFromObject();
                if (sprite.Asset) 
                {
                    AssetReferenceLoader.UnloadAssetReference(sprite); //TODO: cant unload sprites in use
                }
            });
        }
        
        private void SetRewardDisplay()
        {
            const string rewardsPrefix = "Total Rewards: ";
            const string amountPrefix = "Total Achievements: ";
            const string backslash = "/";
            rewardDisplay.text = rewardsPrefix + AchievementManager.GetTotalRewardsOfDynamicAchievements(true) + backslash + AchievementManager.GetTotalRewardsOfDynamicAchievements();
            amountOfUnlocksDisplay.text = amountPrefix + AchievementManager.GetAmountOfDynamicAchievements(true) + backslash + AchievementManager.GetAmountOfDynamicAchievements();
        }
        
        private void SetUnlockedState(DynamicAchievementManager.Achievement achievement)
        {
            if (!Initialised) return; //Sometimes an achievement for opening the app X amount of times will try to set this before the list is created.
            
            achievementListItems[achievement].Background.color = UnLockedColour;
        }
    }
}
