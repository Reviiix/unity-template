using System;
using Credits;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ConditionalMenus
{
    [Serializable]
    public class FirstOpenPopUpMenu : PopUpInterface, IUserInterface
    {
        [SerializeField] private TMP_Text rewardDisplay;
        [SerializeField] private Button claimReward;
        private const int WelcomeRewardCredits = PlayerEngagementManager.WelcomeRewardCredits;
        private const int WelcomeRewardPremiumCredits = PlayerEngagementManager.WelcomeRewardPremiumCredits;

        public override void Initialise()
        {
            if (PlayerEngagementManager.TimesGameHasBeenOpened > 1)
            {
                UnityEngine.Object.Destroy(display.gameObject);
                return;
            }
            claimReward.onClick.AddListener(ClaimReward);
            rewardDisplay.text = "CLAIM " + WelcomeRewardCredits + " CREDITS AND " + WelcomeRewardPremiumCredits + " PREMIUM CREDITS!";
            base.Initialise();
        }
        
        public void Enable(bool state = true)
        {
            display.enabled = state;

            switch (state)
            {
                case true:
                    AppearAnimation(popUpMenu);
                    break;
                case false:
                    UnityEngine.Object.Destroy(display.gameObject);
                    break;
            }
        }

        private void ClaimReward()
        {
            CreditsManager.ChangeCredits(CreditsManager.Currency.Credits,WelcomeRewardCredits);
            CreditsManager.ChangeCredits(CreditsManager.Currency.PremiumCredits,WelcomeRewardPremiumCredits);
            Enable(false);
        }
        
        protected override void CloseButtonPressed()
        {
            DisappearAnimation(popUpMenu, () =>
            {
                Enable(false);
            });
        }
    }
}