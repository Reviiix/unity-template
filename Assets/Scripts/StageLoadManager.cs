using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PureFunctions.UnitySpecific;
using PureFunctions.UnitySpecific.Effects;
using Statistics;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UserInterface;

/// <summary>
/// This class handles loading different stages.
/// Stages (scenes) should be asset references.
/// </summary>
public static class StageLoadManager
{
    private static KeyValuePair<int, int> _furthestLevelIndex = GameStatistics.FurthestLevelIndex;
    public static int GetStageIndex(int stageGroup, int stage) => StageConfiguration.GetAssetReferenceIndex(stageGroup, stage);
    private static AssetReference GetStageAsAssetReference(int stageGroup, int stage) => StageConfiguration.GetAssetReferenceForStage(stageGroup, stage);
    public const string InitialisationStageFilePath = StageConfiguration.InitialisationSceneFilePath;
    public const string StageFilePathPrefix = StageConfiguration.FilePathPrefix;
    
    public static readonly int TotalAmountOfStages = StageConfiguration.GetAmountOfAssetReferences;
    public static readonly int AmountOfStageGroups = StageConfiguration.GetAmountOfGroups();
    public static int GetAmountOfStagesIn(int groupIndex) => StageConfiguration.GetAmountOfStagesInGroup(groupIndex);
    
    public static void LoadLatestUnlockedStage()
    {
        LoadSpecificStage(_furthestLevelIndex.Key, _furthestLevelIndex.Value);
    }
    
    public static void LoadTutorial()
    {
        LoadSpecificStage(0, 0);
    }
    
    public static void LoadSpecificStage(int stageGroup, int stage)
    {
        Coroutiner.StartCoroutine(FadeInNewStage(stageGroup, stage,() =>
        {
            //Fade complete.
            UserInterfaceManager.EnableBackground(false);
            UserInterfaceManager.EnableAllNonPermanentCanvasesStaticCall(false);
        }, () =>
        {
            //Fade and load complete.
            UserInterfaceManager.EnableHeadsUpDisplay();
        }));
    }

    private static IEnumerator FadeInNewStage(int stageGroup, int stage, Action fadeCompleteCallBack = null, Action completeCallBack = null)
    {
        return StageLoader.LoadStageWithFade(stageGroup,  stage, fadeCompleteCallBack, completeCallBack);
    }

    private readonly struct StageLoader
    {
        #region Fade
        private static RawImage FadeImage => UserInterfaceManager.TransitionalFadeImage;
        private const int TotalFadeInAndOutDuration = 3;
        private const int FadeDuration = TotalFadeInAndOutDuration / 2; //2 because fade in then fade out
        #endregion Fade
            
        public static IEnumerator LoadStageWithFade(int stageGroup, int stage, Action fadeCompleteCallBack = null, Action completeCallBack = null)
        {
            yield return Coroutiner.StartCoroutine(Fade.FadeImageAlphaUp(FadeImage, FadeDuration)); 
            
            fadeCompleteCallBack?.Invoke();
            
            AssetReferenceLoader.LoadScene(GetStageAsAssetReference(stageGroup, stage)); //Should i do a wait here or does the fade disquise it??
            
            yield return Coroutiner.StartCoroutine(Fade.FadeImageAlphaDown(FadeImage, FadeDuration));
            
            completeCallBack?.Invoke();
        }
    }

    private readonly struct StageConfiguration
    {
        public const string InitialisationSceneFilePath = "Assets/Scenes/Initialisation.unity";
        public const string FilePathPrefix = "Assets/Scenes/";
        private const string FilePathSuffix = ".unity";
        private const string LevelGroupOneFilePath = FilePathPrefix + "LevelGroupOne/0-";
        private const string LevelGroupTwoFilePath = FilePathPrefix + "LevelGroupTwo/1-";
        private static readonly StageGroup[] StageGroups =
        {
            new StageGroup(new []
            {
                new AssetReference(LevelGroupOneFilePath + "0(Tutorial)" + FilePathSuffix),
                new AssetReference(LevelGroupOneFilePath + "1" + FilePathSuffix),
                new AssetReference(LevelGroupOneFilePath + "2" + FilePathSuffix),
                new AssetReference(LevelGroupOneFilePath + "3" + FilePathSuffix),
                new AssetReference(LevelGroupOneFilePath + "4" + FilePathSuffix),
                new AssetReference(LevelGroupOneFilePath + "5" + FilePathSuffix),
                new AssetReference(LevelGroupOneFilePath + "6" + FilePathSuffix),
            }),
            new StageGroup(new []
            {
                new AssetReference(LevelGroupTwoFilePath + "0(Tutorial)" + FilePathSuffix),
                new AssetReference(LevelGroupTwoFilePath + "1" + FilePathSuffix),
                new AssetReference(LevelGroupTwoFilePath + "2" + FilePathSuffix),
                new AssetReference(LevelGroupTwoFilePath + "3" + FilePathSuffix),
                new AssetReference(LevelGroupTwoFilePath + "4" + FilePathSuffix),
                new AssetReference(LevelGroupTwoFilePath + "5" + FilePathSuffix),
                new AssetReference(LevelGroupTwoFilePath + "6" + FilePathSuffix),
            }),
        };
        public static AssetReference GetAssetReferenceForStage(int stageGroup, int stage) => StageGroups[stageGroup].Stages[stage];
        public static int GetAmountOfGroups() => StageGroups.Length;
        public static int GetAmountOfStagesInGroup(int groupIndex) => StageGroups[groupIndex].Stages.Length;
        public static int GetAmountOfAssetReferences => ReturnAllAssetReferences().Length;

        public static int GetAssetReferenceIndex(int stageGroup, int currentStageGroupStage)
        {
            var mountOfStagesInCompletedWorlds = 0;
            for (var i = 0; i < stageGroup; i++)
            {
                if (stageGroup != i) mountOfStagesInCompletedWorlds += StageGroups[i].Stages.Length;
            }
            return mountOfStagesInCompletedWorlds + currentStageGroupStage;
        }

        private static AssetReference[] ReturnAllAssetReferences()
        {
            var returnVariable = new List<AssetReference>();
            foreach (var stageGroup in StageGroups)
            {
                returnVariable = returnVariable.Concat(stageGroup.Stages).ToList();
            }
            return returnVariable.ToArray();
        }

        private readonly struct StageGroup
        {
            public readonly AssetReference[] Stages;

            public StageGroup(AssetReference[] stages)
            {
                Stages = stages;
            }
        }
    }
}