using System;
using UnityEngine;

public static class Debugging
{
    private const bool DisplayDebugMessages = true;
    
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
        DisplayDebugMessage("Unused assets have been forcefully unloaded and garbage has been collected manually.");
    }
}
