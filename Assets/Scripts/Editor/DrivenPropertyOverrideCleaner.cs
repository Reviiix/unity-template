using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityTemplate.EditorTools
{
    /// <summary>Drops prefab overrides on RectTransform properties that a component drives (a Slider's fill and handle anchors, layout groups) before scenes and prefabs are saved, so saving without changes leaves the file unchanged.</summary>
    [InitializeOnLoad]
    public static class DrivenPropertyOverrideCleaner
    {
        private static readonly PropertyInfo DrivenPropertiesInfo = typeof(RectTransform).GetProperty("drivenProperties", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly Dictionary<string, DrivenTransformProperties> PropertyFlags = new()
        {
            { "m_AnchorMin.x", DrivenTransformProperties.AnchorMinX },
            { "m_AnchorMin.y", DrivenTransformProperties.AnchorMinY },
            { "m_AnchorMax.x", DrivenTransformProperties.AnchorMaxX },
            { "m_AnchorMax.y", DrivenTransformProperties.AnchorMaxY },
            { "m_AnchoredPosition.x", DrivenTransformProperties.AnchoredPositionX },
            { "m_AnchoredPosition.y", DrivenTransformProperties.AnchoredPositionY },
            { "m_SizeDelta.x", DrivenTransformProperties.SizeDeltaX },
            { "m_SizeDelta.y", DrivenTransformProperties.SizeDeltaY },
            { "m_Pivot.x", DrivenTransformProperties.PivotX },
            { "m_Pivot.y", DrivenTransformProperties.PivotY },
        };

        /// <summary>Hooks scene and prefab saving.</summary>
        static DrivenPropertyOverrideCleaner()
        {
            EditorSceneManager.sceneSaving += (scene, _) => Clean(scene.GetRootGameObjects());
            PrefabStage.prefabSaving += root => Clean(new[] { root });
        }

        /// <summary>Removes driven-property overrides from every outermost prefab instance under the given roots; returns how many it removed.</summary>
        public static int Clean(IEnumerable<GameObject> roots)
        {
            if (DrivenPropertiesInfo == null) return 0;

            var removed = 0;
            var instanceRoots = roots.SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .Select(transform => transform.gameObject)
                .Where(PrefabUtility.IsOutermostPrefabInstanceRoot);
            foreach (var instanceRoot in instanceRoots)
            {
                var modifications = PrefabUtility.GetPropertyModifications(instanceRoot);
                if (modifications == null) continue;

                var driven = DrivenSources(instanceRoot);
                var kept = modifications.Where(modification => !IsDriven(modification, driven)).ToArray();
                if (kept.Length == modifications.Length) continue;

                removed += modifications.Length - kept.Length;
                PrefabUtility.SetPropertyModifications(instanceRoot, kept);
            }
            return removed;
        }

        /// <summary>Maps each driven RectTransform's source object (what modifications target) to the properties being driven.</summary>
        private static Dictionary<Object, DrivenTransformProperties> DrivenSources(GameObject instanceRoot)
        {
            var driven = new Dictionary<Object, DrivenTransformProperties>();
            foreach (var rect in instanceRoot.GetComponentsInChildren<RectTransform>(true))
            {
                if (rect.drivenByObject == null) continue;
                var source = PrefabUtility.GetCorrespondingObjectFromSource(rect);
                if (source != null) driven[source] = (DrivenTransformProperties)DrivenPropertiesInfo.GetValue(rect);
            }
            return driven;
        }

        /// <summary>Returns true when the modification sets a property its RectTransform's driver overwrites anyway.</summary>
        private static bool IsDriven(PropertyModification modification, Dictionary<Object, DrivenTransformProperties> driven)
        {
            return modification.target != null
                && driven.TryGetValue(modification.target, out var drivenProperties)
                && PropertyFlags.TryGetValue(modification.propertyPath, out var flag)
                && (drivenProperties & flag) != 0;
        }
    }
}
