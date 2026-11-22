using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityTemplate.Core;
using UnityTemplate.Progression;
using UnityTemplate.UserInterface;

namespace UnityTemplate.EditorTools
{
    /// <summary>Plays one stage from the Editor in one call (also in a batch-mode Editor): enters Play Mode from the boot scene, closes the start-up popups, loads the stage and logs when it shows; when Play Mode ends, the local save is put back as it was.</summary>
    [InitializeOnLoad]
    public static class StagePlayCheck
    {
        public const string LoadedLogPrefix = "StagePlayCheck: stage loaded";
        private const string GroupKey = "UnityTemplate.StagePlayCheck.Group";
        private const string StageKey = "UnityTemplate.StagePlayCheck.Stage";
        private const string RestoreSaveKey = "UnityTemplate.StagePlayCheck.RestoreSave";
        private const string PreviousStartSceneKey = "UnityTemplate.StagePlayCheck.PreviousStartScene";
        private const string InitialisationScenePath = "Assets/Scenes/Initialisation.unity";
        private const string SaveFileName = "save.json";
        private const string BackupFolderName = "StagePlayCheckSaveBackup";
        private const double TimeoutSeconds = 60;
        private static double deadline;
        private static bool loadRequested;
        private static bool isWaiting;

        private static string BackupFolder => Path.Combine(Application.temporaryCachePath, BackupFolderName);

        /// <summary>Hooks Play Mode changes, and resumes waiting if a domain reload interrupted a check.</summary>
        static StagePlayCheck()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            if (EditorApplication.isPlaying && SessionState.GetInt(GroupKey, -1) >= 0) StartWaiting();
        }

        /// <summary>Enters Play Mode and loads the given stage once the start-up popups are closed; logs "StagePlayCheck: stage loaded" when it shows. Exit Play Mode to finish: the save is restored.</summary>
        public static void Play(int group, int stage)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("StagePlayCheck: exit Play Mode first.");
                return;
            }

            BackUpSave();
            SessionState.SetBool(RestoreSaveKey, true);
            SessionState.SetString(PreviousStartSceneKey, AssetDatabase.GetAssetPath(EditorSceneManager.playModeStartScene));
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(InitialisationScenePath);
            SessionState.SetInt(GroupKey, group);
            SessionState.SetInt(StageKey, stage);
            EditorApplication.EnterPlaymode();
        }

        /// <summary>Tools menu entry: plays the first stage.</summary>
        [MenuItem("Tools/Scenes/Play First Stage (Skip Start-Up Popups)")]
        private static void PlayFirstStage()
        {
            Play(0, 0);
        }

        /// <summary>Starts waiting for the game when Play Mode starts, and restores the save when it ends.</summary>
        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode && SessionState.GetInt(GroupKey, -1) >= 0) StartWaiting();
            if (change != PlayModeStateChange.EnteredEditMode) return;

            StopWaiting();
            ClearPendingStage();
            if (!SessionState.GetBool(RestoreSaveKey, false)) return;
            RestoreSave();
            SessionState.EraseBool(RestoreSaveKey);
            var previousStartScene = SessionState.GetString(PreviousStartSceneKey, string.Empty);
            EditorSceneManager.playModeStartScene = previousStartScene.Length > 0 ? AssetDatabase.LoadAssetAtPath<SceneAsset>(previousStartScene) : null;
            SessionState.EraseString(PreviousStartSceneKey);
        }

        /// <summary>Begins checking the game every Editor update until the stage shows or the check times out.</summary>
        private static void StartWaiting()
        {
            if (isWaiting) return;
            isWaiting = true;
            loadRequested = false;
            deadline = EditorApplication.timeSinceStartup + TimeoutSeconds;
            EditorApplication.update += Step;
        }

        /// <summary>Stops the per-update check.</summary>
        private static void StopWaiting()
        {
            isWaiting = false;
            EditorApplication.update -= Step;
        }

        /// <summary>Closes open popups until the start-up popups are done, then loads the stage and reports once it shows.</summary>
        private static void Step()
        {
            var group = SessionState.GetInt(GroupKey, -1);
            var stage = SessionState.GetInt(StageKey, -1);
            if (!EditorApplication.isPlaying || group < 0)
            {
                StopWaiting();
                return;
            }
            if (EditorApplication.timeSinceStartup > deadline)
            {
                StopWaiting();
                ClearPendingStage();
                Debug.LogError($"StagePlayCheck: stage {group}-{stage} didn't load within {TimeoutSeconds} seconds.");
                return;
            }
            if (Time.frameCount < 3 || !ServiceLocator.TryGet<UserInterfaceManager>(out var userInterface) || !ServiceLocator.TryGet<StageLoader>(out var stageLoader)) return;

            foreach (var popUp in Object.FindObjectsByType<PopUpWindow>(FindObjectsSortMode.None)) popUp.PressBack();
            if (userInterface.IsShowingStartUpPopUps) return;

            if (!loadRequested)
            {
                if (!stageLoader.CanLoadStage(group, stage))
                {
                    StopWaiting();
                    ClearPendingStage();
                    Debug.LogError($"StagePlayCheck: stage {group}-{stage} can't be loaded (no scene in the stage catalogue, or a stage is already open).");
                    return;
                }
                stageLoader.LoadStage(group, stage);
                loadRequested = true;
                return;
            }
            if (!stageLoader.IsInStage || stageLoader.IsLoading) return;

            StopWaiting();
            ClearPendingStage();
            Debug.Log($"{LoadedLogPrefix}: {group}-{stage}. Exit Play Mode when done; the save will be restored.");
        }

        /// <summary>Forgets the stage waiting to be loaded.</summary>
        private static void ClearPendingStage()
        {
            SessionState.EraseInt(GroupKey);
            SessionState.EraseInt(StageKey);
        }

        /// <summary>Copies the save files aside; a backup left by an interrupted check is restored first, since it holds the real save.</summary>
        private static void BackUpSave()
        {
            if (Directory.Exists(BackupFolder)) RestoreSave();
            Directory.CreateDirectory(BackupFolder);
            foreach (var path in SaveFiles()) File.Copy(path, Path.Combine(BackupFolder, Path.GetFileName(path)));
        }

        /// <summary>Deletes the save Play Mode wrote and copies the backed-up files back, leaving no save if there was none.</summary>
        private static void RestoreSave()
        {
            if (!Directory.Exists(BackupFolder)) return;
            foreach (var path in SaveFiles()) File.Delete(path);
            foreach (var path in Directory.GetFiles(BackupFolder)) File.Copy(path, Path.Combine(Application.persistentDataPath, Path.GetFileName(path)));
            Directory.Delete(BackupFolder, true);
        }

        /// <summary>Returns the save files (the save, its backup and any unfinished temporary file).</summary>
        private static string[] SaveFiles()
        {
            var folder = Application.persistentDataPath;
            return Directory.Exists(folder) ? Directory.GetFiles(folder, SaveFileName + "*") : new string[0];
        }
    }
}
