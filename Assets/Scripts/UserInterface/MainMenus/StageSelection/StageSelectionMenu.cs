using System;
using System.Collections;
using System.Collections.Generic;
using PureFunctions;
using PureFunctions.UnitySpecific;
using Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UserInterface.MainMenus.StageSelection
{
    /// <summary>
    /// This is the stage selection menu
    /// </summary>
    [Serializable]
    public class StageSelectionMenu : UserInterface, IUserInterface
    {
        [SerializeField] private Button nextLevelGroupButton, previousLevelGroupButton;
        [SerializeField] private Transform levelItemParent;
        [SerializeField] private ScrollRect levelScroller;
        private static readonly AssetReference DisplayItemPrefab = new AssetReference("Prefabs/UserInterface/StageDisplayItem.prefab");
        private static readonly WaitUntil WaitUntilAssetReferenceIsLoadedAsynchronously = new WaitUntil(() => _addressableAsGameObject != null);
        private static GameObject _addressableAsGameObject;
        private static RectTransform[] _levelGroupDisplays;
        private static LevelItemDisplay[] _levelListItems;
        private static int _amountOfLevelGroups = StageLoadManager.AmountOfStageGroups;
        private static int _currentLeveGroupDisplayIndex;
        private static int CurrentLeveGroupDisplayIndex
        {
            get => _currentLeveGroupDisplayIndex;
            set
            {
                _currentLeveGroupDisplayIndex = value;
                
                if (_currentLeveGroupDisplayIndex >= _amountOfLevelGroups) _currentLeveGroupDisplayIndex = 0;

                if (_currentLeveGroupDisplayIndex < 0) _currentLeveGroupDisplayIndex = _amountOfLevelGroups-1;
            }
        }

        public void Initialise()
        {
            Coroutiner.StartCoroutine(CreateDisplayAsynchronously());
        }

        public void Enable(bool state = true)
        {
            display.enabled = state;
        }

        private IEnumerator CreateDisplayAsynchronously()
        {
            var currentLevelIndex = 0;
            var levelGroupDisplays = new List<RectTransform>();
            var parentCache = levelItemParent;
            var mostRelevantLevelUnlockedIndex = GameStatistics.ReturnMostRecentUnlockedLevel();

            AssetReferenceLoader.LoadAssetReferenceAsynchronously<GameObject>(DisplayItemPrefab, (returnVariable) =>
            {
                _addressableAsGameObject = returnVariable;
                AssetReferenceLoader.DestroyOrUnload(returnVariable);
            });
            
            yield return WaitUntilAssetReferenceIsLoadedAsynchronously;
            
            _levelListItems = new LevelItemDisplay[StageLoadManager.TotalAmountOfStages];

            for (var groupIndex = 0; groupIndex < _amountOfLevelGroups; groupIndex++)
            {
                var amountOfLevels = StageLoadManager.GetAmountOfStagesIn(groupIndex);
                
                levelItemParent = Object.Instantiate(parentCache, levelItemParent.parent);
                levelGroupDisplays.Add(levelItemParent.GetComponent<RectTransform>());

                for (var levelIndex = 0; levelIndex < amountOfLevels; levelIndex++)
                {
                    CreateDisplayItemAsynchronously(groupIndex + "-" + levelIndex, groupIndex, levelIndex, currentLevelIndex > mostRelevantLevelUnlockedIndex);
                    currentLevelIndex++;
                }
            }

            AssetReferenceLoader.DestroyOrUnload(parentCache.gameObject);
            levelItemParent = null;
            
            if (_amountOfLevelGroups > 0)
            {
                _levelGroupDisplays = levelGroupDisplays.ToArray();
                AssignLevelGroupNavigationButtonEvents();
                ChangeLevelGroup(0);
            }
        }

        private void CreateDisplayItemAsynchronously(string description,int levelGroupIndex, int levelIndex, bool locked)
        {
            var levelItemGameObject = Object.Instantiate(_addressableAsGameObject, levelItemParent).GetComponentInChildren<StageListItem>();
            var levelItemGameTransform = levelItemGameObject.transform;
            levelItemGameTransform.SetParent(levelItemParent);
            levelItemGameTransform.localScale = Vector3.one;

            _levelListItems[levelIndex] = new LevelItemDisplay(levelItemGameObject.ReturnButton(), levelItemGameObject.ReturnStageNameDisplay(), levelItemGameObject.ReturnStarDisplay());
            _levelListItems[levelIndex].LoadLevelButton.onClick.AddListener(()=>StageLoadManager.LoadSpecificStage(levelGroupIndex, levelIndex));
            _levelListItems[levelIndex].LevelNameDisplay.text = description;
            _levelListItems[levelIndex].LoadLevelButton.interactable = !locked;
            #if UNITY_EDITOR || DEBUG_BUILD
            _levelListItems[levelIndex].LoadLevelButton.interactable = true;
            #endif
            foreach (var star in _levelListItems[levelIndex].Stars)
            {
                star.material.color = Color.white;
            }
            levelItemGameObject.RemoveClassFromObject();
        }
        
        private void AssignLevelGroupNavigationButtonEvents()
        {
            nextLevelGroupButton.onClick.AddListener(DisplayNextLevelGroup);
            previousLevelGroupButton.onClick.AddListener(DisplayPreviousLevelGroup);
        }

        private void DisplayNextLevelGroup()
        {
            ChangeLevelGroup(CurrentLeveGroupDisplayIndex++);
        }
        
        private void DisplayPreviousLevelGroup()
        {
            ChangeLevelGroup(CurrentLeveGroupDisplayIndex--);
        }

        private void ChangeLevelGroup(int index)
        {
            EnableAllLevelGroupDisplays(false);
            _levelGroupDisplays[index].gameObject.SetActive(true);
            levelScroller.content = _levelGroupDisplays[index];
        }

        private void EnableAllLevelGroupDisplays(bool state = true)
        {
            foreach (var levelGroupDisplay in _levelGroupDisplays)
            {
                levelGroupDisplay.gameObject.SetActive(state);
            }
        }

        private readonly struct LevelItemDisplay
        {
            public readonly Button LoadLevelButton;
            public readonly TMP_Text LevelNameDisplay;
            public readonly Image[] Stars;

            public LevelItemDisplay(Button loadLevelButton, TMP_Text levelNameDisplay, Image[] stars)
            {
                LoadLevelButton = loadLevelButton;
                LevelNameDisplay = levelNameDisplay;
                Stars = stars;
            }
        }
    }
}