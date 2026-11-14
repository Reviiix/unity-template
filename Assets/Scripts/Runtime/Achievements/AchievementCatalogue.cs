using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnityTemplate.Achievements
{
    /// <summary>Designer-editable definitions of the permanent achievements (description, reward, icon) plus the icon used for remotely configured ones.</summary>
    [CreateAssetMenu(fileName = "AchievementCatalogue", menuName = "Unity Template/Achievement Catalogue")]
    public class AchievementCatalogue : ScriptableObject
    {
        [SerializeField] private AchievementDefinition[] permanentAchievements = Array.Empty<AchievementDefinition>();
        [SerializeField] private AssetReferenceSprite dynamicAchievementIcon;

        public AchievementDefinition[] PermanentAchievements => permanentAchievements;
        public AssetReferenceSprite DynamicAchievementIcon => dynamicAchievementIcon;
    }

    /// <summary>One permanent achievement as authored in the AchievementCatalogue asset.</summary>
    [Serializable]
    public struct AchievementDefinition
    {
        public Achievement achievement;
        public string description;
        public int reward;
        public AssetReferenceSprite icon;
    }
}
