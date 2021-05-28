using System;
using Credits;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ConditionalMenus
{
    [Serializable]
    public class DailyLogInPopUp : PopUpInterface, IUserInterface
    {
        [SerializeField] private TMP_Text rewardDisplay;
        [SerializeField] private TMP_Text consecutiveOpensDisplay;
        [SerializeField] private Button claimRewardButton;
        private static int DailyBonusReward => PlayerEngagementManager.DailyBonusReward;

        public override void Initialise()
        {
            if (PlayerEngagementManager.TimesGameHasBeenOpened <= 1 || PlayerEngagementManager.IsRepeatOpenToday)
            {
                UnityEngine.Object.Destroy(display.gameObject);
                return;
            }
            base.Initialise();
            SetButtonEvents();
            SetDisplay();
        }

        private void SetButtonEvents()
        {
            claimRewardButton.onClick.AddListener(ClaimReward);
        }
        
        private void SetDisplay()
        {
            const string claimPrefix = "CLAIM ";
            const string consecutiveOpensPrefix = "CONSECUTIVE BONUS: ";
            rewardDisplay.text = claimPrefix + DailyBonusReward + "!";
            consecutiveOpensDisplay.text = consecutiveOpensPrefix + PlayerInformation.ConsecutiveDailyOpens;
        }
        
        private static void ClaimReward()
        {
            CreditsManager.ChangeCredits(CreditsManager.Currency.PremiumCredits, DailyBonusReward);
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
    }
}
