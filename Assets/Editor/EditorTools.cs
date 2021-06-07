using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    public class EditorTools : UnityEditor.Editor
    {
        private const string InitialisationSceneFilePath = "Assets/Scenes/Initialisation.unity";
        
        [MenuItem("Leon Tools/Scenes/Load Initialisation Scene %l")]
        public static void LoadInitialisationScene()
        {
            EditorSceneManager.OpenScene(InitialisationSceneFilePath);
        }
        
        [MenuItem("Leon Tools/Reset/Delete Save Data")]
        public static void DeleteSaveData()
        {
            SaveSystem.Delete();
            SaveSystem.Save();
        }
    }
}