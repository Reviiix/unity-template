using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Economy;
using UnityTemplate.Engagement;

namespace UnityTemplate.UserInterface.ConditionalMenus
{
    /// <summary>The daily bonus popup: shows the streak bonus, spins the prize wheel and pays both out in premium credits on CLAIM.</summary>
    public class DailyLogInPopUp : PopUpWindow
    {
        private const float RollUpSeconds = 101 / 30f;
        private const string WheelWinFormat = "WHEEL WIN: {0:0}";
        private const string ClaimText = "CLAIM";
        [SerializeField] private SpinningWheel bonusWheel;
        [SerializeField] private Button proceedButton;
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private TMP_Text consecutiveOpensDisplay;
        [SerializeField] private TMP_Text wheelWinDisplay;
        private EngagementService engagement;
        private CreditsService credits;
        private Tween rollUp;
        private int wheelWinnings;
        private bool hasSpun;

        protected override void Awake()
        {
            base.Awake();
            engagement = ServiceLocator.Get<EngagementService>();
            credits = ServiceLocator.Get<CreditsService>();
            bonusWheel.SpinEnded += OnWheelSpinEnded;
            proceedButton.onClick.AddListener(OnProceedPressed);
            consecutiveOpensDisplay.SetText("CONSECUTIVE BONUS: {0:0}", engagement.GetDailyBonusCredits(DateTime.Now));
            wheelWinDisplay.SetText(WheelWinFormat, 0);
        }

        protected override void OnDestroy()
        {
            rollUp?.Kill();
            base.OnDestroy();
        }

        /// <summary>The X button claims the streak bonus without spinning and slides the popup away.</summary>
        protected override void OnCloseButtonPressed()
        {
            Claim();
            CloseWithSlide();
        }

        private void OnProceedPressed()
        {
            if (hasSpun) ClaimAndClose();
            else StartSpin();
        }

        private void StartSpin()
        {
            hasSpun = true;
            closeButton.gameObject.SetActive(false);
            proceedButton.interactable = false;
            buttonText.text = ClaimText;

            var segment = engagement.PendingWheelSegment;
            if (segment == EngagementService.NoPendingWheelSegment)
            {
                segment = bonusWheel.PickRandomSegment();
                engagement.SetPendingWheelSegment(segment);
            }
            bonusWheel.SpinTo(segment);
        }

        private void OnWheelSpinEnded(int segmentValue)
        {
            wheelWinnings = segmentValue;
            RollUpWheelWin(0, segmentValue, () => proceedButton.interactable = true);
        }

        private void ClaimAndClose()
        {
            SetContentInteractable(false);
            var payout = Claim();
            RollUpWheelWin(payout, 0, CloseImmediately);
        }

        private int Claim()
        {
            var today = DateTime.Now;
            var payout = engagement.GetDailyBonusCredits(today) + wheelWinnings;
            if (engagement.ClaimDailyBonus(today)) credits.Add(Currency.PremiumCredits, payout);
            return payout;
        }

        private void RollUpWheelWin(int from, int to, Action onComplete)
        {
            rollUp?.Kill();
            float shown = from;
            rollUp = DOTween.To(() => shown, value =>
                {
                    shown = value;
                    wheelWinDisplay.SetText(WheelWinFormat, shown);
                }, to, RollUpSeconds)
                .SetEase(Ease.Linear)
                .SetLink(gameObject)
                .OnComplete(() => onComplete());
        }
    }
}
