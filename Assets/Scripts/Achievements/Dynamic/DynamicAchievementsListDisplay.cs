using System.Collections;
using System.Collections.Generic;
using Achievements.Display.ListDisplay;
using PureFunctions;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DynamicAchievementsListDisplay : AchievementListDisplay
{
    private const string DisplayItemPrefabPath = "Prefabs/UserInterface/Achievements/AchievementListItem.prefab";
    private static readonly AssetReference DisplayItemPrefab = new AssetReference(DisplayItemPrefabPath);
    private readonly Dictionary<DynamicAchievementManager.Achievement, AchievementItemStruct> _achievementListItems = new Dictionary<DynamicAchievementManager.Achievement, AchievementItemStruct>();

    private void OnEnable()
    {
        DynamicAchievementManager.OnAchievementsSet += OnAchievementsSet;
        DynamicAchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
    }
    
    private void OnDisable()
    {
        DynamicAchievementManager.OnAchievementsSet -= OnAchievementsSet;
        DynamicAchievementManager.OnAchievementUnlocked -= OnAchievementUnlocked;
    }
    
    private void OnAchievementUnlocked(DynamicAchievementManager.Achievement achievement)
    {
        SetUnlockedState(achievement);
        SetRewardDisplay();
    }

    private void OnAchievementsSet(DynamicAchievementManager.Achievement[] achievements)
    {
        StartCoroutine(CreateListAsynchronously(achievements));
        SetRewardDisplay();
    }

    private IEnumerator CreateListAsynchronously(DynamicAchievementManager.Achievement[] achievements)
    {
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
            CreateListItemAsynchronously(achievement, DynamicAchievementManager.ReturnDescription(achievement), DynamicAchievementManager.ReturnReward(achievement), DynamicAchievementManager.ReturnSpriteAssetReference(achievement), DynamicAchievementManager.ReturnUnlockedState(achievement));
        }

        DisplayItemAddressableAsGameObject = null;
        ListCreated = true;
    }

    private void CreateListItemAsynchronously(DynamicAchievementManager.Achievement itemName, string description, int reward, AssetReference sprite, bool unlocked)
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
    
    private void SetRewardDisplay()
    {
        const string rewardsPrefix = "Total Rewards: ";
        const string amountPrefix = "Total Achievements: ";
        rewardDisplay.text = rewardsPrefix + DynamicAchievementManager.ReturnTotalRewards(true) + "/" + DynamicAchievementManager.ReturnTotalRewards();
        amountOfUnlocksDisplay.text = amountPrefix + DynamicAchievementManager.ReturnAmountOfAchievements(true) + "/" + DynamicAchievementManager.ReturnAmountOfAchievements();
    }
    
    private void SetUnlockedState(DynamicAchievementManager.Achievement achievement)
    {
        if (!ListCreated) return; //Sometimes an achievement for opening the app x times will try to set this before the list is created.
        
        _achievementListItems[achievement].Background.color = UnLockedColour;
    }
}
