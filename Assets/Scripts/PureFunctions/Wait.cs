using System;
using System.Collections;
using UnityEngine;

namespace PureFunctions
{
   public static class Wait
   {
      public static IEnumerator WaitThenCallBack(float seconds, Action callBack)
      {
         yield return new WaitForSeconds(seconds);
         callBack();
      }
   }
}
