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
}
