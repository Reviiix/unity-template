using System.Collections;
using System.Collections.Generic;
using Achievements.Shared.List;
using PureFunctions;
using PureFunctions.UnitySpecific;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Achievements.Permanent
{
    /// <summary>
    /// This class displays the permanent achievements in a list
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class PermanentAchievementListDisplay : AchievementListDisplay
    {
        private const string DisplayItemPrefabPath = "Prefabs/UserInterface/Achievements/AchievementListItem.prefab";
        private static readonly AssetReference DisplayItemPrefab = new (DisplayItemPrefabPath);
        private readonly Dictionary<PermanentAchievementManager.Achievement, AchievementItemStruct> achievementListItems = new ();

        protected override IEnumerator Start()
        {
            yield return Wait.WaitForInitialisation;
            yield return StartCoroutine(base.Start());
            CreateDisplayAsynchronously();
        }

        private void OnEnable()
        {
            AchievementManager.OnPermanentAchievementUnlocked += OnAchievementUnlocked;
        }

        private void OnDisable()
        {
            AchievementManager.OnPermanentAchievementUnlocked += OnAchievementUnlocked;
        }

        private void OnAchievementUnlocked(PermanentAchievementManager.Achievement achievement)
        {
            SetUnlockedState(achievement);
            SetRewardDisplay();
        }

        private void CreateDisplayAsynchronously()
        {
            CreateListAsynchronously();
            SetRewardDisplay();
        }

        private void CreateListAsynchronously()
        {
            var achievements = AchievementManager.GetPermanentAchievements;
            var amountOfAchievements = achievements.Length;
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<GameObject>(DisplayItemPrefab, (returnVariable) =>
            {
                for (var i = 0; i < amountOfAchievements; i++)
                {
                    var achievement = achievements[i];
                    CreateListItemAsynchronously(returnVariable, achievement, AchievementManager.GetDescription(achievement), AchievementManager.GetReward(achievement), AchievementManager.GetSpriteAssetReference(achievement), AchievementManager.GetUnlockedState(achievement));
                }
                Initialised = true;
                AssetReferenceLoader.DestroyOrUnload(returnVariable);
            });
        }

        private void CreateListItemAsynchronously(GameObject item, PermanentAchievementManager.Achievement itemName, string description, int reward, AssetReference sprite, bool unlocked)
        {
            var achievementItemGameObject = Instantiate(item, Display).GetComponentInChildren<AchievementListItem>();
            var achievementItemGameObjectTransform = achievementItemGameObject.transform;
            
            achievementItemGameObjectTransform.SetParent(Display);
            achievementItemGameObjectTransform.localScale = Vector3.one;

            achievementListItems.Add(itemName, new AchievementItemStruct(achievementItemGameObject.Background, achievementItemGameObject.ReturnNameDisplay, achievementItemGameObject.ReturnDescriptionDisplay, achievementItemGameObject.ReturnRewardDisplay, achievementItemGameObject.ReturnGraphic, unlocked));
            achievementListItems[itemName].NameDisplay.text = StringUtilities.AddSpacesBeforeCapitals(itemName.ToString());
            achievementListItems[itemName].DescriptionDisplay.text = description;
            achievementListItems[itemName].RewardDisplay.text = reward.ToString();

            if (unlocked) SetUnlockedState(itemName);
            
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<Sprite>(sprite, (returnSprite) =>
            {
                achievementListItems[itemName].Graphic.sprite = returnSprite;
                achievementItemGameObject.RemoveClassFromObject();
                
                if (sprite.Asset) AssetReferenceLoader.UnloadAssetReference(sprite); //TODO: cant unload sprites in use
            });
        }

        private void SetUnlockedState(PermanentAchievementManager.Achievement achievement)
        {
            if (!Initialised) return; //Sometimes an achievement for opening the app x times will try to set this before the list is created.
            
            achievementListItems[achievement].Background.color = UnLockedColour;
        }

        private void SetRewardDisplay()
        {
            const string rewardsPrefix = "Total Rewards: ";
            const string amountPrefix = "Total Achievements: ";
            rewardDisplay.text = rewardsPrefix + AchievementManager.GetTotalRewardsOfPermanentAchievements(true) + "/" + AchievementManager.GetTotalRewardsOfPermanentAchievements();
            amountOfUnlocksDisplay.text = amountPrefix + AchievementManager.GetAmountOfPermanentAchievements(true) + "/" + AchievementManager.GetAmountOfPermanentAchievements();
        }
    }
}