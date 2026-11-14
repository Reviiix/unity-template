using UnityEngine.AddressableAssets;

namespace UnityTemplate.Achievements
{
    /// <summary>An achievement at runtime: its display texts, reward, icon and whether the player has unlocked it.</summary>
    public class AchievementInfo
    {
        public Achievement Id { get; }
        public string Title { get; }
        public string Description { get; }
        public int Reward { get; }
        public AssetReferenceSprite Icon { get; }
        public bool IsUnlocked { get; internal set; }

        /// <summary>Creates the runtime entry; the title is the id with spaces before each capital letter.</summary>
        public AchievementInfo(Achievement id, string description, int reward, AssetReferenceSprite icon)
        {
            Id = id;
            Title = Utilities.StringUtilities.AddSpacesBeforeCapitals(id.ToString());
            Description = description;
            Reward = reward;
            Icon = icon;
        }
    }
}
