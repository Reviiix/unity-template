using System.Collections;
using System.Collections.Generic;
using PureFunctions;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Achievements.Display.ListDisplay
{
    public class AchievementListDisplay : MonoBehaviour
    {
        [SerializeField] private Transform display;
        private static readonly AssetReference DisplayItemPrefab = new AssetReference("Assets/Prefabs/UserInterface/Achievements/AchievementListDisplayItem.prefab");
        private static readonly WaitUntil WaitUntilAssetReferenceIsLoadedAsynchronously = new WaitUntil(() => _addressableAsGameObject != null);
        private static GameObject _addressableAsGameObject;
        private readonly Dictionary<AchievementManager.Achievement, AchievementItemDisplay> _achievementListItems = new Dictionary<AchievementManager.Achievement, AchievementItemDisplay>();
        private static readonly Color LockedColour = new Color(1, 1, 1, 0.5f);
        private static readonly Color UnLockedColour = Color.white;

        private void OnEnable()
        {
            AchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
            StartCoroutine(Wait.WaitForAnyAsynchronousInitialisationToComplete(()=>
            {
                StartCoroutine(CreateDisplayAsynchronously());
            }));
        }

        private void OnDisable()
        {
            AchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
        }

        private void OnAchievementUnlocked(AchievementManager.Achievement achievement)
        {
            SetUnlockedState(achievement);
        }

        private IEnumerator CreateDisplayAsynchronously()
        {
            var achievements = AchievementManager.ReturnAllAchievements();
            var amountOfAchievements = achievements.Length;
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<GameObject>(DisplayItemPrefab, (returnVariable) =>
            {
                _addressableAsGameObject = returnVariable;
            });
            yield return WaitUntilAssetReferenceIsLoadedAsynchronously;
            for (var i = 0; i < amountOfAchievements; i++)
            {
                var achievement = achievements[i];
                CreateDisplayItemAsynchronously(achievement, AchievementManager.ReturnDescription(achievement), AchievementManager.ReturnReward(achievement), AchievementManager.ReturnGraphic(achievement), AchievementManager.ReturnUnlockedState(achievement));
            }
        }

        private void CreateDisplayItemAsynchronously(AchievementManager.Achievement itemName, string description, int reward, IKeyEvaluator sprite, bool unlocked)
        {
            var achievementItemGameObject = Instantiate(_addressableAsGameObject, display).GetComponentInChildren<AchievementListItemDisplayObject>();
            var achievementItemGameObjectTransform = achievementItemGameObject.transform;
            achievementItemGameObjectTransform.SetParent(display);
            achievementItemGameObjectTransform.localScale = Vector3.one;

            _achievementListItems.Add(itemName, new AchievementItemDisplay(achievementItemGameObject.ReturnNameDisplay(), achievementItemGameObject.ReturnDescriptionDisplay(), achievementItemGameObject.ReturnRewardDisplay(), achievementItemGameObject.ReturnGraphic(), unlocked));
            
            _achievementListItems[itemName].NameDisplay.text = itemName.ToString();
            _achievementListItems[itemName].DescriptionDisplay.text = description;
            _achievementListItems[itemName].RewardDisplay.text = reward.ToString();

            if (unlocked) SetUnlockedState(itemName);
            
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<Sprite>(sprite, (returnSprite) =>
            {
                _achievementListItems[itemName].Graphic.sprite = returnSprite;
                achievementItemGameObject.RemoveClassFromObject();
            });
        }

        private void SetUnlockedState(AchievementManager.Achievement achievement)
        {
            _achievementListItems[achievement].Graphic.color = Color.white;
        }
        
        private readonly struct AchievementItemDisplay
        {
            private static readonly Color LockedColourCache = LockedColour;
            private static readonly Color UnLockedColourCache = UnLockedColour;
            public readonly TMP_Text NameDisplay;
            public readonly TMP_Text DescriptionDisplay;
            public readonly TMP_Text RewardDisplay;
            public readonly Image Graphic;

            public AchievementItemDisplay(TMP_Text nameDisplay, TMP_Text descriptionDisplay, TMP_Text rewardDisplay, Image graphic, bool unlocked)
            {
                NameDisplay = nameDisplay;
                DescriptionDisplay = descriptionDisplay;
                RewardDisplay = rewardDisplay;
                Graphic = graphic;
                Graphic.color = LockedColourCache;
                if (unlocked) Graphic.color = UnLockedColourCache;
            }
        }
    }
}