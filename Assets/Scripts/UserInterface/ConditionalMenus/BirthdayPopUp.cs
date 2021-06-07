using Credits;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ConditionalMenus
{
    public class BirthdayPopUp : PopUpInterface, IUserInterface
    {
        private readonly bool _birthday = HolidayManager.IsUserBirthday;
        private const int RewardCreditsCredits = PlayerEngagementManager.BirthdayRewardCredits;
        private const int RewardCreditsPremiumCredits = PlayerEngagementManager.BirthdayRewardPremiumCredits;
        [SerializeField] private Button claimRewardButton;
        
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

        }

        private static void ClaimReward()
        {
            CreditsManager.ChangeCredits(CreditsManager.Currency.Credits, RewardCreditsCredits);
            CreditsManager.ChangeCredits(CreditsManager.Currency.PremiumCredits, RewardCreditsPremiumCredits);
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
