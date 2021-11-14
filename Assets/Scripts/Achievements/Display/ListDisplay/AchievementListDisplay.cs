using System.Collections;
using System.Collections.Generic;
using Abstract;
using PureFunctions;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Achievements.Display.ListDisplay
{
    [RequireComponent(typeof(Transform))]
    public class AchievementListDisplay : Singleton<AchievementListDisplay>
    {
        private Transform _display;
        private static bool _listCreated;
        private const string DisplayItemPrefabPath = "Prefabs/UserInterface/Achievements/AchievementListItem.prefab";
        private static readonly AssetReference DisplayItemPrefab = new AssetReference(DisplayItemPrefabPath);
        private static GameObject _displayItemAddressableAsGameObject;
        private static readonly WaitUntil WaitUntilAssetReferenceIsLoadedAsynchronously = new WaitUntil(() => _displayItemAddressableAsGameObject != null);
        private readonly Dictionary<AchievementManager.Achievement, AchievementItemStruct> _achievementListItems = new Dictionary<AchievementManager.Achievement, AchievementItemStruct>();
        private static readonly Color UnLockedColour = Color.yellow;
        [SerializeField] private TMP_Text rewardDisplay;
        [SerializeField] private TMP_Text amountOfAchievementsDisplay;

        private void Awake()
        {
            _display = GetComponent<Transform>();
            StartCoroutine(ProjectManager.WaitForAnyAsynchronousInitialisationToComplete(CreateDisplayAsynchronously));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            AchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            AchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
        }

        private void OnAchievementUnlocked(AchievementManager.Achievement achievement)
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
            var achievements = AchievementManager.ReturnAllAchievements();
            var amountOfAchievements = achievements.Length;
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<GameObject>(DisplayItemPrefab, (returnVariable) =>
            {
                _displayItemAddressableAsGameObject = returnVariable;
                AssetReferenceLoader.DestroyOrUnload(returnVariable);
            });
            yield return WaitUntilAssetReferenceIsLoadedAsynchronously;
            for (var i = 0; i < amountOfAchievements; i++)
            {
                var achievement = achievements[i];
                CreateListItemAsynchronously(achievement, AchievementManager.ReturnDescription(achievement), AchievementManager.ReturnReward(achievement), AchievementManager.ReturnSpriteAssetReference(achievement), AchievementManager.ReturnUnlockedState(achievement));
            }

            _displayItemAddressableAsGameObject = null;
            _listCreated = true;
        }

        private void CreateListItemAsynchronously(AchievementManager.Achievement itemName, string description, int reward, AssetReference sprite, bool unlocked)
        {
            var achievementItemGameObject = Instantiate(_displayItemAddressableAsGameObject, _display).GetComponentInChildren<AchievementListItem>();
            var achievementItemGameObjectTransform = achievementItemGameObject.transform;
            
            achievementItemGameObjectTransform.SetParent(_display);
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

        private void SetUnlockedState(AchievementManager.Achievement achievement)
        {
            if (!_listCreated) return; //Sometimes an achievement for opening the app x times will try to set this before the list is created.
            
            _achievementListItems[achievement].Background.color = UnLockedColour;
        }

        private void SetRewardDisplay()
        {
            const string rewardsPrefix = "Total Rewards: ";
            const string amountPrefix = "Total Achievements: ";
            rewardDisplay.text = rewardsPrefix + AchievementManager.ReturnTotalRewards(true) + "/" +AchievementManager.ReturnTotalRewards();
            amountOfAchievementsDisplay.text = amountPrefix + AchievementManager.ReturnAmountOfAchievements(true) + "/" + AchievementManager.ReturnAmountOfAchievements();
        }
        
        private readonly struct AchievementItemStruct
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
}