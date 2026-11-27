using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Economy;
using UnityTemplate.Engagement;

namespace UnityTemplate.UserInterface.ConditionalMenus
{
    /// <summary>The welcome popup shown until its one-off reward is claimed: CLAIM, X and Android back all pay the reward once.</summary>
    public class FirstOpenPopUpMenu : PopUpWindow
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text creditsAmount;
        [SerializeField] private TMP_Text premiumCreditsAmount;
        [SerializeField] private Button claimButton;

        /// <summary>Wires CLAIM and writes the welcome title and reward amounts.</summary>
        protected override void Awake()
        {
            base.Awake();
            claimButton.onClick.AddListener(ClaimAndClose);
            title.text = "WELCOME TO " + Application.productName.ToUpperInvariant();
            creditsAmount.SetText("{0}", EngagementService.WelcomeCredits);
            premiumCreditsAmount.SetText("{0}", EngagementService.WelcomePremiumCredits);
        }

        /// <summary>The X button also pays the reward, so closing the popup never loses it.</summary>
        protected override void OnCloseButtonPressed()
        {
            ClaimAndClose();
        }

        /// <summary>Pays the welcome reward if it is still owed and slides the popup away.</summary>
        private void ClaimAndClose()
        {
            if (ServiceLocator.Get<EngagementService>().ClaimWelcomeReward())
            {
                var credits = ServiceLocator.Get<CreditsService>();
                credits.Add(Currency.Credits, EngagementService.WelcomeCredits);
                credits.Add(Currency.PremiumCredits, EngagementService.WelcomePremiumCredits);
            }
            CloseWithSlide();
        }
    }
}
