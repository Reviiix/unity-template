using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityTemplate.Tests
{
    /// <summary>Checks the stage scenes in the catalogue: they load on top of the boot scene, whose AudioListener stays, so they must not bring their own.</summary>
    public class StageSceneTests
    {
        private const string StageCataloguePath = "Assets/Settings/StageCatalogue.asset";

        [Test]
        public void StageScenesHaveNoAudioListener()
        {
            foreach (var path in StageScenePaths())
            {
                var scene = SceneManager.GetSceneByPath(path);
                var openedHere = !scene.isLoaded;
                if (openedHere) scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                try
                {
                    foreach (var root in scene.GetRootGameObjects())
                    {
                        Assert.IsEmpty(root.GetComponentsInChildren<AudioListener>(true), $"{path} has an AudioListener; the boot scene's is the only one.");
                    }
                }
                finally
                {
                    if (openedHere) EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        /// <summary>Returns the path of every scene assigned in the stage catalogue.</summary>
        private static IEnumerable<string> StageScenePaths()
        {
            var groups = new SerializedObject(AssetDatabase.LoadAssetAtPath<ScriptableObject>(StageCataloguePath)).FindProperty("groups");
            for (var group = 0; group < groups.arraySize; group++)
            {
                var scenes = groups.GetArrayElementAtIndex(group).FindPropertyRelative("scenes");
                for (var stage = 0; stage < scenes.arraySize; stage++)
                {
                    var path = AssetDatabase.GUIDToAssetPath(scenes.GetArrayElementAtIndex(stage).FindPropertyRelative("m_AssetGUID").stringValue);
                    if (!string.IsNullOrEmpty(path)) yield return path;
                }
            }
        }
    }
}
