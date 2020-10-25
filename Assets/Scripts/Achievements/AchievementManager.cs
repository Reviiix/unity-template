using System;
using UnityEditor;
using UnityEngine;

namespace Achievements
{
    //Create the achievement structs in the inspector and they will be converted into actual achievements to ensure they can only be accessed by the appropriate sources.
    public class AchievementManager : MonoBehaviour
    {
        public static readonly string AchievementKeyPrefix = PlayerSettings.productName + " Achievement:";
        [SerializeField]
        private Achievement[] achievementStructs;
        public static Achievement[] Achievements { get; private set; }

        public void Initialise()
        {
            CreateAchievementsFromStructs();
        }

        private void CreateAchievementsFromStructs()
        {
            Achievements = new Achievement[achievementStructs.Length];
            for (var i = 0; i < achievementStructs.Length; i++)
            {
                Achievements[i] = new Achievement(i, achievementStructs[i].Name, achievementStructs[i].Description, achievementStructs[i].Value, achievementStructs[i].Icon);
            }
        }
    }

    [Serializable]
    public class Achievement : IAchievement
    {
        public int Index { get; private set; }
        public bool Locked
        {
            get
            {
                var key = AchievementManager.AchievementKeyPrefix + ". " + Name + " Locked State";
                
                if (!PlayerPrefs.HasKey(key)) return false;
                
                return PlayerPrefs.GetInt(key) == 1;
            }
            private set
            {
                var key = AchievementManager.AchievementKeyPrefix + ". " + Name + " Locked State";
                var intValue = value ? 1 : 0;
                
                PlayerPrefs.SetInt(key, intValue);
            }
        }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Value { get; private set; }
        public Sprite Icon { get; private set; }

        public Achievement(int index, string name, string description, int value, Sprite icon)
        {
            Index = index;
            Name = name;
            Description = description;
            Value = value;
            Icon = icon;
        }

        public void Unlock(bool state = true)
        {
            Locked = state;
        }
    }
    
    public struct AchievementStruct
    {
        public string Name;
        public string Description;
        public int Value;
        public Sprite Icon;
    }
    
    public interface IAchievement
    {
        void Unlock(bool state);
    }
}
