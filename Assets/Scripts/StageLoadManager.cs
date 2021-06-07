using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using PureFunctions.Effects;
using Statistics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UserInterface;

public static class StageLoadManager
{
    private static KeyValuePair<int, int> _furthestLevelIndex = GameStatistics.FurthestLevelIndex;
    private static MonoBehaviour CoRoutineHandler => ProjectManager.Instance;
    public static int ReturnStageIndex(int stageGroup, int stage) => StageConfiguration.ReturnAssetReferenceIndex(stageGroup, stage);
    private static AssetReference ReturnStageAsAssetReference(int stageGroup, int stage) => StageConfiguration.ReturnAssetReferenceForStage(stageGroup, stage);
    public static int ReturnTotalAmountOfStages => StageConfiguration.ReturnAmountOfAssetReferences;
    public static int ReturnAmountOfStageGroups => StageConfiguration.ReturnAmountOfGroups();
    public static int ReturnAmountOfStagesIn(int groupIndex) => StageConfiguration.ReturnAmountOfStagesInGroup(groupIndex);
    
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
        CoRoutineHandler.StartCoroutine(FadeInNewStage(stageGroup, stage,() =>
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
        private const int FadeDuration = TotalFadeInAndOutDuration / 2;
        #endregion Fade
            
        public static IEnumerator LoadStageWithFade(int stageGroup, int stage, Action fadeCompleteCallBack = null, Action completeCallBack = null)
        {
            yield return CoRoutineHandler.StartCoroutine(Fade.FadeImageAlphaUp(FadeImage, FadeDuration)); 
            
            fadeCompleteCallBack?.Invoke();
            
            AssetReferenceLoader.LoadScene(ReturnStageAsAssetReference(stageGroup, stage)); //Should i do a wait here or does the fade disquise it??
            
            yield return CoRoutineHandler.StartCoroutine(Fade.FadeImageAlphaDown(FadeImage, FadeDuration));
            
            completeCallBack?.Invoke();
        }
    }

    private readonly struct StageConfiguration
    {
        private static readonly StageGroup[] StageGroups =
        {
            //World 1
            new StageGroup(new []
            {
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
            }),
            //World 2
            new StageGroup(new []
            {
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
            }),
            //World 3
            new StageGroup(new []
            {
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
                new AssetReference("Assets/Scenes/StageGroupOne/0-0(Tutorial).unity"),
            })
        };
        public static AssetReference ReturnAssetReferenceForStage(int stageGroup, int stage) => StageGroups[stageGroup].Stages[stage];
        public static int ReturnAmountOfGroups() => StageGroups.Length;
        public static int ReturnAmountOfStagesInGroup(int groupIndex) => StageGroups[groupIndex].Stages.Length;
        public static int ReturnAmountOfAssetReferences => ReturnAllAssetReferences().Length;

        public static int ReturnAssetReferenceIndex(int stageGroup, int currentStageGroupStage)
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