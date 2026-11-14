using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace UnityTemplate.EditorTools
{
    /// <summary>Makes Play always start from Initialisation.unity (so services exist), returning to the open scene afterwards; toggle it in the Tools menu.</summary>
    [InitializeOnLoad]
    public static class PlayFromInitialisationScene
    {
        private const string MenuPath = "Tools/Scenes/Always Play From Initialisation Scene";
        private const string PreferenceKey = "UnityTemplate.PlayFromInitialisationScene";
        private const string TestsRunningKey = "UnityTemplate.TestsRunning";
        private const string InitialisationScenePath = "Assets/Scenes/Initialisation.unity";

        static PlayFromInitialisationScene()
        {
            ScriptableObject.CreateInstance<TestRunnerApi>().RegisterCallbacks(new TestRunCallbacks());
            if (!EditorApplication.isPlayingOrWillChangePlaymode) EditorApplication.delayCall += Apply;
        }

        private static bool IsEnabled
        {
            get => EditorPrefs.GetBool(PreferenceKey, true);
            set => EditorPrefs.SetBool(PreferenceKey, value);
        }

        /// <summary>Switches the option on or off from the Tools menu.</summary>
        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            IsEnabled = !IsEnabled;
            Apply();
        }

        /// <summary>Shows a tick next to the menu item while the option is on.</summary>
        [MenuItem(MenuPath, true)]
        private static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, IsEnabled);
            return true;
        }

        /// <summary>Points Unity's play-mode start scene at Initialisation.unity, or clears it in batch mode, during test runs or when switched off.</summary>
        private static void Apply()
        {
            var useInitialisationScene = IsEnabled && !Application.isBatchMode && !SessionState.GetBool(TestsRunningKey, false);
            EditorSceneManager.playModeStartScene = useInitialisationScene ? AssetDatabase.LoadAssetAtPath<SceneAsset>(InitialisationScenePath) : null;
        }

        /// <summary>Steps aside while the Test Runner runs, because PlayMode tests must start from the Test Runner's own scene.</summary>
        private class TestRunCallbacks : ICallbacks
        {
            /// <summary>Clears the start scene before the run enters Play mode.</summary>
            public void RunStarted(ITestAdaptor testsToRun)
            {
                SessionState.SetBool(TestsRunningKey, true);
                EditorSceneManager.playModeStartScene = null;
            }

            /// <summary>Restores the start scene once the run has finished.</summary>
            public void RunFinished(ITestResultAdaptor result)
            {
                SessionState.SetBool(TestsRunningKey, false);
                Apply();
            }

            /// <summary>Not needed: the start scene only matters for the whole run.</summary>
            public void TestStarted(ITestAdaptor test)
            {
            }

            /// <summary>Not needed: the start scene only matters for the whole run.</summary>
            public void TestFinished(ITestResultAdaptor result)
            {
            }
        }
    }
}
