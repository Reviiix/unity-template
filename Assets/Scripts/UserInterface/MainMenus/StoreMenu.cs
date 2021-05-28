using System;

namespace UserInterface.MainMenus
{
    [Serializable]
    public class StoreMenu : UserInterface, IUserInterface
    {
        public void Initialise()
        {
            
        }
        
        public void Enable(bool state = true)
        {
            display.enabled = state;
        }
    }
}