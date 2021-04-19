using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    public class EditorTools : UnityEditor.Editor
    {
        private const string InitialisationSceneFilePath = "Assets/Scenes/Initialisation.unity";
        
        [MenuItem("Tools/Scenes/Load Initialisation Scene %l")]
        public static void LoadInitialisationScene()
        {
            EditorSceneManager.OpenScene(InitialisationSceneFilePath);
        }
    }
}