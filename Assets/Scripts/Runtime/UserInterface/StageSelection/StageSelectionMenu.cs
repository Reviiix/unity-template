using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Progression;
using UnityTemplate.Theming;
using UnityTemplate.Utilities;

namespace UnityTemplate.UserInterface.MainMenus.StageSelection
{
    /// <summary>The stage selection menu: one scrollable page of stage buttons per stage group, with next/previous group buttons.</summary>
    public class StageSelectionMenu : MenuBase
    {
        [SerializeField] private Button nextLevelGroupButton;
        [SerializeField] private Button previousLevelGroupButton;
        [SerializeField] private Transform levelItemParent;
        [SerializeField] private ScrollRect levelScroller;
        [SerializeField] private AssetReferenceGameObject stageItemPrefab;
        private readonly StringBuilder labelBuilder = new();
        private readonly List<(StageListItem Item, int Group, int Stage)> stageButtons = new();
        private AsyncOperationHandle<GameObject> stageItemHandle;
        private RectTransform[] groupPages = Array.Empty<RectTransform>();
        private int currentGroup;
        private ProgressService progress;
        private SaveService saveService;
        private ThemeService theme;

        /// <summary>Stops listening for RESET ALL and theme changes, and releases the stage item prefab when the menu is destroyed; the buttons use its sprites and font until then.</summary>
        private void OnDestroy()
        {
            if (saveService != null) saveService.DataLoaded -= RefreshStageButtons;
            if (theme != null) theme.Changed -= RefreshStageButtons;
            if (stageItemHandle.IsValid()) Addressables.Release(stageItemHandle);
        }

        /// <summary>Builds one page of stage buttons per group from the stage catalogue, then shows the first group.</summary>
        public async void Initialise()
        {
            progress = ServiceLocator.Get<ProgressService>();
            saveService = ServiceLocator.Get<SaveService>();
            theme = ServiceLocator.Get<ThemeService>();
            try
            {
                stageItemHandle = Addressables.LoadAssetAsync<GameObject>(stageItemPrefab);
                var itemPrefab = await stageItemHandle.ToAwaitable();
                if (stageItemHandle.Status != AsyncOperationStatus.Succeeded || itemPrefab == null)
                {
                    Debug.LogError("The stage item prefab could not be loaded; the stage selection menu stays empty. " + stageItemHandle.OperationException);
                    return;
                }
                BuildPages(itemPrefab);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return;
            }

            saveService.DataLoaded += RefreshStageButtons;
            theme.Changed += RefreshStageButtons;
            nextLevelGroupButton.onClick.AddListener(() => ShowGroup(currentGroup + 1));
            previousLevelGroupButton.onClick.AddListener(() => ShowGroup(currentGroup - 1));
            ShowGroup(0);
        }

        /// <summary>Creates one page per stage group with a labelled, wired button per stage.</summary>
        private void BuildPages(GameObject itemPrefab)
        {
            var catalogue = progress.Catalogue;
            var stageLoader = ServiceLocator.Get<StageLoader>();
            groupPages = new RectTransform[catalogue.GroupCount];

            for (var group = 0; group < catalogue.GroupCount; group++)
            {
                var page = (RectTransform)Instantiate(levelItemParent, levelItemParent.parent);
                groupPages[group] = page;

                for (var stage = 0; stage < catalogue.GetStageCount(group); stage++)
                {
                    var item = Instantiate(itemPrefab, page).GetComponentInChildren<StageListItem>();
                    item.transform.localScale = Vector3.one;
                    labelBuilder.Clear().Append(group).Append('-').Append(stage);
                    item.StageNameDisplay.SetText(labelBuilder);
                    stageButtons.Add((item, group, stage));

                    var selectedGroup = group;
                    var selectedStage = stage;
                    item.Button.onClick.AddListener(() => stageLoader.LoadStage(selectedGroup, selectedStage));
                }
            }

            Destroy(levelItemParent.gameObject);
            RefreshStageButtons();
        }

        /// <summary>Shows each stage button's locked state and rating, at start-up and again after RESET ALL.</summary>
        private void RefreshStageButtons()
        {
            foreach (var (item, group, stage) in stageButtons)
            {
                item.SetSelectable(CanSelect(progress, progress.Catalogue, group, stage));
                item.ShowRating(progress.GetRating(group, stage));
            }
        }

        /// <summary>Returns whether a stage button is tappable: it needs a scene and, outside development builds, must be unlocked.</summary>
        private static bool CanSelect(ProgressService progress, StageCatalogue catalogue, int group, int stage)
        {
            if (!catalogue.HasScene(group, stage)) return false;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return true;
#else
            return progress.IsUnlocked(group, stage);
#endif
        }

        /// <summary>Shows one group's page (wrapping around), scrolled to its first stage.</summary>
        private void ShowGroup(int group)
        {
            if (groupPages.Length == 0) return;

            currentGroup = (group % groupPages.Length + groupPages.Length) % groupPages.Length;
            for (var i = 0; i < groupPages.Length; i++)
            {
                groupPages[i].gameObject.SetActive(i == currentGroup);
            }
            levelScroller.content = groupPages[currentGroup];
            levelScroller.StopMovement();
            groupPages[currentGroup].anchoredPosition = Vector2.zero;
        }
    }
}
