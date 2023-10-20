using System;
using System.IO;
using System.Linq;
using Credits;
using Statistics.Experience;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    public class EditorTools : UnityEditor.Editor
    {
        #region Menu Item Paths
        private const string ToolPath = "Tools/";
        private const string ScenePath = ToolPath + "Scenes/";
        private const string SavePath = ToolPath + "SaveInGameData/";
        private const string GameStatisticsPath = ToolPath + "GameStatistics/";
        #endregion Menu Item Paths

        [MenuItem(ScenePath + nameof(LoadInitialisationScene))]
        public static void LoadInitialisationScene()
        {
            EditorSceneManager.OpenScene(StageLoadManager.InitialisationStageFilePath);
        }
        
        public class GameSceneSelectWindow : EditorWindow
        {
            private int selectedScene;
	
            [MenuItem(ScenePath + nameof(OpenSceneSelect))]
            public static void OpenSceneSelect()
            {
                GetWindowWithRect(typeof(GameSceneSelectWindow), new Rect(Screen.width / 2f, Screen.height / 2f, 500, 100));
            }

            private void OnGUI()
            {
                const string assetFilter = "t:Scene";
                const string loadSceneButtonName = "Load Scene";
                const string loadSceneWithDebugButtonName = loadSceneButtonName + " With Debug";
                var scenePaths = AssetDatabase.FindAssets(assetFilter, new[] {StageLoadManager.StageFilePathPrefix}).Select(AssetDatabase.GUIDToAssetPath).ToArray();
                if (GUILayout.Button(loadSceneButtonName))
                {
                    EditorSceneManager.OpenScene(scenePaths[selectedScene]);
                }

                if (GUILayout.Button(loadSceneWithDebugButtonName))
                {
                    EditorSceneManager.OpenScene(scenePaths[selectedScene]);
                }
                selectedScene = EditorGUILayout.Popup(selectedScene, scenePaths.Select(Path.GetFileName).ToArray());
            }
        }
        
        [MenuItem(SavePath + nameof(SaveData))]
        public static void SaveData()
        {
            SaveSystem.Save();
        }
        
        [MenuItem(SavePath + nameof(DeleteSaveData))]
        public static void DeleteSaveData()
        {
            SaveSystem.Delete();
            SaveData();
        }

        [MenuItem(GameStatisticsPath + "/" + nameof(ResetDailyWheel))]
        public static void ResetDailyWheel()
        {
            SaveData();
            throw new NotImplementedException();
        }
        
        [MenuItem(GameStatisticsPath + nameof(Add100Credits))]
        public static void Add100Credits()
        {
            CreditsManager.AddCredits(CreditsManager.Currency.Credits, 100);
            SaveData();
        }
        
        [MenuItem(GameStatisticsPath + nameof(Add100PremiumCredits))]
        public static void Add100PremiumCredits()
        {
            CreditsManager.AddCredits(CreditsManager.Currency.PremiumCredits, 100);
            SaveData();
        }
        
        [MenuItem(GameStatisticsPath + nameof(Add100Experience))]
        public static void Add100Experience()
        {
            ExperienceManager.AddExperience(100);
            SaveData();
        }
    }
}