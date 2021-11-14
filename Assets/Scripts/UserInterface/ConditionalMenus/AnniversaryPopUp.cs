using System;
using Credits;
using Player;
using PureFunctions.DateTime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ConditionalMenus
{
    public class AnniversaryPopUp : PopUpInterface, IUserInterface
    {
        public static Action OnAnniversaryOfFirstOpen;
        [SerializeField] private Button claimRewardButton;
        [SerializeField] private TMP_Text textDisplay;
        private static readonly int RewardCredits = PlayerEngagementManager.AnniversaryRewardCredits;
        private static readonly int RewardPremiumCredits = PlayerEngagementManager.AnniversaryRewardPremiumCredits;

        protected override void Awake()
        {
            base.Awake();
            SetButtonEvents();
            SetDisplay();
        }
        
        private void SetButtonEvents()
        {
            claimRewardButton.onClick.AddListener(ClaimReward);
            claimRewardButton.onClick.AddListener(()=>Enable(false));
        }
        
        private void SetDisplay()
        {
            textDisplay.text = "Its our " + DateTimeOrdinals.ConvertToOrdinalString(HolidayManager.AmountOfYearsSinceFirstOpen) + " anniversary! Thanks for continuing to play and support our game, here is " + RewardCredits + " credits and " + RewardPremiumCredits + " premium credits as a thank you!";
        }

        private static void ClaimReward()
        {
            CreditsManager.ChangeCredits(CreditsManager.Currency.Credits, RewardCredits);
            CreditsManager.ChangeCredits(CreditsManager.Currency.PremiumCredits, RewardPremiumCredits);
        }

        public override void Enable(bool state = true)
        {
            base.Enable(state);
            OnAnniversaryOfFirstOpen?.Invoke();
        }
    }
}
