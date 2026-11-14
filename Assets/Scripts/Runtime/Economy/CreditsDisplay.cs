using System;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityTemplate.Core;

namespace UnityTemplate.Economy
{
    /// <summary>Shows both currency balances and rolls the number up whenever a balance changes.</summary>
    public class CreditsDisplay : MonoBehaviour
    {
        public const float RollUpSeconds = 101 / 30f;
        [SerializeField] private CreditDisplay credits;
        [SerializeField] private CreditDisplay premiumCredits;
        private CreditsService creditsService;

        private SaveService saveService;

        private void OnEnable()
        {
            creditsService = ServiceLocator.Get<CreditsService>();
            saveService = ServiceLocator.Get<SaveService>();
            creditsService.BalanceChanged += OnBalanceChanged;
            saveService.DataLoaded += ShowCurrentBalances;
            ShowCurrentBalances();
        }

        private void OnDisable()
        {
            credits.StopAnimation();
            premiumCredits.StopAnimation();
            creditsService.BalanceChanged -= OnBalanceChanged;
            saveService.DataLoaded -= ShowCurrentBalances;
        }

        private void ShowCurrentBalances()
        {
            credits.Show(creditsService.GetBalance(Currency.Credits));
            premiumCredits.Show(creditsService.GetBalance(Currency.PremiumCredits));
        }

        private void OnBalanceChanged(Currency currency, ValueChange change)
        {
            (currency == Currency.Credits ? credits : premiumCredits).Animate(change, gameObject);
        }

        /// <summary>One balance read-out; changes that arrive mid-animation are queued and played afterwards.</summary>
        [Serializable]
        private class CreditDisplay
        {
            [SerializeField] private TMP_Text creditsDisplay;
            [SerializeField] private string prefix = "";
            [SerializeField] private string suffix = "";
            private readonly Queue<ValueChange> queuedChanges = new();
            private readonly StringBuilder textBuilder = new();
            private Tween rollUp;

            public void Show(long value)
            {
                StopAnimation();
                queuedChanges.Clear();
                SetText(value);
            }

            public void Animate(ValueChange change, GameObject owner)
            {
                if (rollUp != null && rollUp.IsActive())
                {
                    queuedChanges.Enqueue(change);
                    return;
                }

                double shown = change.OldValue;
                rollUp = DOTween.To(() => shown, value =>
                    {
                        shown = value;
                        SetText((long)Math.Round(shown));
                    }, change.NewValue, RollUpSeconds)
                    .SetEase(Ease.Linear)
                    .SetLink(owner)
                    .OnComplete(() =>
                    {
                        SetText(change.NewValue);
                        rollUp = null;
                        if (queuedChanges.Count > 0) Animate(queuedChanges.Dequeue(), owner);
                    });
            }

            public void StopAnimation()
            {
                rollUp?.Kill();
                rollUp = null;
            }

            private void SetText(long value)
            {
                textBuilder.Clear().Append(prefix).Append(value).Append(suffix);
                creditsDisplay.SetText(textBuilder);
            }
        }
    }
}
