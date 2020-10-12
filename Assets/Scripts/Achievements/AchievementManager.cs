using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Achievements
{
    public class AchievementManager : MonoBehaviour
    {
        public Achievement[] achievements;
    }

    [Serializable]
    public class Achievement : IAchievement
    {
        public bool Unlocked { get; private set; }
        [SerializeField]
        private string name;
        [SerializeField]
        private string description;
        [SerializeField]
        private int value;
        [SerializeField]
        private Sprite image;

        public void Unlock(bool state = true)
        {
            Unlocked = state;
        }
    }
    
    public interface IAchievement
    {
        void Unlock(bool state);
    }
}
