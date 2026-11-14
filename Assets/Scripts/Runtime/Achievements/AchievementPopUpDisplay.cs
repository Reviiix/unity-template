using System.Collections.Generic;
using UnityEngine;
using UnityTemplate.Core;

namespace UnityTemplate.Achievements
{
    /// <summary>Shows unlocked achievements as toasts using pooled popups, queueing them when every popup is busy.</summary>
    public class AchievementPopUpDisplay : MonoBehaviour
    {
        private readonly Queue<AchievementPopUpItem> idlePopUps = new();
        private readonly Queue<AchievementInfo> waitingAchievements = new();
        private AchievementService achievements;

        /// <summary>Takes every pooled achievement toast and parents it under this display.</summary>
        private void Awake()
        {
            achievements = ServiceLocator.Get<AchievementService>();
            var pool = ServiceLocator.Get<ObjectPooler>();
            var amountOfPopUps = pool.GetPoolSize(PoolKey.AchievementPopUp);
            for (var i = 0; i < amountOfPopUps; i++)
            {
                var popUp = pool.Get(PoolKey.AchievementPopUp, false).GetComponent<AchievementPopUpItem>();
                popUp.transform.SetParent(transform, false);
                idlePopUps.Enqueue(popUp);
            }
        }

        /// <summary>Listens for unlocked achievements.</summary>
        private void OnEnable()
        {
            achievements.AchievementUnlocked += OnAchievementUnlocked;
        }

        /// <summary>Stops listening for unlocked achievements.</summary>
        private void OnDisable()
        {
            achievements.AchievementUnlocked -= OnAchievementUnlocked;
        }

        /// <summary>Shows the achievement on an idle toast, or queues it until one is free.</summary>
        private void OnAchievementUnlocked(AchievementInfo achievement)
        {
            if (idlePopUps.Count == 0)
            {
                waitingAchievements.Enqueue(achievement);
                return;
            }
            Show(achievement, idlePopUps.Dequeue());
        }

        /// <summary>Shows an achievement on a toast, then shows the next queued one or returns the toast to the idle queue.</summary>
        private void Show(AchievementInfo achievement, AchievementPopUpItem popUp)
        {
            popUp.transform.SetAsLastSibling();
            popUp.Show(achievement, () =>
            {
                popUp.gameObject.SetActive(false);
                if (waitingAchievements.Count > 0) Show(waitingAchievements.Dequeue(), popUp);
                else idlePopUps.Enqueue(popUp);
            });
        }
    }
}
