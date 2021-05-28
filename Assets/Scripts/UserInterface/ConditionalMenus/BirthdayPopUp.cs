using System;
using Object = UnityEngine.Object;

namespace UserInterface.ConditionalMenus
{
    [Serializable]
    public class BirthdayPopUp : PopUpInterface, IUserInterface
    {
        private readonly bool Birthday = HolidayManager.IsUserBirthday;

        public override void Initialise()
        {
            base.Initialise();
            if (!Birthday) Object.Destroy(display.gameObject);
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
                    Object.Destroy(display.gameObject);
                    break;
            }
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
