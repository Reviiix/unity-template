using System.Collections;
using System.Collections.Generic;
using Abstract;
using Abstract.Interfaces;
using PureFunctions;
using UnityEngine;

namespace Achievements.Display.PopUp
{
    [RequireComponent(typeof(Transform))]
    public class AchievementPopUpDisplay : PrivateSingleton<AchievementPopUpDisplay>, IHandleSimultaneousAdditions
    {
        private static readonly Queue<AchievementPopUpItem> PopUps = new Queue<AchievementPopUpItem>();
        private static readonly Queue<PermanentAchievementManager.Achievement> DelayedPermanentAchievementsQueue = new Queue<PermanentAchievementManager.Achievement>();
        private static readonly Queue<DynamicAchievementManager.Achievement> DelayedDynamicAchievementsQueue = new Queue<DynamicAchievementManager.Achievement>();
        private static int _maximumNumberOfActiveAchievements;
        private static Transform _parent;
        private const int PoolIndex = 1;
        private static int _activeAchievements;
        private static bool SpaceAvailable => _activeAchievements < _maximumNumberOfActiveAchievements;

        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(ProjectManager.WaitForInitialisationToComplete(ResolveDependencies));
        }

        private void OnEnable()
        {
            PermanentAchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
            DynamicAchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
            StartCoroutine(ProjectManager.WaitForInitialisationToComplete(ResolveDependencies));
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            PermanentAchievementManager.OnAchievementUnlocked -= OnAchievementUnlocked;
            DynamicAchievementManager.OnAchievementUnlocked -= OnAchievementUnlocked;
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

        private static void OnAchievementUnlocked(PermanentAchievementManager.Achievement achievement)
        {
            if (_activeAchievements >= _maximumNumberOfActiveAchievements)
            {
                DelayedPermanentAchievementsQueue.Enqueue(achievement);
                return;
            }
            PopAchievement(achievement);
        }
        
        private static void OnAchievementUnlocked(DynamicAchievementManager.Achievement achievement)
        {
            if (_activeAchievements >= _maximumNumberOfActiveAchievements)
            {
                DelayedDynamicAchievementsQueue.Enqueue(achievement);
                return;
            }
            PopAchievement(achievement);
        }

        private static void PopAchievement(PermanentAchievementManager.Achievement achievement)
        {
            var assetReference = PermanentAchievementManager.ReturnSpriteAssetReference(achievement);
            _activeAchievements++;
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<Sprite>(assetReference, (returnVariable)=>
            {
                var popUp = PopUps.Dequeue();
                popUp.transform.SetSiblingIndex(_maximumNumberOfActiveAchievements-1);
                popUp.Show(returnVariable, StringUtilities.AddSpacesBeforeCapitals(achievement.ToString()), PermanentAchievementManager.ReturnDescription(achievement), PermanentAchievementManager.ReturnReward(achievement), () =>
                {
                    _activeAchievements--;
                    PopUps.Enqueue(popUp);
                    popUp.gameObject.SetActive(false);
                });
                AssetReferenceLoader.UnloadAssetReference(assetReference);
                Instance.StartCoroutine(Instance.HandleDelayedAdditions());
            });
        }
        
        private static void PopAchievement(DynamicAchievementManager.Achievement achievement)
        {
            var assetReference = DynamicAchievementManager.ReturnSpriteAssetReference(achievement);
            _activeAchievements++;
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<Sprite>(assetReference, (returnVariable)=>
            {
                var popUp = PopUps.Dequeue();
                popUp.transform.SetSiblingIndex(_maximumNumberOfActiveAchievements-1);
                popUp.Show(returnVariable, StringUtilities.AddSpacesBeforeCapitals(achievement.ToString()), DynamicAchievementManager.ReturnDescription(achievement), DynamicAchievementManager.ReturnReward(achievement), () =>
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

            OnAchievementUnlocked((PermanentAchievementManager.Achievement)DelayedDynamicAchievementsQueue.Dequeue());
        }
    }
}
