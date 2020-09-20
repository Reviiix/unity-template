using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PureFunctions
{
    public static class ChangeTextColors
    {
        public static void Change(IEnumerable<TMP_Text> textsToChange, Color newColor)
        {
            foreach (var text in textsToChange)
            {
                text.color = newColor;
            }
        }
    }
}
