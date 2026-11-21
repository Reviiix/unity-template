using System.IO;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityTemplate.Tests
{
    /// <summary>Checks that saving the boot scene without changes writes the same file, so scene diffs only show real edits.</summary>
    public class SceneSaveTests
    {
        private const string InitialisationScenePath = "Assets/Scenes/Initialisation.unity";
        private const string CopyPath = "Temp/InitialisationSaveCheck.unity";

        [Test]
        public void SavingTheBootSceneUnchangedWritesTheSameFile()
        {
            var scene = SceneManager.GetSceneByPath(InitialisationScenePath);
            var openedHere = !scene.isLoaded;
            if (openedHere) scene = EditorSceneManager.OpenScene(InitialisationScenePath, OpenSceneMode.Additive);
            try
            {
                Assert.IsFalse(scene.isDirty && !openedHere, "The boot scene has unsaved edits; save or revert them first.");
                Assert.IsTrue(EditorSceneManager.SaveScene(scene, CopyPath, true));
                Assert.AreEqual(File.ReadAllText(InitialisationScenePath), File.ReadAllText(CopyPath), "Saving the unchanged boot scene changed it (look for prefab overrides on driven RectTransform properties).");
            }
            finally
            {
                if (openedHere) EditorSceneManager.CloseScene(scene, true);
                File.Delete(CopyPath);
            }
        }
    }
}
