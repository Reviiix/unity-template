using System;
using System.Collections;
using System.Collections.Generic;
using Abstract.Interfaces;
using PureFunctions;
using PureFunctions.Effects;
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
            MonoBehaviour coRoutineHandler = this;
            yield return ProjectManager.WaitForInitialisation;
            credits.Initialise(coRoutineHandler, CreditsManager.GetCredits(CreditsManager.Currency.Credits));
            premiumCredits.Initialise(coRoutineHandler, CreditsManager.GetCredits(CreditsManager.Currency.PremiumCredits));
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
            private MonoBehaviour coRoutineHandler;
            private bool animating;
            [SerializeField] private TMP_Text creditsDisplay;
            [SerializeField] private string prefix = "ERROR";
            [SerializeField] private string suffix = "ERROR";
            private readonly Queue<ValueChangeInformation> additionQueue = new Queue<ValueChangeInformation>(); //Experience additions that happened during this animation are queued up and applied after the animation has finished.

            public void Initialise(MonoBehaviour coRoutineHandler, long startingValue)
            {
                this.coRoutineHandler = coRoutineHandler;
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
                coRoutineHandler.StartCoroutine(NumberRollup.Rollup(creditsDisplay, valueChangeInformation.OldValue, newValue, prefix, suffix,RollUpSpeed,() =>
                {
                    animating = false;
                    creditsDisplay.text = prefix + newValue + suffix;
                    coRoutineHandler.StartCoroutine(HandleDelayedAdditions());
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
