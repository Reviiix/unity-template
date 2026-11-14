using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
   /// <summary>
   /// The static wait class is intended to hold all generic waiting co routines that can be used throughout the application.
   /// </summary>
   public static class Wait
   {
      private static readonly WaitUntil WaitForRemoteConfig = new (()=>RemoteConfigurationManager.RemoteConfigSet);
      public static readonly WaitUntil WaitForInitialisation = new (() => ProjectManager.Initialised);
      
      public static IEnumerator WaitThenCallBack(float seconds, Action callBack)
      {
         yield return new WaitForSeconds(seconds);
         callBack();
      }
      
      public static IEnumerator WaitForInitialisationToComplete(Action callBack)
      {
         yield return WaitForInitialisation;
         callBack();
      }

      public static IEnumerator WaitForRemoteConfigToUpdate(Action callBack)
      {
         yield return WaitForRemoteConfig;
         callBack();
      }
   }
}
