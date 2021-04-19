using System;
using TMPro;

namespace UserInterface
{
    [Serializable]
    public class InGameUserInterface : UserInterface, IUserInterface
    {
        public TMP_Text timeText;
        public TMP_Text scoreText;

        public void Enable(bool state = true)
        {
            display.enabled = state;
        }
    }
}
