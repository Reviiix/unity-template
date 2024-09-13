using System.Collections;
using System.Collections.Generic;
using Abstract.Interfaces;
using Achievements.Dynamic;
using Achievements.Permanent;
using PureFunctions;
using PureFunctions.UnitySpecific;
using UnityEngine;

namespace Achievements.PopUp
{
    /// <summary>
    /// This class handles the popping of achievements. It is shared by dynamic and permanent achievements.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class AchievementPopUpDisplay : PrivateSingleton<AchievementPopUpDisplay>, IHandleSimultaneousAdditions
    {
        private static readonly Queue<AchievementPopUpItem> PopUps = new ();
        private static readonly Queue<Achievement> DelayedPermanentAchievementsQueue = new ();
        private static readonly Queue<Achievement> DelayedDynamicAchievementsQueue = new ();
        private static int _maximumNumberOfActiveAchievements;
        private static Transform _parent;
        private const int PoolIndex = 1;
        private static int _activeAchievements;
        private static bool SpaceAvailable => _activeAchievements < _maximumNumberOfActiveAchievements;

        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(Wait.WaitForInitialisationToComplete(ResolveDependencies));
        }

        private void OnEnable()
        {
            AchievementManager.OnPermanentAchievementUnlocked += OnAchievementUnlocked;
            AchievementManager.OnDynamicAchievementUnlocked += OnAchievementUnlocked;
            StartCoroutine(Wait.WaitForInitialisationToComplete(ResolveDependencies));
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            AchievementManager.OnPermanentAchievementUnlocked -= OnAchievementUnlocked;
            AchievementManager.OnDynamicAchievementUnlocked -= OnAchievementUnlocked;
        }
        
        private void ResolveDependencies()
        {
            _maximumNumberOfActiveAchievements = ObjectPooler.GetMaximumActiveObjects(PoolIndex);
            _parent = GetComponent<Transform>();
            for (var i = 0; i < _maximumNumberOfActiveAchievements; i++)
            {
                var popUp = ObjectPooler.GetObjectFromPool(PoolIndex, Vector3.zero, Quaternion.identity.normalized, false).GetComponent<AchievementPopUpItem>();
                popUp.transform.SetParent(_parent);
                PopUps.Enqueue(popUp);
            }
        }
        
        
        private static void OnAchievementUnlocked(Achievement achievement)
        {
            if (_activeAchievements >= _maximumNumberOfActiveAchievements)
            {
                DelayedDynamicAchievementsQueue.Enqueue(achievement);
                return;
            }
            PopAchievement(achievement);
        }

        private static void PopAchievement(Achievement achievement)
        {
            var assetReference = AchievementManager.GetSpriteAssetReference(achievement);
            _activeAchievements++;
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<Sprite>(assetReference, (returnVariable)=>
            {
                var popUp = PopUps.Dequeue();
                popUp.transform.SetSiblingIndex(_maximumNumberOfActiveAchievements-1);
                popUp.Show(returnVariable, StringUtilities.AddSpacesBeforeCapitals(achievement.ToString()), AchievementManager.GetDescription(achievement), AchievementManager.GetReward(achievement), () =>
                {
                    _activeAchievements--;
                    PopUps.Enqueue(popUp);
                    popUp.gameObject.SetActive(false);
                });
                AssetReferenceLoader.UnloadAssetReference(assetReference);
                Instance.StartCoroutine(Instance.HandleDelayedAdditions());
            });
        }

        public IEnumerator HandleDelayedAdditions()
        {
            yield return new WaitUntil(() => SpaceAvailable);
            Instance.StartCoroutine(HandleDelayedAdditionsForPermanentAchievements());
            Instance.StartCoroutine(HandleDelayedAdditionsForDynamicAchievements());
        }
        
        private static IEnumerator HandleDelayedAdditionsForPermanentAchievements()
        {
            yield return new WaitUntil(() => SpaceAvailable);
            
            if (DelayedPermanentAchievementsQueue.Count == 0) yield break;

            OnAchievementUnlocked(DelayedPermanentAchievementsQueue.Dequeue());
        }
        
        private static IEnumerator HandleDelayedAdditionsForDynamicAchievements()
        {
            yield return new WaitUntil(() => SpaceAvailable);
            
            if (DelayedDynamicAchievementsQueue.Count == 0) yield break;

            OnAchievementUnlocked(DelayedDynamicAchievementsQueue.Dequeue());
        }
    }
}
