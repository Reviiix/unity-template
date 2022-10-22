using System.Collections;
using System.Collections.Generic;
using Achievements.Shared.List;
using PureFunctions;
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
        private static readonly AssetReference DisplayItemPrefab = new AssetReference(DisplayItemPrefabPath);
        private readonly Dictionary<PermanentAchievementManager.Achievement, AchievementItemStruct> _achievementListItems = new Dictionary<PermanentAchievementManager.Achievement, AchievementItemStruct>();

        protected override IEnumerator Start()
        {
            yield return ProjectManager.WaitForInitialisation;
            yield return StartCoroutine(base.Start());
            CreateDisplayAsynchronously();
        }

        private void OnEnable()
        {
            PermanentAchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
        }

        private void OnDisable()
        {
            PermanentAchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
        }

        private void OnAchievementUnlocked(PermanentAchievementManager.Achievement achievement)
        {
            SetUnlockedState(achievement);
            SetRewardDisplay();
        }

        private void CreateDisplayAsynchronously()
        {
            StartCoroutine(CreateListAsynchronously());
            SetRewardDisplay();
        }

        private IEnumerator CreateListAsynchronously()
        {
            var achievements = PermanentAchievementManager.ReturnAllAchievements();
            var amountOfAchievements = achievements.Length;
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<GameObject>(DisplayItemPrefab, (returnVariable) =>
            {
                DisplayItemAddressableAsGameObject = returnVariable;
                AssetReferenceLoader.DestroyOrUnload(returnVariable);
            });
            yield return WaitUntilAssetReferenceIsLoadedAsynchronously;
            for (var i = 0; i < amountOfAchievements; i++)
            {
                var achievement = achievements[i];
                CreateListItemAsynchronously(achievement, PermanentAchievementManager.ReturnDescription(achievement), PermanentAchievementManager.ReturnReward(achievement), PermanentAchievementManager.ReturnSpriteAssetReference(achievement), PermanentAchievementManager.ReturnUnlockedState(achievement));
            }

            DisplayItemAddressableAsGameObject = null;
            ListCreated = true;
        }

        private void CreateListItemAsynchronously(PermanentAchievementManager.Achievement itemName, string description, int reward, AssetReference sprite, bool unlocked)
        {
            var achievementItemGameObject = Instantiate(DisplayItemAddressableAsGameObject, Display).GetComponentInChildren<AchievementListItem>();
            var achievementItemGameObjectTransform = achievementItemGameObject.transform;
            
            achievementItemGameObjectTransform.SetParent(Display);
            achievementItemGameObjectTransform.localScale = Vector3.one;

            _achievementListItems.Add(itemName, new AchievementItemStruct(achievementItemGameObject.Background, achievementItemGameObject.ReturnNameDisplay, achievementItemGameObject.ReturnDescriptionDisplay, achievementItemGameObject.ReturnRewardDisplay, achievementItemGameObject.ReturnGraphic, unlocked));
            _achievementListItems[itemName].NameDisplay.text = StringUtilities.AddSpacesBeforeCapitals(itemName.ToString());
            _achievementListItems[itemName].DescriptionDisplay.text = description;
            _achievementListItems[itemName].RewardDisplay.text = reward.ToString();

            if (unlocked) SetUnlockedState(itemName);
            
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<Sprite>(sprite, (returnSprite) =>
            {
                _achievementListItems[itemName].Graphic.sprite = returnSprite;
                achievementItemGameObject.RemoveClassFromObject();
                
                if (sprite.Asset) AssetReferenceLoader.UnloadAssetReference(sprite); //TODO: cant unload sprites in use
            });
        }

        private void SetUnlockedState(PermanentAchievementManager.Achievement achievement)
        {
            if (!ListCreated) return; //Sometimes an achievement for opening the app x times will try to set this before the list is created.
            
            _achievementListItems[achievement].Background.color = UnLockedColour;
        }

        private void SetRewardDisplay()
        {
            const string rewardsPrefix = "Total Rewards: ";
            const string amountPrefix = "Total Achievements: ";
            rewardDisplay.text = rewardsPrefix + PermanentAchievementManager.ReturnTotalRewards(true) + "/" + PermanentAchievementManager.ReturnTotalRewards();
            amountOfUnlocksDisplay.text = amountPrefix + PermanentAchievementManager.ReturnAmountOfAchievements(true) + "/" + PermanentAchievementManager.ReturnAmountOfAchievements();
        }
    }
}