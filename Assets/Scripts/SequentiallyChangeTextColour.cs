using System.Collections;
using TMPro;
using UnityEngine;

public static class SequentiallyChangeTextColour
{
   private const float ChangeTime = 0.75f;
   private static readonly WaitForSeconds WaitChangeTime = new WaitForSeconds(ChangeTime);

   public static Coroutine StartChangeTextColorSequence(TMP_Text textToChange, Color firstColor, Color secondColor)
   {
      return GameManager.instance.StartCoroutine(ChangeTextColorSequence(textToChange, firstColor, secondColor));
   }

   private static IEnumerator ChangeTextColorSequence(TMP_Text textToChange, Color firstColor, Color secondColor)
   {
      textToChange.color = firstColor;
      yield return WaitChangeTime;
      textToChange.color = secondColor;
      yield return WaitChangeTime;
      StartChangeTextColorSequence(textToChange, firstColor, secondColor);
   }
}
