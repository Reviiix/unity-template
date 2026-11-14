using UnityEditor;
using UnityEditor.SceneManagement;

namespace UnityTemplate.EditorTools
{
    /// <summary>Makes Play always start from Initialisation.unity (so services exist), returning to the open scene afterwards; toggle it in the Tools menu.</summary>
    [InitializeOnLoad]
    public static class PlayFromInitialisationScene
    {
        private const string MenuPath = "Tools/Scenes/Always Play From Initialisation Scene";
        private const string PreferenceKey = "UnityTemplate.PlayFromInitialisationScene";
        private const string InitialisationScenePath = "Assets/Scenes/Initialisation.unity";

        static PlayFromInitialisationScene()
        {
            EditorApplication.delayCall += Apply;
        }

        private static bool IsEnabled
        {
            get => EditorPrefs.GetBool(PreferenceKey, true);
            set => EditorPrefs.SetBool(PreferenceKey, value);
        }

        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            IsEnabled = !IsEnabled;
            Apply();
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, IsEnabled);
            return true;
        }

        private static void Apply()
        {
            EditorSceneManager.playModeStartScene = IsEnabled ? AssetDatabase.LoadAssetAtPath<SceneAsset>(InitialisationScenePath) : null;
        }
    }
}
