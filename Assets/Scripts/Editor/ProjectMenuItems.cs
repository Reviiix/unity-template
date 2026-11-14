using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityTemplate.Core;
using UnityTemplate.Economy;
using UnityTemplate.Progression;

namespace UnityTemplate.EditorTools
{
    /// <summary>Tools menu shortcuts for opening scenes and editing the local save while developing.</summary>
    public static class ProjectMenuItems
    {
        private const string ScenesMenu = "Tools/Scenes/";
        private const string SaveMenu = "Tools/Save Data/";
        private const string CheatsMenu = "Tools/Cheats/";
        private const string InitialisationScenePath = "Assets/Scenes/Initialisation.unity";
        private const string SaveFileName = "save.json";

        /// <summary>Opens the boot scene, asking to save changes to the open scenes first.</summary>
        [MenuItem(ScenesMenu + "Open Initialisation Scene")]
        public static void OpenInitialisationScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorSceneManager.OpenScene(InitialisationScenePath);
        }

        /// <summary>Deletes the local save: through the running game in Play mode, or the file itself in Edit mode.</summary>
        [MenuItem(SaveMenu + "Delete Save Data")]
        public static void DeleteSaveData()
        {
            if (!EditorUtility.DisplayDialog("Delete save data", "Delete the local save file? This cannot be undone.", "Delete", "Cancel")) return;

            if (Application.isPlaying)
            {
                ServiceLocator.Get<SaveService>().DeleteAll();
                return;
            }
            foreach (var path in Directory.GetFiles(Application.persistentDataPath, SaveFileName + "*")) File.Delete(path);
            Debug.Log("Save data deleted.");
        }

        /// <summary>Makes today's daily bonus claimable again by editing the save file (Edit mode only).</summary>
        [MenuItem(SaveMenu + "Reset Daily Bonus")]
        public static void ResetDailyBonus()
        {
            var path = Path.Combine(Application.persistentDataPath, SaveFileName);
            if (!File.Exists(path))
            {
                Debug.Log("There is no save file yet, so the daily bonus is already available.");
                return;
            }

            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            data.lastDailyBonusDay = -1;
            data.pendingWheelSegment = -1;
            File.WriteAllText(path, JsonUtility.ToJson(data));
            Debug.Log("Daily bonus reset; it appears on the next launch.");
        }

        [MenuItem(SaveMenu + "Reset Daily Bonus", true)]
        private static bool CanResetDailyBonus()
        {
            return !Application.isPlaying;
        }

        /// <summary>Adds 100 credits to the running game.</summary>
        [MenuItem(CheatsMenu + "Add 100 Credits")]
        public static void Add100Credits()
        {
            ServiceLocator.Get<CreditsService>().Add(Currency.Credits, 100);
        }

        /// <summary>Adds 100 premium credits to the running game.</summary>
        [MenuItem(CheatsMenu + "Add 100 Premium Credits")]
        public static void Add100PremiumCredits()
        {
            ServiceLocator.Get<CreditsService>().Add(Currency.PremiumCredits, 100);
        }

        /// <summary>Adds 100 experience to the running game.</summary>
        [MenuItem(CheatsMenu + "Add 100 Experience")]
        public static void Add100Experience()
        {
            ServiceLocator.Get<ExperienceService>().Add(100);
        }

        [MenuItem(CheatsMenu + "Add 100 Credits", true)]
        [MenuItem(CheatsMenu + "Add 100 Premium Credits", true)]
        [MenuItem(CheatsMenu + "Add 100 Experience", true)]
        private static bool IsPlaying()
        {
            return Application.isPlaying;
        }
    }
}
