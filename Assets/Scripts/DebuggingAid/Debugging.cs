using System;
using UnityEngine;

namespace DebuggingAid
{
    /// <summary>
    /// Send debug messages through this class so they can be switched on and off as needed.
    /// I recommend not logging errors through here as we want them to display regardless.
    /// </summary>
    public static class Debugging
    {
        private const bool DisplayDebugMessages = ProjectManager.EnabledFeatures.DebugMessages;
        #region Common Error Messages
        //The following strings are used consistently as error messages throughout the project so instead of having multiple instances of the same error messages across classes we save them here.
        public const string IsNotAccountedForInSwitchStatement = " is not accounted for in this switch statement! ";
        public const char FullStop = '.';
        public const string DuplicateErrorMessagePrefix = "There are 2 ";
        public const string DuplicateSingletonErrorMessageSuffix = " singletons in the scene. Removing ";
        public const string CanNotFindSingletonErrorMessage = "Can not find singleton: ";

        private const string UnusedAssetsRemovedWarning = "Unused assets have been forcefully unloaded and garbage has been collected manually.";
        #endregion Common Error Messages

        public static void DisplayDebugMessage(string message)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (DisplayDebugMessages)
            {
                Debug.Log(message);
            }
        }

        public static void ClearUnusedAssetsAndCollectGarbage()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
            DisplayDebugMessage(UnusedAssetsRemovedWarning);
        }
    }
}
