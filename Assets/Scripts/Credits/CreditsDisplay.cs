using System;
using System.Collections;
using System.Collections.Generic;
using Abstract;
using Abstract.Interfaces;
using PureFunctions.Effects;
using TMPro;
using UnityEngine;

namespace Credits
{
    public class CreditsDisplay : MonoBehaviour
    {
        private const int RollUpTimeInSeconds = 1;
        [SerializeField] private CreditDisplay credits;
        [SerializeField] private CreditDisplay premiumCredits;

        private void Start()
        {
            StartCoroutine(ProjectManager.WaitForAnyAsynchronousInitialisationToComplete(() =>
            {
                MonoBehaviour coRoutineHandler = this;
                credits.Initialise(coRoutineHandler, CreditsManager.ReturnCredits(CreditsManager.Currency.Credits));
                premiumCredits.Initialise(coRoutineHandler, CreditsManager.ReturnCredits(CreditsManager.Currency.PremiumCredits));
            }));
        }

        private void OnEnable()
        {
            CreditsManager.OnCreditsChanged += credits.OnCreditsChanged;
            CreditsManager.OnPremiumCreditsChanged += premiumCredits.OnCreditsChanged;
        }
        
        private void OnDisable()
        {
            CreditsManager.OnCreditsChanged -= credits.OnCreditsChanged;
            CreditsManager.OnPremiumCreditsChanged -= premiumCredits.OnCreditsChanged;
        }

        [Serializable]
        private class CreditDisplay : IHandleSimultaneousAdditions
        {
            private const int RollUpSpeed = RollUpTimeInSeconds;
            private MonoBehaviour _coRoutineHandler;
            private bool _animating;
            [SerializeField] private TMP_Text creditsDisplay;
            [SerializeField] private string prefix = "ERROR";
            [SerializeField] private string suffix = "ERROR";
            private readonly Queue<KeyValuePair<long, long>> _additionQueue = new Queue<KeyValuePair<long, long>>(); //Experience additions that happened during this animation are queued up and applied after the animation has finished.

            public void Initialise(MonoBehaviour coRoutineHandler, long startingValue)
            {
                _coRoutineHandler = coRoutineHandler;
                creditsDisplay.text = prefix + startingValue + suffix;
            }

            public void OnCreditsChanged(long originalAmount, long newAmount)
            {
                if (_animating)
                {
                    _additionQueue.Enqueue(new KeyValuePair<long, long>(originalAmount, newAmount));
                    return;
                }
                UpdateDisplay(originalAmount, newAmount);
            }

            private void UpdateDisplay(long originalAmount, long newAmount)
            {
                _animating = true;
                _coRoutineHandler.StartCoroutine(NumberRolling.Rollup(creditsDisplay, originalAmount, newAmount, prefix, suffix,RollUpSpeed,() =>
                {
                    _animating = false;
                    creditsDisplay.text = prefix + newAmount + suffix;
                    _coRoutineHandler.StartCoroutine(HandleDelayedAdditions());
                }));
            }
            
            public IEnumerator HandleDelayedAdditions()
            {
                yield return new WaitUntil(() => !_animating);
            
                if (_additionQueue.Count == 0) yield break;

                var dequeue = _additionQueue.Dequeue();
                UpdateDisplay(dequeue.Key, dequeue.Value);
            }
        }
    }
}
