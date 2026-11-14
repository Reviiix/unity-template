using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnityTemplate.Progression
{
    /// <summary>Designer-editable list of stage groups and the Addressable scene of each stage; an empty scene slot shows as a locked stage.</summary>
    [CreateAssetMenu(fileName = "StageCatalogue", menuName = "Unity Template/Stage Catalogue")]
    public class StageCatalogue : ScriptableObject
    {
        [SerializeField] private StageGroup[] groups = Array.Empty<StageGroup>();

        public int GroupCount => groups.Length;

        /// <summary>Returns how many stages a group contains.</summary>
        public int GetStageCount(int group)
        {
            return groups[group].scenes.Length;
        }

        /// <summary>Returns the total number of stages across every group.</summary>
        public int GetTotalStageCount()
        {
            var total = 0;
            foreach (var group in groups) total += group.scenes.Length;
            return total;
        }

        /// <summary>Converts a group and stage number into one index across all stages, used for ratings.</summary>
        public int GetStageIndex(int group, int stage)
        {
            var index = 0;
            for (var i = 0; i < group; i++) index += groups[i].scenes.Length;
            return index + stage;
        }

        /// <summary>Returns the stage's scene reference, which is invalid when no scene has been assigned yet.</summary>
        public AssetReference GetScene(int group, int stage)
        {
            return groups[group].scenes[stage];
        }

        /// <summary>Returns true when a scene is assigned to the stage, so it can be loaded.</summary>
        public bool HasScene(int group, int stage)
        {
            return GetScene(group, stage).RuntimeKeyIsValid();
        }

        /// <summary>One group of stages: the scene of each stage, in order.</summary>
        [Serializable]
        private struct StageGroup
        {
            public AssetReference[] scenes;
        }
    }
}
