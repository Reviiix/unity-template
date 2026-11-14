using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Economy;
using UnityTemplate.Engagement;
using UnityTemplate.Utilities;

namespace UnityTemplate.UserInterface.ConditionalMenus
{
    /// <summary>The first-open anniversary popup: thanks the player and pays the anniversary reward on CLAIM.</summary>
    public class AnniversaryPopUp : PopUpWindow
    {
        private const int PremiumCreditsDivisor = 10;
        [SerializeField] private Button claimRewardButton;
        [SerializeField] private TMP_Text textDisplay;
        private EngagementService engagement;
        private int rewardCredits;
        private int rewardPremiumCredits;

        protected override void Awake()
        {
            base.Awake();
            engagement = ServiceLocator.Get<EngagementService>();
            claimRewardButton.onClick.AddListener(ClaimAndClose);

            var today = DateTime.Now;
            rewardCredits = engagement.GetAnniversaryCredits(today);
            rewardPremiumCredits = rewardCredits / PremiumCreditsDivisor;
            textDisplay.text = new StringBuilder("It's our ")
                .Append(DateTimeOrdinals.ConvertToOrdinalString(engagement.GetYearsSinceFirstOpen(today)))
                .Append(" anniversary! Thanks for continuing to play and support our game, here is ")
                .Append(rewardCredits).Append(" credits and ")
                .Append(rewardPremiumCredits).Append(" premium credits as a thank you!")
                .ToString();
        }

        private void ClaimAndClose()
        {
            if (engagement.ClaimAnniversary(DateTime.Now))
            {
                var credits = ServiceLocator.Get<CreditsService>();
                credits.Add(Currency.Credits, rewardCredits);
                credits.Add(Currency.PremiumCredits, rewardPremiumCredits);
            }
            CloseImmediately();
        }
    }
}
