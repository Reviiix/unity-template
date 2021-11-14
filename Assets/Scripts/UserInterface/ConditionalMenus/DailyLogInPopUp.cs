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

        protected override void Awake()
        {
            base.Awake();
            bonusWheel.OnWheelSpinEnded += OnWheelSpinEnded;
            proceedButton.onClick.AddListener(StartSpin);
            closeButton.onClick.AddListener(ClaimReward);
            SetDisplay();
            RollupCredits(0, 0);
        }

        private void OnDisable()
        {
            if (_creditsRollup != null) StopCoroutine(_creditsRollup);
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
            RollupCredits(resultSegmentValue);
            proceedButton.interactable = true;
        }

        private void ChangeButtonState()
        {
            const string claim = "CLAIM!";
            proceedButton.onClick.RemoveListener(StartSpin);
            proceedButton.onClick.AddListener(() => Enable(false));
            proceedButton.onClick.AddListener(ClaimReward);
            buttonText.text = claim;
        }

        private void ClaimReward()
        {
            Display.GetComponent<GraphicRaycaster>().enabled = false; // Stop players clicking anything else;
            CreditsManager.ChangeCredits(CreditsManager.Currency.PremiumCredits, DailyCredits + _creditsFromWheelCache);
        }

        private void RollupCredits(int credits, int seconds = 3)
        {
            const string prefix = "WHEEL WIN: ";
            const string suffix = "";
            _creditsRollup = StartCoroutine(NumberRolling.Rollup(wheelWinDisplay, 0, credits, prefix, suffix, seconds));
        }
    }
}
