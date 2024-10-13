using System;
using System.Collections;
using System.Collections.Generic;
using Abstract.Interfaces;
using pure_unity_methods;
using pure_unity_methods.Effects;
using PureFunctions;
using PureFunctions.UnitySpecific;
using PureFunctions.UnitySpecific.Effects;
using TMPro;
using UnityEngine;

namespace Credits
{
    /// <summary>
    /// This class handles displaying the credit values.
    /// It is not for tracking values. That job belongs to CreditsManager.
    /// </summary>
    public class CreditsDisplay : MonoBehaviour
    {
        public const int RollUpTimeInSeconds = 1;
        [SerializeField] private CreditDisplay credits;
        [SerializeField] private CreditDisplay premiumCredits;

        private IEnumerator Start()
        {
            yield return Wait.WaitForInitialisation;
            credits.Initialise(CreditsManager.GetCredits(CreditsManager.Currency.Credits));
            premiumCredits.Initialise(CreditsManager.GetCredits(CreditsManager.Currency.PremiumCredits));
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
            private bool animating;
            [SerializeField] private TMP_Text creditsDisplay;
            [SerializeField] private string prefix = "ERROR";
            [SerializeField] private string suffix = "ERROR";
            private readonly Queue<ValueChangeInformation> additionQueue = new (); //Experience additions that happened during this animation are queued up and applied after the animation has finished.

            public void Initialise(long startingValue)
            {
                creditsDisplay.text = prefix + startingValue + suffix;
            }

            public void OnCreditsChanged(ValueChangeInformation valueChangeInformation)
            {
                if (animating)
                {
                    additionQueue.Enqueue(valueChangeInformation);
                    return;
                }
                UpdateDisplay(valueChangeInformation);
            }

            private void UpdateDisplay(ValueChangeInformation valueChangeInformation)
            {
                var newValue = valueChangeInformation.NewValue;
                animating = true;
                Coroutiner.StartCoroutine(NumberRollup.Rollup(creditsDisplay, valueChangeInformation.OldValue, newValue, prefix, suffix,RollUpSpeed,() =>
                {
                    animating = false;
                    creditsDisplay.text = prefix + newValue + suffix;
                    Coroutiner.StartCoroutine(HandleDelayedAdditions());
                }));
            }
            
            public IEnumerator HandleDelayedAdditions()
            {
                yield return new WaitUntil(() => !animating);
            
                if (additionQueue.Count == 0) yield break;
                
                UpdateDisplay(additionQueue.Dequeue());
            }
        }
    }
}
