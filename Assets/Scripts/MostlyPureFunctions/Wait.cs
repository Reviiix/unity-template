using System;
using System.Collections;
using UnityEngine;

namespace MostlyPureFunctions
{
   public static class Wait
   {
      public static IEnumerator WaitThenCallBack(float seconds, Action callBack)
      {
         yield return new WaitForSeconds(seconds);
         callBack();
      }
      
      public static IEnumerator WaitForAnyAsynchronousInitialisationToComplete(Action callBack)
      {
         yield return new WaitUntil(() => ProjectManager.HasBeenInitialised);
         callBack();
      }
   }
}
