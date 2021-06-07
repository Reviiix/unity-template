using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MostlyPureFunctions.Effects
{
    public static class ChangeTextColour
    {
        private static MonoBehaviour CoRoutineHandler => ProjectManager.Instance;
        private static Coroutine _changeSequence;
        private const float ChangeTime = 0.75f;
        private static readonly WaitForSeconds WaitChangeTime = new WaitForSeconds(ChangeTime);
        
        public static void Change(IEnumerable<TMP_Text> textsToChange, Color newColor)
        {
            foreach (var text in textsToChange)
            {
                text.color = newColor;
            }
        }
        
        public static void Change(TMP_Text textToChange, Color firstColor, Color secondColor)
        {
            _changeSequence = CoRoutineHandler.StartCoroutine(ChangeTextColorSequence(textToChange, firstColor, secondColor));
        }

        public static void StopChangeTextColorSequence()
        {
            if (_changeSequence == null) return;
         
            CoRoutineHandler.StopCoroutine(_changeSequence);
        }

        private static IEnumerator ChangeTextColorSequence(TMP_Text textToChange, Color firstColor, Color secondColor)
        {
            textToChange.color = firstColor;
            yield return WaitChangeTime;
            textToChange.color = secondColor;
            yield return WaitChangeTime;
            Change(textToChange, firstColor, secondColor);
        }
    }
}
