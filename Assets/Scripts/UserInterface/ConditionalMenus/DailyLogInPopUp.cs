using System;
using Credits;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UserInterface.ConditionalMenus
{
    [Serializable]
    public class DailyLogInPopUp : PopUpInterface, IUserInterface
    {
        [SerializeField] private Button claimRewardButton;
        [SerializeField] private TMP_Text rewardDisplay;
        [SerializeField] private TMP_Text consecutiveOpensDisplay;
        private static int RewardCredits => PlayerEngagementManager.DailyBonusRewardCredits;
        private static int RewardPremiumCredits => PlayerEngagementManager.DailyBonusRewardCredits;

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
            const string claimPrefix = "CLAIM ";
            const string consecutiveOpensPrefix = "CONSECUTIVE BONUS: ";
            rewardDisplay.text = claimPrefix + RewardCredits + "!";
            consecutiveOpensDisplay.text = consecutiveOpensPrefix + PlayerEngagementManager.ConsecutiveDailyOpens;
        }
        
        private static void ClaimReward()
        {
            CreditsManager.ChangeCredits(CreditsManager.Currency.Credits, RewardCredits);
            CreditsManager.ChangeCredits(CreditsManager.Currency.PremiumCredits, RewardPremiumCredits);
        }

        public void Enable(bool state = true)
        {
            Display.enabled = state;
            switch (state)
            {
                case true:
                    AppearAnimation(popUpMenu);
                    break;
                case false:
                    ClaimReward();
                    DisappearAnimation(popUpMenu, UnloadSelf);
                    break;
            }
        }
    }
}
