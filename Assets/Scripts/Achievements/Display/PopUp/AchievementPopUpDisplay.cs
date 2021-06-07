using System.Collections;
using System.Collections.Generic;
using Abstract;
using Abstract.Interfaces;
using MostlyPureFunctions;
using UnityEngine;

namespace Achievements.Display.PopUp
{
    [RequireComponent(typeof(Transform))]
    public class AchievementPopUpDisplay : PrivateSingleton<AchievementPopUpDisplay>, IHandleSimultaneousAdditions
    {
        private static readonly Queue<AchievementPopUpItem> PopUps = new Queue<AchievementPopUpItem>();
        private static readonly Queue<AchievementManager.Achievement> DelayedAchievementsQueue = new Queue<AchievementManager.Achievement>();
        private static int _maximumNumberOfActiveAchievements;
        private static Transform _parent;
        private const int PoolIndex = 1;
        private static int _activeAchievements;
        private static bool SpaceAvailable => _activeAchievements < _maximumNumberOfActiveAchievements;

        protected override void OnEnable()
        {
            base.OnEnable();
            AchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
            StartCoroutine(Wait.WaitForAnyAsynchronousInitialisationToComplete(ResolveDependencies));
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            AchievementManager.OnAchievementUnlocked -= OnAchievementUnlocked;
        }
        
        private void ResolveDependencies()
        {
            _maximumNumberOfActiveAchievements = ObjectPooling.ReturnMaximumActiveObjects(PoolIndex);
            _parent = GetComponent<Transform>();
            for (var i = 0; i < _maximumNumberOfActiveAchievements; i++)
            {
                var popUp = ObjectPooling.ReturnObjectFromPool(PoolIndex, Vector3.zero, Quaternion.identity.normalized, false).GetComponent<AchievementPopUpItem>();
                popUp.transform.SetParent(_parent);
                PopUps.Enqueue(popUp);
            }
        }

        private static void OnAchievementUnlocked(AchievementManager.Achievement achievement)
        {
            if (_activeAchievements >= _maximumNumberOfActiveAchievements)
            {
                DelayedAchievementsQueue.Enqueue(achievement);
                return;
            }
            PopAchievements(achievement);
        }

        private static void PopAchievements(AchievementManager.Achievement achievement)
        {
            var assetReference = AchievementManager.ReturnSpriteAssetReference(achievement);
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<Sprite>(assetReference, (returnVariable)=>
            {
                var popUp = PopUps.Dequeue();
                popUp.transform.SetSiblingIndex(_maximumNumberOfActiveAchievements-1);
                _activeAchievements++;
                popUp.Show(returnVariable, achievement.ToString(), AchievementManager.ReturnDescription(achievement), AchievementManager.ReturnReward(achievement), () =>
                {
                    _activeAchievements--;
                    PopUps.Enqueue(popUp);
                });
                AssetReferenceLoader.UnloadAssetReference(assetReference);
                Instance.StartCoroutine(Instance.HandleDelayedAdditions());
            });
        }
        

        public IEnumerator HandleDelayedAdditions()
        {
            yield return new WaitUntil(() => SpaceAvailable);
            
            if (DelayedAchievementsQueue.Count == 0) yield break;

            var dequeue = DelayedAchievementsQueue.Dequeue();
            
            OnAchievementUnlocked(dequeue);
        }
    }
}
