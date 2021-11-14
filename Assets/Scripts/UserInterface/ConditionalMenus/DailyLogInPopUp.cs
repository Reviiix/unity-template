using System;
using Credits;
using Player;
using PureFunctions.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ConditionalMenus
{
    [Serializable]
    public class DailyLogInPopUp : PopUpInterface, IUserInterface
    {
        [SerializeField] private SpinningWheel bonusWheel;
        [SerializeField] private Button proceedButton;
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private TMP_Text consecutiveOpensDisplay;
        [SerializeField] private TMP_Text wheelWinDisplay;
        private static Coroutine _creditsRollup;
        private static int _creditsFromWheelCache;
        private static int DailyCredits => PlayerEngagementManager.DailyBonusRewardCredits;
        private const int RollUpTime = CreditsDisplay.RollUpTimeInSeconds;

        protected override void Awake()
        {
            base.Awake();
            bonusWheel.OnWheelSpinEnded += OnWheelSpinEnded;
            proceedButton.onClick.AddListener(StartSpin);
            closeButton.onClick.AddListener(ClaimReward);
            SetDisplay();
            RollupCredits(0,0,0);
        }

        private void OnDisable()
        {
            if (_creditsRollup != null)
            {
                StopCoroutine(_creditsRollup);
            }
        }

        private void SetDisplay()
        {
            const string consecutiveOpensPrefix = "CONSECUTIVE BONUS: ";
            consecutiveOpensDisplay.text = consecutiveOpensPrefix + DailyCredits;
        }

        private void StartSpin()
        {
            closeButton.gameObject.SetActive(false);
            proceedButton.interactable = false;
            ChangeButtonState();
            bonusWheel.StartSpin();
        }
        
        private void OnWheelSpinEnded(int resultSegmentValue)
        {
            _creditsFromWheelCache = resultSegmentValue;
            RollupCredits(0, resultSegmentValue, RollUpTime, () =>
            {
                proceedButton.interactable = true;
            });
        }

        private void ChangeButtonState()
        {
            const string claim = "CLAIM";
            proceedButton.onClick.RemoveListener(StartSpin);
            proceedButton.onClick.AddListener(ClaimReward);
            buttonText.text = claim;
        }

        private void ClaimReward()
        {
            var credits = DailyCredits + _creditsFromWheelCache;
            Display.GetComponent<GraphicRaycaster>().enabled = false;
            CreditsManager.ChangeCredits(CreditsManager.Currency.PremiumCredits, credits);
            RollupCredits(credits, 0, RollUpTime, () =>
            {
                Enable(false);
            });
        }

        private void RollupCredits(int startValue, int endValue, int seconds, Action callBack = null)
        {
            const string prefix = "WHEEL WIN: ";
            const string suffix = "";
            _creditsRollup = StartCoroutine(NumberRollup.Rollup(wheelWinDisplay, startValue, endValue, prefix, suffix, seconds, callBack));
        }
    }
}
