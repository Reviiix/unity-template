using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PureFunctions
{
    public static class ChangeTextColour
    {
        public static void Change(IEnumerable<TMP_Text> textsToChange, Color newColor)
        {
            foreach (var text in textsToChange)
            {
                text.color = newColor;
            }
        }
        
        private static Coroutine _changeSequence;
        private static MonoBehaviour _coRoutineHandler;
        private const float ChangeTime = 0.75f;
        private static readonly WaitForSeconds WaitChangeTime = new WaitForSeconds(ChangeTime);

        public static void Change(TMP_Text textToChange, Color firstColor, Color secondColor, MonoBehaviour coRoutineHandler)
        {
            _coRoutineHandler = coRoutineHandler;
            _changeSequence = _coRoutineHandler.StartCoroutine(ChangeTextColorSequence(textToChange, firstColor, secondColor, _coRoutineHandler));
        }
      
        public static void StopChangeTextColorSequence()
        {
            if (_changeSequence == null) return;
         
            _coRoutineHandler.StopCoroutine(_changeSequence);
        }

        private static IEnumerator ChangeTextColorSequence(TMP_Text textToChange, Color firstColor, Color secondColor, MonoBehaviour coRoutineHandler)
        {
            textToChange.color = firstColor;
            yield return WaitChangeTime;
            textToChange.color = secondColor;
            yield return WaitChangeTime;
            Change(textToChange, firstColor, secondColor, coRoutineHandler);
        }
    }
}
